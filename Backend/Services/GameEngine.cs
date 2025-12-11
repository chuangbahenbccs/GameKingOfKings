using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KingOfKings.Backend.Services;

public class GameEngine : IGameEngine
{
    private readonly IServiceProvider _serviceProvider;

    public GameEngine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string> ProcessCommandAsync(Guid playerId, string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return string.Empty;

        var parts = command.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var action = parts[0].ToLower();
        var args = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var combatManager = scope.ServiceProvider.GetRequiredService<ICombatManager>();

        var player = await db.PlayerCharacters
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return "Error: Player not found.";

        // Check if in combat for movement restriction
        var inCombat = await combatManager.IsInCombatAsync(playerId);

        switch (action)
        {
            case "look":
            case "l":
                return await HandleLook(db, player, inCombat);
            case "move":
            case "go":
            case "n":
            case "s":
            case "e":
            case "w":
            case "north":
            case "south":
            case "east":
            case "west":
                if (inCombat) return "<span class='text-yellow-400'>You cannot move while in combat! Use 'flee' to escape.</span>";
                return await HandleMove(db, player, action, args);
            case "say":
                return $"You say: \"{args}\"";
            case "kill":
            case "attack":
            case "k":
                if (string.IsNullOrEmpty(args)) return "Attack what? Usage: kill <target>";
                return await combatManager.StartCombatAsync(playerId, args);
            case "flee":
            case "run":
                return await combatManager.FleeAsync(playerId);
            case "cast":
            case "skill":
                if (string.IsNullOrEmpty(args)) return await HandleSkillList(db, player);
                return await combatManager.UseSkillAsync(playerId, args);
            case "status":
            case "stats":
            case "st":
                return await HandleStatus(db, player, inCombat);
            case "inventory":
            case "inv":
            case "i":
                return await HandleInventory(db, playerId);
            case "equip":
                if (string.IsNullOrEmpty(args)) return "Equip what? Usage: equip <item name>";
                return await HandleEquip(db, playerId, args);
            case "unequip":
                if (string.IsNullOrEmpty(args)) return "Unequip what? Usage: unequip <item name>";
                return await HandleUnequip(db, playerId, args);
            case "use":
                if (string.IsNullOrEmpty(args)) return "Use what? Usage: use <item name>";
                return await HandleUseItem(db, playerId, args);
            case "rest":
                if (inCombat) return "<span class='text-yellow-400'>You cannot rest while in combat!</span>";
                return await HandleRest(db, player);
            case "help":
            case "?":
                return HandleHelp();
            default:
                return "Unknown command. Type 'help' for a list of commands.";
        }
    }

    private async Task<string> HandleLook(AppDbContext db, PlayerCharacter player, bool inCombat)
    {
        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "You are in the void.";

        var result = $"<div class='text-yellow-400 font-bold'>{room.Name}</div><div>{room.Description}</div>";

        // Show monsters in room
        var monsters = await db.Monsters.Where(m => m.LocationId == player.CurrentRoomId).ToListAsync();
        if (monsters.Any())
        {
            result += "\n<div class='text-red-300 mt-2'>Monsters here: " +
                string.Join(", ", monsters.Select(m => m.Name)) + "</div>";
        }

        // Show exits
        var exits = JsonSerializer.Deserialize<Dictionary<string, int>>(room.ExitsJson);
        if (exits != null && exits.Any())
        {
            var exitNames = exits.Keys.Select(e => e switch
            {
                "n" => "North",
                "s" => "South",
                "e" => "East",
                "w" => "West",
                _ => e
            });
            result += $"\n<div class='text-green-300'>Exits: {string.Join(", ", exitNames)}</div>";
        }

        if (inCombat)
        {
            result += "\n<div class='text-red-400'>⚔️ You are in combat!</div>";
        }

        return result;
    }

    private async Task<string> HandleMove(AppDbContext db, PlayerCharacter player, string action, string args)
    {
        // Normalize direction
        string direction = action;
        if (action == "move" || action == "go") direction = args;

        if (string.IsNullOrEmpty(direction)) return "Go where? (n/s/e/w)";

        direction = direction.ToLower().Substring(0, 1); // n, s, e, w

        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "You are stuck.";

        var exits = JsonSerializer.Deserialize<Dictionary<string, int>>(room.ExitsJson);

        if (exits != null && exits.ContainsKey(direction))
        {
            player.CurrentRoomId = exits[direction];
            await db.SaveChangesAsync();
            return await HandleLook(db, player, false);
        }

        return "You cannot go that way.";
    }

    private async Task<string> HandleSkillList(AppDbContext db, PlayerCharacter player)
    {
        var skills = await db.Skills
            .Where(s => (!s.RequiredClass.HasValue || s.RequiredClass == player.Class) && s.RequiredLevel <= player.Level)
            .ToListAsync();

        if (!skills.Any())
            return "You don't know any skills yet.";

        var result = "<div class='text-cyan-400 font-bold'>Available Skills:</div>";
        foreach (var skill in skills)
        {
            string typeColor = skill.Type switch
            {
                SkillType.Physical => "text-orange-400",
                SkillType.Magical => "text-blue-400",
                SkillType.Healing => "text-green-400",
                _ => "text-white"
            };
            result += $"\n<div class='{typeColor}'>• {skill.Name} ({skill.SkillId}) - MP: {skill.MpCost} - {skill.Description}</div>";
        }
        result += "\n<div class='text-gray-400'>Usage: cast <skill_id></div>";

        return result;
    }

    private async Task<string> HandleStatus(AppDbContext db, PlayerCharacter player, bool inCombat)
    {
        var result = $@"<div class='text-yellow-400 font-bold'>═══ {player.Name} ═══</div>
<div>Class: <span class='text-cyan-400'>{player.Class}</span> | Level: <span class='text-green-400'>{player.Level}</span></div>
<div>EXP: <span class='text-yellow-300'>{player.Exp}/{player.Level * 100}</span></div>
<div class='mt-2'>HP: <span class='text-red-400'>{player.CurrentHp}/{player.MaxHp}</span></div>
<div>MP: <span class='text-blue-400'>{player.CurrentMp}/{player.MaxMp}</span></div>
<div class='mt-2 text-gray-300'>═══ Stats ═══</div>
<div>STR: {player.Stats.Str} | DEX: {player.Stats.Dex} | CON: {player.Stats.Con}</div>
<div>INT: {player.Stats.Int} | WIS: {player.Stats.Wis}</div>";

        if (inCombat)
        {
            result += "\n<div class='text-red-400 mt-2'>⚔️ Currently in combat!</div>";
        }

        return result;
    }

    private async Task<string> HandleInventory(AppDbContext db, Guid playerId)
    {
        var items = await db.InventoryItems
            .Include(i => i.Item)
            .Where(i => i.PlayerId == playerId)
            .OrderBy(i => i.SlotIndex)
            .ToListAsync();

        if (!items.Any())
            return "<div class='text-gray-400'>Your inventory is empty.</div>";

        var result = "<div class='text-yellow-400 font-bold'>═══ Inventory ═══</div>";

        // Show equipped items first
        var equipped = items.Where(i => i.IsEquipped).ToList();
        if (equipped.Any())
        {
            result += "\n<div class='text-cyan-400'>Equipped:</div>";
            foreach (var item in equipped)
            {
                result += $"\n<div class='text-green-400'>  [{item.EquippedSlot}] {item.Item?.Name}</div>";
            }
        }

        // Show backpack items
        var backpack = items.Where(i => !i.IsEquipped).ToList();
        if (backpack.Any())
        {
            result += "\n<div class='text-gray-300 mt-2'>Backpack:</div>";
            foreach (var item in backpack)
            {
                string typeColor = item.Item?.Type switch
                {
                    ItemType.Weapon => "text-orange-400",
                    ItemType.Armor => "text-blue-400",
                    ItemType.Consumable => "text-green-400",
                    ItemType.Quest => "text-purple-400",
                    _ => "text-white"
                };
                result += $"\n<div class='{typeColor}'>  • {item.Item?.Name} x{item.Quantity}</div>";
            }
        }

        return result;
    }

    private async Task<string> HandleEquip(AppDbContext db, Guid playerId, string itemName)
    {
        var inventoryItem = await db.InventoryItems
            .Include(i => i.Item)
            .FirstOrDefaultAsync(i =>
                i.PlayerId == playerId &&
                !i.IsEquipped &&
                i.Item != null &&
                i.Item.Name.ToLower().Contains(itemName.ToLower()));

        if (inventoryItem?.Item == null)
            return $"You don't have '{itemName}' in your inventory.";

        if (inventoryItem.Item.Type != ItemType.Weapon && inventoryItem.Item.Type != ItemType.Armor)
            return "You can only equip weapons and armor.";

        // Determine slot
        var slot = inventoryItem.Item.Type == ItemType.Weapon ? EquipmentSlot.Weapon : EquipmentSlot.Body;

        // Check if something is already equipped in that slot
        var currentEquipped = await db.InventoryItems
            .FirstOrDefaultAsync(i => i.PlayerId == playerId && i.IsEquipped && i.EquippedSlot == slot);

        if (currentEquipped != null)
        {
            currentEquipped.IsEquipped = false;
            currentEquipped.EquippedSlot = EquipmentSlot.None;
        }

        inventoryItem.IsEquipped = true;
        inventoryItem.EquippedSlot = slot;

        await db.SaveChangesAsync();

        return $"<span class='text-green-400'>You equipped {inventoryItem.Item.Name}.</span>";
    }

    private async Task<string> HandleUnequip(AppDbContext db, Guid playerId, string itemName)
    {
        var inventoryItem = await db.InventoryItems
            .Include(i => i.Item)
            .FirstOrDefaultAsync(i =>
                i.PlayerId == playerId &&
                i.IsEquipped &&
                i.Item != null &&
                i.Item.Name.ToLower().Contains(itemName.ToLower()));

        if (inventoryItem?.Item == null)
            return $"You don't have '{itemName}' equipped.";

        inventoryItem.IsEquipped = false;
        inventoryItem.EquippedSlot = EquipmentSlot.None;

        await db.SaveChangesAsync();

        return $"<span class='text-yellow-400'>You unequipped {inventoryItem.Item.Name}.</span>";
    }

    private async Task<string> HandleUseItem(AppDbContext db, Guid playerId, string itemName)
    {
        var inventoryItem = await db.InventoryItems
            .Include(i => i.Item)
            .FirstOrDefaultAsync(i =>
                i.PlayerId == playerId &&
                !i.IsEquipped &&
                i.Item != null &&
                i.Item.Type == ItemType.Consumable &&
                i.Item.Name.ToLower().Contains(itemName.ToLower()));

        if (inventoryItem?.Item == null)
            return $"You don't have any consumable '{itemName}' in your inventory.";

        var player = await db.PlayerCharacters.FindAsync(playerId);
        if (player == null) return "Player not found.";

        // Parse item properties
        var props = JsonSerializer.Deserialize<Dictionary<string, int>>(inventoryItem.Item.PropertiesJson);
        string result = "";

        if (props != null)
        {
            if (props.TryGetValue("HealHp", out int healHp))
            {
                int oldHp = player.CurrentHp;
                player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + healHp);
                int actualHeal = player.CurrentHp - oldHp;
                result = $"<span class='text-green-400'>You used {inventoryItem.Item.Name} and restored {actualHeal} HP!</span>";
            }
            else if (props.TryGetValue("HealMp", out int healMp))
            {
                int oldMp = player.CurrentMp;
                player.CurrentMp = Math.Min(player.MaxMp, player.CurrentMp + healMp);
                int actualHeal = player.CurrentMp - oldMp;
                result = $"<span class='text-blue-400'>You used {inventoryItem.Item.Name} and restored {actualHeal} MP!</span>";
            }
        }

        // Reduce quantity
        inventoryItem.Quantity--;
        if (inventoryItem.Quantity <= 0)
        {
            db.InventoryItems.Remove(inventoryItem);
        }

        await db.SaveChangesAsync();

        return string.IsNullOrEmpty(result) ? $"You used {inventoryItem.Item.Name}." : result;
    }

    private async Task<string> HandleRest(AppDbContext db, PlayerCharacter player)
    {
        // Restore 25% HP and MP
        int hpRestore = player.MaxHp / 4;
        int mpRestore = player.MaxMp / 4;

        int oldHp = player.CurrentHp;
        int oldMp = player.CurrentMp;

        player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + hpRestore);
        player.CurrentMp = Math.Min(player.MaxMp, player.CurrentMp + mpRestore);

        await db.SaveChangesAsync();

        int actualHpRestore = player.CurrentHp - oldHp;
        int actualMpRestore = player.CurrentMp - oldMp;

        return $"<span class='text-green-400'>You rest for a moment...</span>\n" +
               $"<span class='text-red-300'>HP restored: +{actualHpRestore}</span>\n" +
               $"<span class='text-blue-300'>MP restored: +{actualMpRestore}</span>";
    }

    private string HandleHelp()
    {
        return @"<div class='text-yellow-400 font-bold'>═══ Commands ═══</div>
<div class='text-cyan-400'>Movement:</div>
<div>  north (n), south (s), east (e), west (w) - Move in direction</div>
<div>  look (l) - Look around</div>
<div class='text-red-400 mt-2'>Combat:</div>
<div>  kill <target> - Attack a monster</div>
<div>  flee - Try to escape from combat</div>
<div>  cast <skill> - Use a skill</div>
<div class='text-green-400 mt-2'>Character:</div>
<div>  status (st) - View your stats</div>
<div>  inventory (i) - View your inventory</div>
<div>  equip <item> - Equip an item</div>
<div>  unequip <item> - Unequip an item</div>
<div>  use <item> - Use a consumable item</div>
<div>  rest - Rest to recover HP/MP</div>
<div class='text-gray-400 mt-2'>Other:</div>
<div>  say <message> - Say something</div>
<div>  help - Show this help</div>";
    }

    public async Task<Guid> LoginOrRegisterAsync(string username)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = "dummy" // No real auth yet
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var player = await db.PlayerCharacters.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (player == null)
        {
            player = new PlayerCharacter
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = username,
                Class = ClassType.Warrior,
                CurrentRoomId = 1,
                Stats = new CharacterStats { Str = 12, Dex = 10, Int = 8, Wis = 8, Con = 12 }
            };
            db.PlayerCharacters.Add(player);
            await db.SaveChangesAsync();
        }

        return player.Id;
    }

    public async Task TickAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // HP/MP regeneration for players not in combat
        var playersNotInCombat = await db.PlayerCharacters
            .Where(p => !db.ActiveCombats.Any(c => c.PlayerId == p.Id))
            .ToListAsync();

        foreach (var player in playersNotInCombat)
        {
            // Regenerate 1% HP and MP per tick
            if (player.CurrentHp < player.MaxHp)
            {
                player.CurrentHp = Math.Min(player.MaxHp, player.CurrentHp + Math.Max(1, player.MaxHp / 100));
            }
            if (player.CurrentMp < player.MaxMp)
            {
                player.CurrentMp = Math.Min(player.MaxMp, player.CurrentMp + Math.Max(1, player.MaxMp / 50));
            }
        }

        await db.SaveChangesAsync();
    }
}
