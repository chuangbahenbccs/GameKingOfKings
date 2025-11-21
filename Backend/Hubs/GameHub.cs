using KingOfKings.Backend.Data;
using KingOfKings.Backend.Models;
using KingOfKings.Backend.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Hubs;

public class GameHub : Hub
{
    private readonly IGameEngine _gameEngine;
    private readonly IServiceProvider _serviceProvider;

    public GameHub(IGameEngine gameEngine, IServiceProvider serviceProvider)
    {
        _gameEngine = gameEngine;
        _serviceProvider = serviceProvider;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendCommand(string command)
    {
        // TODO: Get actual PlayerId from Context/Auth
        // For now, we'll assume a temporary ID or handle it in JoinGame
        // Let's just use a hardcoded ID for testing if Context is empty, 
        // but ideally we map ConnectionId to PlayerId.
        
        // Temporary: Create a random GUID for the session if not exists
        var playerId = Guid.Empty; // Replace with real lookup
        
        // We need a way to identify the player. 
        // For Phase 2 verification without full Auth, let's just pass the command to engine 
        // and let engine handle a "default" player if needed, or fail.
        // Actually, let's just echo for now if we can't find player, 
        // OR we can implement a simple "Join" that sets a Context item.
        
        if (Context.Items.TryGetValue("PlayerId", out var idObj) && idObj is Guid id)
        {
            playerId = id;
        }
        else
        {
             // Fallback for testing: try to find ANY player or create one?
             // Let's just return error
             await Clients.Caller.SendAsync("ReceiveMessage", "System", "You must join the game first.");
             return;
        }

        var result = await _gameEngine.ProcessCommandAsync(playerId, command);
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", result);
        
        // Send updated player data
        await SendPlayerData(playerId);
    }

    public async Task JoinGame(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Username cannot be empty.");
            return;
        }

        var playerId = await _gameEngine.LoginOrRegisterAsync(username);
        
        if (playerId == Guid.Empty)
        {
            // User exists but no character - need character creation
            await Clients.Caller.SendAsync("NeedCharacterCreation", username);
            return;
        }

        Context.Items["PlayerId"] = playerId;
        await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Welcome, {username}! You have joined the world.");
        
        // Send player data
        await SendPlayerData(playerId);
        
        // Look at current room
        var lookResult = await _gameEngine.ProcessCommandAsync(playerId, "look");
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", lookResult);
    }

    public async Task CreateCharacter(string username, int classType)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Username cannot be empty.");
            return;
        }

        if (classType < 0 || classType > 2)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Invalid class type.");
            return;
        }

        var playerId = await _gameEngine.CreateCharacterAsync(username, (ClassType)classType);
        
        if (playerId == Guid.Empty)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Failed to create character.");
            return;
        }

        Context.Items["PlayerId"] = playerId;
        await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Welcome, {username}! Your character has been created.");
        await Clients.Caller.SendAsync("CharacterCreated");
        
        // Send player data
        await SendPlayerData(playerId);
        
        // Look at current room
        var lookResult = await _gameEngine.ProcessCommandAsync(playerId, "look");
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", lookResult);
    }

    private async Task SendPlayerData(Guid playerId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var player = await db.PlayerCharacters
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == playerId);
            
        if (player == null) return;
        
        // Get skills for this class
        var skills = await db.Skills
            .Where(s => s.RequiredClass == player.Class)
            .ToListAsync();
        
        var playerData = new
        {
            name = player.Name,
            @class = (int)player.Class,
            level = player.Level,
            exp = player.Exp,
            currentHp = player.CurrentHp,
            maxHp = player.MaxHp,
            currentMp = player.CurrentMp,
            maxMp = player.MaxMp,
            stats = new
            {
                str = player.Stats.Str,
                dex = player.Stats.Dex,
                @int = player.Stats.Int,
                wis = player.Stats.Wis,
                con = player.Stats.Con
            },
            currentRoomId = player.CurrentRoomId,
            skills = skills.Select(s => new
            {
                id = s.Id.ToString(),
                name = s.Name,
                description = s.Description,
                requiredClass = (int)s.RequiredClass,
                manaCost = s.ManaCost,
                cooldownSeconds = s.CooldownSeconds,
                damageMultiplier = s.DamageMultiplier,
                baseEffectValue = s.BaseEffectValue,
                type = (int)s.Type
            }).ToList()
        };
        
        await Clients.Caller.SendAsync("PlayerData", playerData);
    }
}
