using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Services;

/// <summary>
/// Combat management service implementation.
/// 戰鬥管理服務實作。
/// </summary>
public class CombatManager : ICombatManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Random _random = new();
    private const int COMBAT_TICK_INTERVAL_MS = 3000; // 3 seconds between auto-attacks

    public CombatManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string> StartCombatAsync(Guid playerId, string targetName)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = await db.PlayerCharacters.FindAsync(playerId);
        if (player == null) return "Player not found.";

        // Check if already in combat
        var existingCombat = await db.ActiveCombats.FirstOrDefaultAsync(c => c.PlayerId == playerId);
        if (existingCombat != null) return "You are already in combat!";

        // Find monster spawn in the same room
        var monsterSpawn = await db.MonsterSpawns
            .Include(ms => ms.MonsterTemplate)
            .FirstOrDefaultAsync(ms =>
                ms.RoomId == player.CurrentRoomId &&
                ms.IsAlive &&
                !ms.InCombat &&
                ms.MonsterTemplate != null &&
                ms.MonsterTemplate.Name.ToLower().Contains(targetName.ToLower()));

        if (monsterSpawn == null)
        {
            // Try to find by exact monster template name
            var monster = await db.Monsters.FirstOrDefaultAsync(m =>
                m.LocationId == player.CurrentRoomId &&
                m.Name.ToLower().Contains(targetName.ToLower()));

            if (monster == null)
                return $"No '{targetName}' found here.";

            // Spawn a new instance of this monster
            monsterSpawn = new MonsterSpawn
            {
                Id = Guid.NewGuid(),
                MonsterTemplateId = monster.Id,
                RoomId = player.CurrentRoomId,
                CurrentHp = monster.MaxHp,
                InCombat = false,
                SpawnedAt = DateTime.UtcNow
            };
            db.MonsterSpawns.Add(monsterSpawn);
            await db.SaveChangesAsync();

            // Reload with template
            monsterSpawn = await db.MonsterSpawns
                .Include(ms => ms.MonsterTemplate)
                .FirstAsync(ms => ms.Id == monsterSpawn.Id);
        }

        // Start combat
        monsterSpawn.InCombat = true;
        monsterSpawn.EngagedPlayerId = playerId;

        var combat = new ActiveCombat
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            MonsterSpawnId = monsterSpawn.Id,
            StartedAt = DateTime.UtcNow,
            LastTick = DateTime.UtcNow,
            NextTick = DateTime.UtcNow.AddMilliseconds(COMBAT_TICK_INTERVAL_MS)
        };
        db.ActiveCombats.Add(combat);

        await db.SaveChangesAsync();

        var monsterName = monsterSpawn.MonsterTemplate?.Name ?? "Monster";
        return $"<span class='text-red-400'>⚔️ Combat started with {monsterName}!</span>\n" +
               $"<span class='text-gray-400'>{monsterName} HP: {monsterSpawn.CurrentHp}/{monsterSpawn.MonsterTemplate?.MaxHp}</span>";
    }

    public async Task<string> FleeAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var combat = await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .FirstOrDefaultAsync(c => c.PlayerId == playerId);

        if (combat == null) return "You are not in combat.";

        // 50% chance to flee successfully
        if (_random.Next(100) < 50)
        {
            // Failed to flee - monster gets a free attack
            var player = await db.PlayerCharacters.FindAsync(playerId);
            if (player != null && combat.MonsterSpawn?.MonsterTemplate != null)
            {
                int damage = Math.Max(1, combat.MonsterSpawn.MonsterTemplate.Attack - (player.Stats.Con / 2));
                player.CurrentHp -= damage;
                if (player.CurrentHp < 0) player.CurrentHp = 0;
                await db.SaveChangesAsync();

                return $"<span class='text-yellow-400'>You failed to flee!</span>\n" +
                       $"<span class='text-red-400'>{combat.MonsterSpawn.MonsterTemplate.Name} hits you for {damage} damage!</span>";
            }
            return "<span class='text-yellow-400'>You failed to flee!</span>";
        }

        // Successfully fled
        if (combat.MonsterSpawn != null)
        {
            combat.MonsterSpawn.InCombat = false;
            combat.MonsterSpawn.EngagedPlayerId = null;
        }
        db.ActiveCombats.Remove(combat);
        await db.SaveChangesAsync();

        return "<span class='text-green-400'>You successfully fled from combat!</span>";
    }

    public async Task<string> UseSkillAsync(Guid playerId, string skillId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var player = await db.PlayerCharacters.FindAsync(playerId);
        if (player == null) return "Player not found.";

        var combat = await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .FirstOrDefaultAsync(c => c.PlayerId == playerId);

        var skill = await db.Skills.FirstOrDefaultAsync(s => s.SkillId == skillId.ToLower());
        if (skill == null) return $"Skill '{skillId}' not found.";

        // Check if player can use this skill
        if (skill.RequiredClass.HasValue && skill.RequiredClass.Value != player.Class)
            return $"This skill requires {skill.RequiredClass.Value} class.";

        if (skill.RequiredLevel > player.Level)
            return $"This skill requires level {skill.RequiredLevel}.";

        if (player.CurrentMp < skill.MpCost)
            return $"Not enough MP! ({player.CurrentMp}/{skill.MpCost} required)";

        // Deduct MP
        player.CurrentMp -= skill.MpCost;

        // Calculate skill effect
        int statValue = skill.ScalingStat switch
        {
            "STR" => player.Stats.Str,
            "INT" => player.Stats.Int,
            "WIS" => player.Stats.Wis,
            "DEX" => player.Stats.Dex,
            "CON" => player.Stats.Con,
            _ => 10
        };

        int power = (int)(skill.BasePower + (statValue * skill.ScalingMultiplier));
        power = (int)(power * (0.9 + _random.NextDouble() * 0.2)); // 90-110% variance

        string result = "";

        switch (skill.Type)
        {
            case SkillType.Physical:
            case SkillType.Magical:
                if (combat == null || combat.MonsterSpawn == null)
                {
                    player.CurrentMp += skill.MpCost; // Refund MP
                    return "You need to be in combat to use attack skills!";
                }

                int damage = Math.Max(1, power - combat.MonsterSpawn.MonsterTemplate!.Defense);
                combat.MonsterSpawn.CurrentHp -= damage;

                string colorClass = skill.Type == SkillType.Magical ? "text-blue-400" : "text-orange-400";
                result = $"<span class='{colorClass}'>⚡ You use {skill.Name}! Deals {damage} damage!</span>";

                if (combat.MonsterSpawn.CurrentHp <= 0)
                {
                    combat.MonsterSpawn.CurrentHp = 0;
                    result += $"\n<span class='text-yellow-400'>🎉 {combat.MonsterSpawn.MonsterTemplate.Name} defeated!</span>";
                }
                break;

            case SkillType.Healing:
                int healAmount = power;
                int oldHp = player.CurrentHp;
                player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + healAmount);
                int actualHeal = player.CurrentHp - oldHp;

                result = $"<span class='text-green-400'>💚 You use {skill.Name}! Healed for {actualHeal} HP!</span>";
                break;
        }

        await db.SaveChangesAsync();
        return result;
    }

    public async Task<CombatTickResult> ProcessCombatTickAsync(Guid combatId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var combat = await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .Include(c => c.Player)
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null || combat.Player == null || combat.MonsterSpawn?.MonsterTemplate == null)
        {
            return new CombatTickResult { CombatEnded = true, Message = "Combat not found." };
        }

        var player = combat.Player;
        var monsterSpawn = combat.MonsterSpawn;
        var monster = monsterSpawn.MonsterTemplate;

        var result = new CombatTickResult
        {
            PlayerId = player.Id,
            PlayerCurrentHp = player.CurrentHp,
            PlayerMaxHp = player.MaxHp,
            MonsterCurrentHp = monsterSpawn.CurrentHp,
            MonsterMaxHp = monster.MaxHp
        };

        // Player auto-attack
        int playerDamage = CalculatePlayerDamage(player, monster);
        monsterSpawn.CurrentHp -= playerDamage;
        result.Message = $"<span class='text-orange-400'>You hit {monster.Name} for {playerDamage} damage!</span>";
        result.MonsterCurrentHp = monsterSpawn.CurrentHp;

        // Check monster death
        if (monsterSpawn.CurrentHp <= 0)
        {
            monsterSpawn.CurrentHp = 0;
            monsterSpawn.KilledAt = DateTime.UtcNow;
            monsterSpawn.InCombat = false;

            // Award exp
            player.Exp += monster.ExpReward;
            result.ExpGained = monster.ExpReward;

            // Process loot
            var lootEntries = await db.LootTableEntries
                .Include(l => l.Item)
                .Where(l => l.MonsterId == monster.Id)
                .ToListAsync();

            foreach (var lootEntry in lootEntries)
            {
                if (_random.NextDouble() * 100 <= lootEntry.DropRate)
                {
                    int quantity = _random.Next(lootEntry.MinQuantity, lootEntry.MaxQuantity + 1);

                    // Add to player inventory
                    var existingItem = await db.InventoryItems
                        .FirstOrDefaultAsync(i => i.PlayerId == player.Id && i.ItemId == lootEntry.ItemId && !i.IsEquipped);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                    }
                    else
                    {
                        var newItem = new InventoryItem
                        {
                            Id = Guid.NewGuid(),
                            PlayerId = player.Id,
                            ItemId = lootEntry.ItemId,
                            Quantity = quantity,
                            SlotIndex = await GetNextInventorySlot(db, player.Id)
                        };
                        db.InventoryItems.Add(newItem);
                    }

                    result.Loot.Add(new LootDrop
                    {
                        ItemId = lootEntry.ItemId,
                        ItemName = lootEntry.Item?.Name ?? "Unknown",
                        Quantity = quantity
                    });
                }
            }

            result.Message += $"\n<span class='text-yellow-400'>🎉 {monster.Name} defeated! +{monster.ExpReward} EXP</span>";

            if (result.Loot.Any())
            {
                result.Message += "\n<span class='text-cyan-400'>📦 Loot: " +
                    string.Join(", ", result.Loot.Select(l => $"{l.ItemName} x{l.Quantity}")) + "</span>";
            }

            result.CombatEnded = true;
            result.MonsterDied = true;

            // Check level up
            var (didLevelUp, levelUpMsg) = await CheckLevelUp(db, player);
            if (didLevelUp)
            {
                result.Message += $"\n{levelUpMsg}";
            }

            // Remove combat and monster spawn
            db.ActiveCombats.Remove(combat);
            db.MonsterSpawns.Remove(monsterSpawn);
        }
        else
        {
            // Monster counter-attack
            int monsterDamage = Math.Max(1, monster.Attack - (player.Stats.Con / 2));
            monsterDamage = (int)(monsterDamage * (0.9 + _random.NextDouble() * 0.2));
            player.CurrentHp -= monsterDamage;
            result.PlayerCurrentHp = player.CurrentHp;

            result.Message += $"\n<span class='text-red-400'>{monster.Name} hits you for {monsterDamage} damage!</span>";

            // Check player death
            if (player.CurrentHp <= 0)
            {
                player.CurrentHp = 0;
                result.PlayerDied = true;
                result.CombatEnded = true;

                // Handle player death (respawn at village)
                player.CurrentHp = player.MaxHp / 2;
                player.CurrentMp = player.MaxMp / 2;
                player.CurrentRoomId = 1; // Village Square
                player.Exp = Math.Max(0, player.Exp - monster.ExpReward); // Lose some exp

                result.Message += "\n<span class='text-red-500'>💀 You have died! Respawning at Village Square...</span>";

                // Clean up combat
                monsterSpawn.InCombat = false;
                monsterSpawn.EngagedPlayerId = null;
                db.ActiveCombats.Remove(combat);
            }
            else
            {
                // Update next tick time
                combat.LastTick = DateTime.UtcNow;
                combat.NextTick = DateTime.UtcNow.AddMilliseconds(COMBAT_TICK_INTERVAL_MS);
            }
        }

        await db.SaveChangesAsync();
        return result;
    }

    public async Task<List<ActiveCombat>> GetActiveCombatsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.ActiveCombats
            .Where(c => c.NextTick <= DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<bool> IsInCombatAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.ActiveCombats.AnyAsync(c => c.PlayerId == playerId);
    }

    public async Task<ActiveCombat?> GetPlayerCombatAsync(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await db.ActiveCombats
            .Include(c => c.MonsterSpawn)
            .ThenInclude(ms => ms!.MonsterTemplate)
            .FirstOrDefaultAsync(c => c.PlayerId == playerId);
    }

    private int CalculatePlayerDamage(PlayerCharacter player, Monster monster)
    {
        int baseDamage = player.Stats.Str * 2;
        int damage = Math.Max(1, baseDamage - monster.Defense);
        return (int)(damage * (0.9 + _random.NextDouble() * 0.2));
    }

    private async Task<int> GetNextInventorySlot(AppDbContext db, Guid playerId)
    {
        var usedSlots = await db.InventoryItems
            .Where(i => i.PlayerId == playerId)
            .Select(i => i.SlotIndex)
            .ToListAsync();

        for (int i = 0; i < 25; i++)
        {
            if (!usedSlots.Contains(i)) return i;
        }
        return -1; // Inventory full
    }

    private async Task<(bool, string)> CheckLevelUp(AppDbContext db, PlayerCharacter player)
    {
        // Simple level formula: ExpRequired = Level * 100
        long expRequired = player.Level * 100;

        if (player.Exp >= expRequired)
        {
            player.Level++;
            player.Exp -= expRequired;

            // Increase stats based on class
            switch (player.Class)
            {
                case ClassType.Warrior:
                    player.Stats.Str += 3;
                    player.Stats.Con += 2;
                    player.Stats.Dex += 1;
                    player.MaxHp += 15;
                    player.MaxMp += 5;
                    break;
                case ClassType.Mage:
                    player.Stats.Int += 3;
                    player.Stats.Wis += 2;
                    player.Stats.Dex += 1;
                    player.MaxHp += 8;
                    player.MaxMp += 15;
                    break;
                case ClassType.Priest:
                    player.Stats.Wis += 3;
                    player.Stats.Int += 2;
                    player.Stats.Con += 1;
                    player.MaxHp += 10;
                    player.MaxMp += 12;
                    break;
            }

            // Restore HP/MP on level up
            player.CurrentHp = player.MaxHp;
            player.CurrentMp = player.MaxMp;

            return (true, $"<span class='text-yellow-300'>🌟 LEVEL UP! You are now level {player.Level}!</span>");
        }

        return (false, "");
    }
}
