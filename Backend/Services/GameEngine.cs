using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Services;

public class GameEngine : IGameEngine
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CombatManager _combatManager;
    
    public GameEngine(IServiceProvider serviceProvider, CombatManager combatManager)
    {
        _serviceProvider = serviceProvider;
        _combatManager = combatManager;
    }

    public async Task<string> ProcessCommandAsync(Guid playerId, string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return string.Empty;

        var parts = command.Trim().Split(' ');
        var action = parts[0].ToLower();
        var args = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var player = await db.PlayerCharacters
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return "錯誤：找不到玩家。";

        switch (action)
        {
            case "help":
            case "h":
            case "?":
                return GetHelpText();
            case "look":
            case "l":
                return await HandleLook(db, player);
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
                return await HandleMove(db, player, action, args);
            case "say":
                return $"你說：「{args}」";
            case "kill":
            case "attack":
            case "k":
                return await HandleCombat(db, player, args);
            case "flee":
            case "run":
                return await HandleFlee(player);
            case "cast":
            case "c":
                return await HandleCastSkill(player, args);
            case "skills":
            case "skill":
                return await HandleShowSkills(db, player);
            default:
                return $"未知的指令：'{command}'。輸入 'help' 查看可用指令。";
        }
    }

    private string GetHelpText()
    {
        return @"<div class='text-green-400'>
<b>==== 遊戲指令說明 ====</b>

<b class='text-yellow-400'>基本指令：</b>
• <span class='text-cyan-400'>help, h, ?</span> - 顯示此說明
• <span class='text-cyan-400'>look, l</span> - 查看當前位置
• <span class='text-cyan-400'>say [訊息]</span> - 說話

<b class='text-yellow-400'>移動指令：</b>
• <span class='text-cyan-400'>n, north</span> - 向北移動
• <span class='text-cyan-400'>s, south</span> - 向南移動
• <span class='text-cyan-400'>e, east</span> - 向東移動
• <span class='text-cyan-400'>w, west</span> - 向西移動

<b class='text-yellow-400'>戰鬥指令：</b>
• <span class='text-cyan-400'>kill [怪物名稱]</span> - 攻擊怪物
• <span class='text-cyan-400'>attack [怪物名稱]</span> - 攻擊怪物
• <span class='text-cyan-400'>k [怪物名稱]</span> - 快速攻擊
• <span class='text-cyan-400'>flee, run</span> - 逃離戰鬥

<b class='text-yellow-400'>技能指令：</b>
• <span class='text-cyan-400'>cast [技能名稱]</span> - 施放技能
• <span class='text-cyan-400'>c [技能名稱]</span> - 快速施放
• <span class='text-cyan-400'>skills</span> - 查看可用技能

<b class='text-yellow-400'>範例：</b>
• kill 木頭人偶
• cast bash (戰士技能)
• cast fireball (法師技能)
• cast heal (牧師技能)
</div>";
    }

    private async Task<string> HandleCombat(AppDbContext db, PlayerCharacter player, string targetName)
    {
        if (string.IsNullOrWhiteSpace(targetName)) return "要攻擊什麼？";

        // 清理目標名稱，移除所有HTML標記和多餘的符號
        targetName = System.Text.RegularExpressions.Regex.Replace(targetName, @"</?[^>]*>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        targetName = targetName.Replace("'", "").Replace("\"", "").Replace("'", "").Replace(""", "").Replace(""", "").Trim();

        // Check if already in combat
        var session = _combatManager.GetSession(player.Id);
        if (session != null) return "你已經在戰鬥中了！";

        // Find monster in room - 從資料庫中查找怪物
        var monster = await db.Monsters.FirstOrDefaultAsync(m =>
            m.LocationId == player.CurrentRoomId &&
            (m.Name.ToLower() == targetName.ToLower() ||
             m.Name.ToLower().Contains(targetName.ToLower())));

        if (monster == null)
        {
            return $"這裡沒有 '{targetName}'。";
        }

        // 複製怪物實例以避免修改資料庫中的原始資料
        var combatMonster = new Monster
        {
            Id = monster.Id,
            Name = monster.Name,
            MaxHp = monster.MaxHp,
            CurrentHp = monster.MaxHp, // 重置為滿血
            Attack = monster.Attack,
            Defense = monster.Defense,
            ExpReward = monster.ExpReward,
            LocationId = monster.LocationId
        };

        _combatManager.StartCombat(player, combatMonster);
        return $"你開始攻擊 {combatMonster.Name}！";
    }

    private async Task<string> HandleFlee(PlayerCharacter player)
    {
        var session = _combatManager.GetSession(player.Id);
        if (session == null) return "你現在不在戰鬥中。";

        await session.ProcessPlayerAction("flee");
        return "你嘗試逃跑...";
    }

    private async Task<string> HandleLook(AppDbContext db, PlayerCharacter player)
    {
        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "你在虛無之中。";

        // 查找房間內的怪物
        var monsters = await db.Monsters
            .Where(m => m.LocationId == player.CurrentRoomId)
            .Select(m => m.Name)
            .Distinct()
            .ToListAsync();

        var result = $"<div class='text-yellow-400 font-bold'>{room.Name}</div><div>{room.Description}</div>";

        if (monsters.Any())
        {
            result += $"<div class='text-red-400 mt-2'>你看到這裡有：{string.Join("、", monsters)}</div>";
        }

        return result;
    }

    private async Task<string> HandleMove(AppDbContext db, PlayerCharacter player, string action, string args)
    {
        // Check if in combat
        var session = _combatManager.GetSession(player.Id);
        if (session != null) return "你必須先脫離戰鬥才能移動！使用 'flee' 指令逃跑。";

        // Normalize direction
        string direction = action;
        if (action == "move" || action == "go") direction = args;

        direction = direction.ToLower().Substring(0, 1); // n, s, e, w

        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "你被困住了。";

        // Simple JSON parsing for exits (in real app use System.Text.Json)
        // ExitsJson: {"n":2,"e":3}
        var exits = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(room.ExitsJson);

        if (exits != null && exits.ContainsKey(direction))
        {
            player.CurrentRoomId = exits[direction];
            await db.SaveChangesAsync();
            return await HandleLook(db, player);
        }

        return "你不能往那個方向走。";
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
        
        // Return player ID if exists, otherwise return empty GUID to indicate character creation needed
        return player?.Id ?? Guid.Empty;
    }

    public async Task<bool> CheckUserHasCharacter(string username)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var player = await db.PlayerCharacters.FirstOrDefaultAsync(p => p.UserId == user.Id);
        return player != null;
    }

    public async Task<Guid> CreateCharacterAsync(string username, ClassType classType)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return Guid.Empty;

        // Check if character already exists
        var existingPlayer = await db.PlayerCharacters.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (existingPlayer != null) return existingPlayer.Id;

        var player = new PlayerCharacter
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = username,
            Class = classType,
            CurrentRoomId = 1,
        };

        var stats = new CharacterStats();
        switch (classType)
        {
            case ClassType.Warrior:
                stats.Str = 15; stats.Con = 15; stats.Dex = 10; stats.Int = 5; stats.Wis = 5;
                player.MaxHp = 150; player.CurrentHp = 150;
                player.MaxMp = 30; player.CurrentMp = 30;
                break;
            case ClassType.Mage:
                stats.Int = 15; stats.Wis = 10; stats.Dex = 10; stats.Con = 8; stats.Str = 5;
                player.MaxHp = 80; player.CurrentHp = 80;
                player.MaxMp = 100; player.CurrentMp = 100;
                break;
            case ClassType.Priest:
                stats.Wis = 15; stats.Int = 10; stats.Con = 12; stats.Dex = 8; stats.Str = 5;
                player.MaxHp = 100; player.CurrentHp = 100;
                player.MaxMp = 80; player.CurrentMp = 80;
                break;
            default:
                stats.Str = 10; stats.Dex = 10; stats.Int = 10; stats.Wis = 10; stats.Con = 10;
                break;
        }
        player.Stats = stats;
        db.PlayerCharacters.Add(player);
        await db.SaveChangesAsync();

        return player.Id;
    }

    public async Task TickAsync()
    {
        // TODO: Implement regen, combat ticks
        await Task.CompletedTask;
    }

    private async Task<string> HandleCastSkill(PlayerCharacter player, string skillName)
    {
        if (string.IsNullOrWhiteSpace(skillName)) return "要施放什麼技能？";

        // 檢查是否在戰鬥中
        var session = _combatManager.GetSession(player.Id);
        if (session == null) return "你必須在戰鬥中才能施放技能！";

        // 傳送技能施放請求給戰鬥會話
        await session.ProcessPlayerAction("cast", skillName.ToLower());
        return string.Empty; // 戰鬥會話會發送結果
    }

    private async Task<string> HandleShowSkills(AppDbContext db, PlayerCharacter player)
    {
        // 獲取該職業的所有技能
        var skills = await db.Skills
            .Where(s => s.RequiredClass == player.Class)
            .ToListAsync();

        if (!skills.Any())
        {
            return "你還沒有學會任何技能。";
        }

        var result = "<div class='text-cyan-400'><b>==== 你的技能 ====</b></div>\n";

        foreach (var skill in skills)
        {
            var skillType = skill.Type switch
            {
                SkillType.Damage => "傷害",
                SkillType.Heal => "治療",
                SkillType.Buff => "增益",
                SkillType.Debuff => "減益",
                _ => "其他"
            };

            result += $"<div class='mb-2'>";
            result += $"<span class='text-yellow-400 font-bold'>{skill.Name}</span> ";
            result += $"<span class='text-gray-400'>({skillType})</span><br/>";
            result += $"<span class='text-gray-300'>{skill.Description}</span><br/>";
            result += $"<span class='text-blue-400'>MP消耗: {skill.ManaCost}</span> ";

            if (skill.CooldownSeconds > 0)
            {
                result += $"<span class='text-orange-400'>冷卻: {skill.CooldownSeconds}秒</span>";
            }

            if (skill.Type == SkillType.Damage && skill.DamageMultiplier > 0)
            {
                result += $"<br/><span class='text-red-400'>傷害倍率: {skill.DamageMultiplier:F1}x</span>";
            }
            else if (skill.Type == SkillType.Heal && skill.BaseEffectValue > 0)
            {
                result += $"<br/><span class='text-green-400'>基礎治療: {skill.BaseEffectValue}</span>";
            }

            result += "</div>";
        }

        result += "<div class='text-gray-400 mt-2'>使用指令: cast [技能名稱] 或 c [技能名稱]</div>";

        return result;
    }
}
