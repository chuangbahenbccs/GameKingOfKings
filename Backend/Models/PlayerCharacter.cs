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

    [Required]
    public string Name { get; set; } = string.Empty;
    
    public ClassType Class { get; set; } = ClassType.Warrior;

    public int Level { get; set; } = 1;
    public long Exp { get; set; } = 0;

    public int MaxHp { get; set; } = 100;
    public int CurrentHp { get; set; } = 100;
    
    public int MaxMp { get; set; } = 50;
    public int CurrentMp { get; set; } = 50;

    public CharacterStats Stats { get; set; } = new();

    // Location
    public int CurrentRoomId { get; set; } = 1;
}
