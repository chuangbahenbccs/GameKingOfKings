using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Models;

public enum ClassType
{
    Warrior,
    Mage,
    Priest
}

[Owned]
public class CharacterStats
{
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Wis { get; set; }
    public int Con { get; set; }
}

public class PlayerCharacter
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// The name of the character.
    /// 角色名稱。
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The class/job of the character.
    /// 角色職業。
    /// </summary>
    public ClassType Class { get; set; } = ClassType.Warrior;

    /// <summary>
    /// Current level.
    /// 當前等級。
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Current experience points.
    /// 當前經驗值。
    /// </summary>
    public long Exp { get; set; } = 0;

    /// <summary>
    /// Maximum Health Points.
    /// 最大生命值。
    /// </summary>
    public int MaxHp { get; set; } = 100;

    /// <summary>
    /// Current Health Points.
    /// 當前生命值。
    /// </summary>
    public int CurrentHp { get; set; } = 100;
    
    /// <summary>
    /// Maximum Mana Points.
    /// 最大魔力值。
    /// </summary>
    public int MaxMp { get; set; } = 50;

    /// <summary>
    /// Current Mana Points.
    /// 當前魔力值。
    /// </summary>
    public int CurrentMp { get; set; } = 50;

    /// <summary>
    /// Character statistics (Str, Dex, etc.).
    /// 角色屬性 (力量, 敏捷 等)。
    /// </summary>
    public CharacterStats Stats { get; set; } = new();

    /// <summary>
    /// ID of the current room/location.
    /// 當前所在房間/地點 ID。
    /// </summary>
    // Location
    public int CurrentRoomId { get; set; } = 1;
}
