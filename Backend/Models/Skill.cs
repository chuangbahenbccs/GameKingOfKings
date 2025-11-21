using System.ComponentModel.DataAnnotations;

namespace KingOfKings.Backend.Models
{
    public enum SkillType
    {
        Damage,
        Heal,
        Buff,
        Debuff
    }

    public class Skill
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ClassType RequiredClass { get; set; }
        public int ManaCost { get; set; }
        public int CooldownSeconds { get; set; }
        public double DamageMultiplier { get; set; } // For damage skills (e.g. 1.5x ATK)
        public int BaseEffectValue { get; set; } // For flat damage/heal
        public SkillType Type { get; set; }
    }
}
