namespace KingOfKings.Backend.Models;

/// <summary>
/// Represents an active monster instance in the game world.
/// 代表遊戲世界中活躍的怪物實例。
/// </summary>
public class MonsterSpawn
{
    public Guid Id { get; set; }

    /// <summary>
    /// Reference to the monster template.
    /// 怪物模板引用。
    /// </summary>
    public int MonsterTemplateId { get; set; }

    /// <summary>
    /// Monster template data.
    /// 怪物模板資料。
    /// </summary>
    public Monster? MonsterTemplate { get; set; }

    /// <summary>
    /// The room where this monster is spawned.
    /// 怪物所在的房間。
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Current HP of this spawn instance.
    /// 此實例的當前 HP。
    /// </summary>
    public int CurrentHp { get; set; }

    /// <summary>
    /// Whether this monster is currently in combat.
    /// 此怪物是否正在戰鬥中。
    /// </summary>
    public bool InCombat { get; set; }

    /// <summary>
    /// The player currently fighting this monster (if any).
    /// 正在與此怪物戰鬥的玩家（如有）。
    /// </summary>
    public Guid? EngagedPlayerId { get; set; }

    /// <summary>
    /// Time when the monster was spawned.
    /// 怪物生成時間。
    /// </summary>
    public DateTime SpawnedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Time when the monster was killed (for respawn calculation).
    /// 怪物被擊殺的時間（用於重生計算）。
    /// </summary>
    public DateTime? KilledAt { get; set; }

    /// <summary>
    /// Whether this monster is alive.
    /// 此怪物是否存活。
    /// </summary>
    public bool IsAlive => CurrentHp > 0;
}
