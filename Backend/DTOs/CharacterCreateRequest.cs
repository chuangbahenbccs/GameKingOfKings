using System.ComponentModel.DataAnnotations;
using KingOfKings.Backend.Models;

namespace KingOfKings.Backend.DTOs;

public class CharacterCreateRequest
{
    [Required]
    [MinLength(3), MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public ClassType Class { get; set; }
}
