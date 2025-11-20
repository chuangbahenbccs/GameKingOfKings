using KingOfKings.Backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace KingOfKings.Backend.Hubs;

public class GameHub : Hub
{
    private readonly IGameEngine _gameEngine;

    public GameHub(IGameEngine gameEngine)
    {
        _gameEngine = gameEngine;
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
    }

    public async Task JoinGame(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Username cannot be empty.");
            return;
        }

        // Quick and dirty "Auth" for prototype: Find user/char or create
        // In production, this would be in AuthService and return a Token
        
        // We need a scope because Hub might be long lived or we want to be safe with DbContext
        // But Hub methods are invoked in a scope usually. 
        // Let's inject IServiceProvider to be safe or just rely on GameEngine if we move this logic there.
        // For now, let's just use a local scope here to access DB directly for "Login"
        
        // Note: We can't easily inject DbContext into Hub if we want to keep Hub lightweight, 
        // but it's fine for this scale. 
        // Better: Delegate to GameEngine or a new AuthService.
        // Let's delegate to GameEngine for "Login" helper for now to keep Hub clean.
        
        var playerId = await _gameEngine.LoginOrRegisterAsync(username);
        
        if (playerId == Guid.Empty)
        {
             await Clients.Caller.SendAsync("ReceiveMessage", "System", "Failed to join game.");
             return;
        }

        Context.Items["PlayerId"] = playerId;
        await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Welcome, {username}! You have joined the world.");
        
        // Look at current room
        var lookResult = await _gameEngine.ProcessCommandAsync(playerId, "look");
        await Clients.Caller.SendAsync("ReceiveMessage", "Game", lookResult);
    }
}
