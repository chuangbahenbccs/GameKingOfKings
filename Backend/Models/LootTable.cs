namespace KingOfKings.Backend.Models;

/// <summary>
/// Represents a loot table entry for monsters.
/// 代表怪物的掉落表條目。
/// </summary>
public class LootTableEntry
{
    public int Id { get; set; }

    /// <summary>
    /// The monster template this loot belongs to.
    /// 此掉落所屬的怪物模板。
    /// </summary>
    public int MonsterId { get; set; }
    public Monster? Monster { get; set; }

    /// <summary>
    /// The item that can be dropped.
    /// 可以掉落的物品。
    /// </summary>
    public int ItemId { get; set; }
    public Item? Item { get; set; }

    /// <summary>
    /// Drop rate as percentage (0-100).
    /// 掉落機率（百分比，0-100）。
    /// </summary>
    public double DropRate { get; set; }

    /// <summary>
    /// Minimum quantity that can drop.
    /// 最小掉落數量。
    /// </summary>
    public int MinQuantity { get; set; } = 1;

    /// <summary>
    /// Maximum quantity that can drop.
    /// 最大掉落數量。
    /// </summary>
    public int MaxQuantity { get; set; } = 1;
}
