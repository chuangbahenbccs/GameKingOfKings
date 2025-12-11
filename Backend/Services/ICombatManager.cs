using KingOfKings.Backend.Models;

namespace KingOfKings.Backend.Services;

/// <summary>
/// Interface for combat management service.
/// 戰鬥管理服務介面。
/// </summary>
public interface ICombatManager
{
    /// <summary>
    /// Start combat with a monster.
    /// 開始與怪物戰鬥。
    /// </summary>
    Task<string> StartCombatAsync(Guid playerId, string targetName);

    /// <summary>
    /// Flee from current combat.
    /// 逃離當前戰鬥。
    /// </summary>
    Task<string> FleeAsync(Guid playerId);

    /// <summary>
    /// Use a skill in combat.
    /// 在戰鬥中使用技能。
    /// </summary>
    Task<string> UseSkillAsync(Guid playerId, string skillId);

    /// <summary>
    /// Process a combat tick for a specific combat.
    /// 處理特定戰鬥的一個回合。
    /// </summary>
    Task<CombatTickResult> ProcessCombatTickAsync(Guid combatId);

    /// <summary>
    /// Get all active combats for processing.
    /// 獲取所有活躍的戰鬥。
    /// </summary>
    Task<List<ActiveCombat>> GetActiveCombatsAsync();

    /// <summary>
    /// Check if player is in combat.
    /// 檢查玩家是否在戰鬥中。
    /// </summary>
    Task<bool> IsInCombatAsync(Guid playerId);

    /// <summary>
    /// Get player's current combat.
    /// 獲取玩家當前的戰鬥。
    /// </summary>
    Task<ActiveCombat?> GetPlayerCombatAsync(Guid playerId);
}

/// <summary>
/// Result of a combat tick.
/// 戰鬥回合結果。
/// </summary>
public class CombatTickResult
{
    public Guid PlayerId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool CombatEnded { get; set; }
    public bool PlayerDied { get; set; }
    public bool MonsterDied { get; set; }
    public int ExpGained { get; set; }
    public List<LootDrop> Loot { get; set; } = new();
    public int? PlayerCurrentHp { get; set; }
    public int? PlayerMaxHp { get; set; }
    public int? MonsterCurrentHp { get; set; }
    public int? MonsterMaxHp { get; set; }
}

/// <summary>
/// Represents a dropped item.
/// 代表掉落的物品。
/// </summary>
public class LootDrop
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
