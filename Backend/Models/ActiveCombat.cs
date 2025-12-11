namespace KingOfKings.Backend.Models;

/// <summary>
/// Represents an active combat session between a player and monster.
/// 代表玩家與怪物之間的活躍戰鬥會話。
/// </summary>
public class ActiveCombat
{
    public Guid Id { get; set; }

    /// <summary>
    /// The player in combat.
    /// 戰鬥中的玩家。
    /// </summary>
    public Guid PlayerId { get; set; }
    public PlayerCharacter? Player { get; set; }

    /// <summary>
    /// The monster spawn being fought.
    /// 正在戰鬥的怪物。
    /// </summary>
    public Guid MonsterSpawnId { get; set; }
    public MonsterSpawn? MonsterSpawn { get; set; }

    /// <summary>
    /// When the combat started.
    /// 戰鬥開始時間。
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last time a combat tick occurred.
    /// 上次戰鬥回合時間。
    /// </summary>
    public DateTime LastTick { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Next combat tick time.
    /// 下次戰鬥回合時間。
    /// </summary>
    public DateTime NextTick { get; set; }

    /// <summary>
    /// Whether the player is currently resting (cannot attack).
    /// 玩家是否正在休息中。
    /// </summary>
    public bool IsResting { get; set; }

    /// <summary>
    /// Queued skill to use on next attack.
    /// 下次攻擊時使用的技能佇列。
    /// </summary>
    public string? QueuedSkillId { get; set; }
}
