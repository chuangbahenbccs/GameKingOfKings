namespace KingOfKings.Backend.Models;

/// <summary>
/// Skill type enumeration.
/// 技能類型枚舉。
/// </summary>
public enum SkillType
{
    Physical,   // 物理技能
    Magical,    // 魔法技能
    Healing     // 治療技能
}

/// <summary>
/// Skill target type.
/// 技能目標類型。
/// </summary>
public enum SkillTargetType
{
    Self,       // 自身
    SingleEnemy,// 單體敵人
    AllEnemies, // 全體敵人
    SingleAlly, // 單體友軍
    AllAllies   // 全體友軍
}

/// <summary>
/// Represents a skill that can be used by a player character.
/// 代表玩家角色可以使用的技能。
/// </summary>
public class Skill
{
    public int Id { get; set; }

    /// <summary>
    /// Internal skill identifier (e.g., "bash", "fireball", "heal").
    /// 內部技能識別碼。
    /// </summary>
    public string SkillId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the skill.
    /// 技能顯示名稱。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the skill.
    /// 技能說明。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Skill type (Physical, Magical, Healing).
    /// 技能類型。
    /// </summary>
    public SkillType Type { get; set; }

    /// <summary>
    /// Target type of the skill.
    /// 技能目標類型。
    /// </summary>
    public SkillTargetType TargetType { get; set; }

    /// <summary>
    /// MP cost to use the skill.
    /// 使用技能的 MP 消耗。
    /// </summary>
    public int MpCost { get; set; }

    /// <summary>
    /// Cooldown in seconds.
    /// 冷卻時間（秒）。
    /// </summary>
    public int Cooldown { get; set; }

    /// <summary>
    /// Base damage/heal amount.
    /// 基礎傷害/治療量。
    /// </summary>
    public int BasePower { get; set; }

    /// <summary>
    /// Stat scaling factor (e.g., "STR", "INT", "WIS").
    /// 屬性倍率參考。
    /// </summary>
    public string ScalingStat { get; set; } = "STR";

    /// <summary>
    /// Scaling multiplier (e.g., 1.5 means 150% of stat).
    /// 屬性倍率。
    /// </summary>
    public double ScalingMultiplier { get; set; } = 1.0;

    /// <summary>
    /// Required class to use this skill (null = all classes).
    /// 需要的職業（null 表示所有職業可用）。
    /// </summary>
    public ClassType? RequiredClass { get; set; }

    /// <summary>
    /// Required level to learn this skill.
    /// 學習此技能需要的等級。
    /// </summary>
    public int RequiredLevel { get; set; } = 1;
}
