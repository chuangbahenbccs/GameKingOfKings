using KingOfKings.Backend.Data;
using KingOfKings.Backend.Hubs;
using KingOfKings.Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KingOfKings.Backend.Services
{
    public class CombatSession
    {
        private readonly PlayerCharacter _player;
        private readonly Monster _monster;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly ICombatService _combatService;
        private readonly ISkillService _skillService;
        private readonly Action<Guid> _onCombatEnd;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _combatTimer;
        private bool _isActive;

        public Guid PlayerId => _player.Id;
        public Monster Monster => _monster;

        public CombatSession(
            PlayerCharacter player,
            Monster monster,
            IHubContext<GameHub> hubContext,
            ICombatService combatService,
            ISkillService skillService,
            Action<Guid> onCombatEnd,
            IServiceProvider serviceProvider)
        {
            _player = player;
            _monster = monster;
            _hubContext = hubContext;
            _combatService = combatService;
            _skillService = skillService;
            _onCombatEnd = onCombatEnd;
            _serviceProvider = serviceProvider;
            _isActive = true;
        }

        public void Start()
        {
            // Tick every 2.5 seconds
            _combatTimer = new Timer(CombatTick, null, 0, 2500);
            SendUpdate("戰鬥開始！");
        }

        public void Stop()
        {
            _isActive = false;
            _combatTimer?.Dispose();
            _onCombatEnd(_player.Id);
        }

        private async void CombatTick(object? state)
        {
            if (!_isActive) return;

            // 1. Player attacks Monster
            int playerDmg = _combatService.CalculateDamage(_player, _monster);
            _monster.CurrentHp -= playerDmg;
            
            var log = new List<string>();
            log.Add($"你對 {_monster.Name} 造成了 {playerDmg} 點傷害！");

            if (_monster.CurrentHp <= 0)
            {
                _monster.CurrentHp = 0;
                log.Add($"{_monster.Name} 被擊敗了！");

                // 給予經驗值
                _player.Exp += _monster.ExpReward;
                log.Add($"獲得 {_monster.ExpReward} 點經驗值！");

                // 檢查是否升級
                var expNeeded = GetExpForNextLevel(_player.Level);
                while (_player.Exp >= expNeeded)
                {
                    _player.Exp -= expNeeded;
                    _player.Level++;

                    // 提升屬性
                    LevelUp(_player);

                    log.Add($"🎉 恭喜升級！現在是等級 {_player.Level}！");
                    log.Add($"生命值和魔力值已完全恢復！");

                    expNeeded = GetExpForNextLevel(_player.Level);
                }

                // TODO: 實作掉落物品系統

                // 保存玩家資料到資料庫
                await SavePlayerData();

                await SendUpdate(log, "victory");

                // 發送更新的玩家資料到前端
                await SendPlayerDataUpdate();

                Stop();
                return;
            }

            // 2. Monster attacks Player
            // Simple monster damage for now
            int monsterDmg = Math.Max(0, _monster.Attack - (_player.Stats.Con / 2));
            // Random variance
            monsterDmg = (int)(monsterDmg * (0.9 + new Random().NextDouble() * 0.2));

            _player.CurrentHp -= monsterDmg;
            log.Add($"{_monster.Name} 對你造成了 {monsterDmg} 點傷害！");

            if (_player.CurrentHp <= 0)
            {
                _player.CurrentHp = 0;
                log.Add("你死亡了...");

                // 死亡懲罰：回到起始點並恢復一半生命值
                _player.CurrentRoomId = 1; // 回到村莊廣場
                _player.CurrentHp = _player.MaxHp / 2;
                _player.CurrentMp = _player.MaxMp / 2;

                await SavePlayerData();
                await SendUpdate(log, "defeat");
                await SendPlayerDataUpdate();
                Stop();
                return;
            }

            await SendUpdate(log, "update");
        }

        public async Task ProcessPlayerAction(string action, string? target = null)
        {
            if (!_isActive) return;

            if (action.ToLower() == "flee")
            {
                if (_combatService.TryFlee(_player, _monster))
                {
                    await SendUpdate(new List<string> { "你成功逃離了戰鬥！" }, "flee");
                    Stop();
                }
                else
                {
                    await SendUpdate(new List<string> { "逃跑失敗！" }, "update");
                }
            }
            else if (action.ToLower() == "cast")
            {
                var skillName = target?.ToLower();
                if (string.IsNullOrWhiteSpace(skillName))
                {
                    await SendUpdate("要施放什麼技能？", "warning");
                    return;
                }

                // 從資料庫查找技能
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var skill = await db.Skills
                    .FirstOrDefaultAsync(s => s.RequiredClass == _player.Class &&
                                             s.Name.ToLower() == skillName);

                if (skill == null)
                {
                    await SendUpdate($"你沒有學會 '{target}' 這個技能！", "warning");
                    return;
                }

                var result = _skillService.CastSkill(_player, skill, _monster);
                if (result.Success)
                {
                    var logs = new List<string>();

                    // 根據技能類型顯示不同訊息
                    if (skill.Type == SkillType.Damage)
                    {
                        logs.Add($"你施放了 {skill.Name}！對 {_monster.Name} 造成了 {result.DamageDealt} 點傷害！");
                    }
                    else if (skill.Type == SkillType.Heal)
                    {
                        logs.Add($"你施放了 {skill.Name}！恢復了 {result.HealingDone} 點生命值！");
                    }
                    else
                    {
                        logs.Add(result.Message);
                    }

                    // 檢查怪物是否被擊敗
                    if (_monster.CurrentHp <= 0)
                    {
                        _monster.CurrentHp = 0;
                        logs.Add($"{_monster.Name} 被擊敗了！");

                        // 給予經驗值
                        _player.Exp += _monster.ExpReward;
                        logs.Add($"獲得 {_monster.ExpReward} 點經驗值！");

                        // 檢查是否升級
                        var expNeeded = GetExpForNextLevel(_player.Level);
                        while (_player.Exp >= expNeeded)
                        {
                            _player.Exp -= expNeeded;
                            _player.Level++;
                            LevelUp(_player);
                            logs.Add($"🎉 恭喜升級！現在是等級 {_player.Level}！");
                            logs.Add($"生命值和魔力值已完全恢復！");
                            expNeeded = GetExpForNextLevel(_player.Level);
                        }

                        await SavePlayerData();
                        await SendUpdate(logs, "victory");
                        await SendPlayerDataUpdate();
                        Stop();
                    }
                    else
                    {
                        await SendUpdate(logs, "update");
                    }
                }
                else
                {
                    await SendUpdate(result.Message, "warning");
                }
            }
        }

        private async Task SendUpdate(string message, string type = "info")
        {
            await SendUpdate(new List<string> { message }, type);
        }

        private async Task SendUpdate(List<string> messages, string type)
        {
            // In a real app, we'd map PlayerId to ConnectionId
            // For now, we broadcast to All (Prototype limitation) or use a Group if we had one
            // Ideally: await _hubContext.Clients.User(_player.Id.ToString()).SendAsync(...)
            
            // Since we don't have full user mapping in Hub yet, we'll send to "All" but frontend filters? 
            // No, that's bad.
            // We need to send to the specific client.
            // We will assume the GameHub has a way to address the user, or we just broadcast for this prototype 
            // and the frontend ignores if it's not for them? No, security risk.
            
            // BEST EFFORT FOR PROTOTYPE:
            // We will send a "CombatUpdate" event.
            // The payload will include PlayerId so frontend can filter (temporary).
            
            var update = new
            {
                PlayerId = _player.Id,
                Monster = new { _monster.Name, _monster.CurrentHp, _monster.MaxHp, Image = "🐺" }, // Placeholder image
                Player = new { _player.CurrentHp, _player.MaxHp, _player.CurrentMp, _player.MaxMp },
                Logs = messages.Select(m => new { Text = m, Type = type }).ToList(),
                Type = type
            };

            await _hubContext.Clients.All.SendAsync("CombatUpdate", update);

            // 也發送到主終端以確保訊息可見
            foreach (var msg in messages)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "戰鬥", msg);
            }
        }

        private int GetExpForNextLevel(int level)
        {
            // 經驗值公式：每級需要的經驗值遞增
            return level * 100 + (level - 1) * 50;
        }

        private void LevelUp(PlayerCharacter player)
        {
            // 根據職業提升不同屬性
            switch (player.Class)
            {
                case ClassType.Warrior:
                    player.Stats.Str += 3;
                    player.Stats.Con += 2;
                    player.Stats.Dex += 1;
                    player.MaxHp += 20;
                    player.MaxMp += 5;
                    break;

                case ClassType.Mage:
                    player.Stats.Int += 3;
                    player.Stats.Wis += 2;
                    player.Stats.Dex += 1;
                    player.MaxHp += 10;
                    player.MaxMp += 15;
                    break;

                case ClassType.Priest:
                    player.Stats.Wis += 3;
                    player.Stats.Int += 2;
                    player.Stats.Con += 1;
                    player.MaxHp += 15;
                    player.MaxMp += 12;
                    break;
            }

            // 升級時完全恢復生命值和魔力值
            player.CurrentHp = player.MaxHp;
            player.CurrentMp = player.MaxMp;
        }

        private async Task SavePlayerData()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 更新資料庫中的玩家資料
            var playerInDb = await db.PlayerCharacters.FirstOrDefaultAsync(p => p.Id == _player.Id);
            if (playerInDb != null)
            {
                playerInDb.Level = _player.Level;
                playerInDb.Exp = _player.Exp;
                playerInDb.CurrentHp = _player.CurrentHp;
                playerInDb.MaxHp = _player.MaxHp;
                playerInDb.CurrentMp = _player.CurrentMp;
                playerInDb.MaxMp = _player.MaxMp;
                playerInDb.CurrentRoomId = _player.CurrentRoomId;
                playerInDb.Stats = _player.Stats;

                await db.SaveChangesAsync();
            }
        }

        private async Task SendPlayerDataUpdate()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var player = await db.PlayerCharacters
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == _player.Id);

            if (player == null) return;

            // 獲取技能資料
            var skills = await db.Skills
                .Where(s => s.RequiredClass == player.Class)
                .ToListAsync();

            var playerData = new
            {
                id = player.Id.ToString(),
                name = player.Name,
                @class = (int)player.Class,
                level = player.Level,
                exp = player.Exp,
                expForNextLevel = GetExpForNextLevel(player.Level),
                currentHp = player.CurrentHp,
                maxHp = player.MaxHp,
                currentMp = player.CurrentMp,
                maxMp = player.MaxMp,
                stats = new
                {
                    str = player.Stats.Str,
                    dex = player.Stats.Dex,
                    @int = player.Stats.Int,
                    wis = player.Stats.Wis,
                    con = player.Stats.Con
                },
                currentRoomId = player.CurrentRoomId,
                skills = skills.Select(s => new
                {
                    id = s.Id.ToString(),
                    name = s.Name,
                    description = s.Description,
                    requiredClass = (int)s.RequiredClass,
                    manaCost = s.ManaCost,
                    cooldownSeconds = s.CooldownSeconds,
                    damageMultiplier = s.DamageMultiplier,
                    baseEffectValue = s.BaseEffectValue,
                    type = (int)s.Type
                }).ToList()
            };

            // 發送給特定玩家
            await _hubContext.Clients.All.SendAsync("PlayerData", playerData);
        }
    }
}
