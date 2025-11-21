using KingOfKings.Backend.Models;

namespace KingOfKings.Backend.Services
{
    public interface ISkillService
    {
        SkillResult CastSkill(PlayerCharacter player, Skill skill, Monster? target);
    }

    public class SkillResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int DamageDealt { get; set; }
        public int HealingDone { get; set; }
        public int ManaConsumed { get; set; }
    }

    public class SkillService : ISkillService
    {
        private readonly Random _random = new Random();

        public SkillResult CastSkill(PlayerCharacter player, Skill skill, Monster? target)
        {
            if (player.CurrentMp < skill.ManaCost)
            {
                return new SkillResult { Success = false, Message = $"魔力不足！需要 {skill.ManaCost} MP，你只有 {player.CurrentMp} MP。" };
            }

            // TODO: Check Cooldowns (Need to track CD state on Player)

            var result = new SkillResult
            {
                Success = true,
                ManaConsumed = skill.ManaCost,
                Message = $"你施放了 {skill.Name}！"
            };

            player.CurrentMp -= skill.ManaCost;

            switch (skill.Type)
            {
                case SkillType.Damage:
                    if (target == null)
                    {
                        result.Success = false;
                        result.Message = "沒有選擇目標！";
                        return result;
                    }
                    
                    // Calculate Damage
                    // Base damage based on class primary stat
                    // 技能傷害應該比普通攻擊更高，因為需要消耗魔力
                    int baseDmg = 0;
                    if (player.Class == ClassType.Warrior) baseDmg = player.Stats.Str * 3; // 戰士技能基於力量
                    else if (player.Class == ClassType.Mage) baseDmg = player.Stats.Int * 4; // 法師技能基於智力，傷害更高
                    else if (player.Class == ClassType.Priest) baseDmg = player.Stats.Wis * 3; // 牧師的神聖打擊基於智慧

                    int damage = (int)(baseDmg * skill.DamageMultiplier) + skill.BaseEffectValue;
                    
                    // Apply variance
                    damage = (int)(damage * (0.9 + _random.NextDouble() * 0.2));
                    
                    // Apply defense
                    damage -= target.Defense;
                    if (damage < 0) damage = 0;

                    target.CurrentHp -= damage;
                    result.DamageDealt = damage;
                    result.Message = $"你對 {target.Name} 施放了 {skill.Name}，造成 {damage} 點傷害！";
                    break;

                case SkillType.Heal:
                    // 治療技能也應該更強力，因為消耗魔力
                    int healAmount = (int)(player.Stats.Wis * 3 * skill.DamageMultiplier) + skill.BaseEffectValue;
                    // Apply variance
                    healAmount = (int)(healAmount * (0.9 + _random.NextDouble() * 0.2));

                    int oldHp = player.CurrentHp;
                    player.CurrentHp += healAmount;
                    if (player.CurrentHp > player.MaxHp) player.CurrentHp = player.MaxHp;
                    
                    result.HealingDone = player.CurrentHp - oldHp;
                    result.Message = $"你施放了 {skill.Name}，恢復了 {result.HealingDone} 點生命值！";
                    break;

                // TODO: Buff/Debuff
            }

            return result;
        }
    }
}
