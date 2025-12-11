namespace KingOfKings.Backend.Models;

/// <summary>
/// Equipment slot types.
/// 裝備欄位類型。
/// </summary>
public enum EquipmentSlot
{
    None,       // 非裝備
    Weapon,     // 武器
    Head,       // 頭部
    Body,       // 身體
    Hands,      // 手部
    Feet,       // 腳部
    Accessory   // 飾品
}

/// <summary>
/// Represents an item in a player's inventory.
/// 代表玩家背包中的物品。
/// </summary>
public class InventoryItem
{
    public Guid Id { get; set; }

    /// <summary>
    /// The player who owns this item.
    /// 擁有此物品的玩家。
    /// </summary>
    public Guid PlayerId { get; set; }
    public PlayerCharacter? Player { get; set; }

    /// <summary>
    /// Reference to the item template.
    /// 物品模板引用。
    /// </summary>
    public int ItemId { get; set; }
    public Item? Item { get; set; }

    /// <summary>
    /// Quantity of this item.
    /// 物品數量。
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Whether this item is currently equipped.
    /// 此物品是否已裝備。
    /// </summary>
    public bool IsEquipped { get; set; }

    /// <summary>
    /// The equipment slot if equipped.
    /// 裝備的欄位。
    /// </summary>
    public EquipmentSlot EquippedSlot { get; set; } = EquipmentSlot.None;

    /// <summary>
    /// Slot index in the inventory (0-24 for 5x5 grid).
    /// 背包中的位置索引。
    /// </summary>
    public int SlotIndex { get; set; }
}
