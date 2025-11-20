namespace KingOfKings.Backend.DTOs;

public class GameStateDto
{
    public string Location { get; set; } = string.Empty;
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMp { get; set; }
    public int MaxMp { get; set; }
    public int Level { get; set; }
    public long Exp { get; set; }
}
