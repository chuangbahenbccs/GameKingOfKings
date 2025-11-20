using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Services;

public class GameEngine : IGameEngine
{
    private readonly IServiceProvider _serviceProvider;
    
    public GameEngine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<string> ProcessCommandAsync(Guid playerId, string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return string.Empty;

        var parts = command.Trim().Split(' ');
        var action = parts[0].ToLower();
        var args = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var player = await db.PlayerCharacters
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId);

        if (player == null) return "Error: Player not found.";

        switch (action)
        {
            case "look":
            case "l":
                return await HandleLook(db, player);
            case "move":
            case "go":
            case "n":
            case "s":
            case "e":
            case "w":
            case "north":
            case "south":
            case "east":
            case "west":
                return await HandleMove(db, player, action, args);
            case "say":
                return $"You say: \"{args}\"";
            default:
                return "Unknown command.";
        }
    }

    private async Task<string> HandleLook(AppDbContext db, PlayerCharacter player)
    {
        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "You are in the void.";

        return $"<div class='text-yellow-400 font-bold'>{room.Name}</div><div>{room.Description}</div>";
    }

    private async Task<string> HandleMove(AppDbContext db, PlayerCharacter player, string action, string args)
    {
        // Normalize direction
        string direction = action;
        if (action == "move" || action == "go") direction = args;
        
        direction = direction.ToLower().Substring(0, 1); // n, s, e, w

        var room = await db.Rooms.FindAsync(player.CurrentRoomId);
        if (room == null) return "You are stuck.";

        // Simple JSON parsing for exits (in real app use System.Text.Json)
        // ExitsJson: {"n":2,"e":3}
        var exits = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(room.ExitsJson);
        
        if (exits != null && exits.ContainsKey(direction))
        {
            player.CurrentRoomId = exits[direction];
            await db.SaveChangesAsync();
            return await HandleLook(db, player);
        }

        return "You cannot go that way.";
    }

    public async Task<Guid> LoginOrRegisterAsync(string username)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            user = new User 
            { 
                Id = Guid.NewGuid(), 
                Username = username, 
                PasswordHash = "dummy" // No real auth yet
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        var player = await db.PlayerCharacters.FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (player == null)
        {
            player = new PlayerCharacter
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = username,
                Class = ClassType.Warrior,
                CurrentRoomId = 1,
                Stats = new CharacterStats { Str = 10, Dex = 10, Int = 10, Wis = 10, Con = 10 }
            };
            db.PlayerCharacters.Add(player);
            await db.SaveChangesAsync();
        }

        return player.Id;
    }

    public async Task TickAsync()
    {
        // TODO: Implement regen, combat ticks
        await Task.CompletedTask;
    }
}
