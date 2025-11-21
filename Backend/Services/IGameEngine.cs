using KingOfKings.Backend.Models;

namespace KingOfKings.Backend.Services;

public interface IGameEngine
{
    Task<string> ProcessCommandAsync(Guid playerId, string command);
    Task<Guid> LoginOrRegisterAsync(string username);
    Task<bool> CheckUserHasCharacter(string username);
    Task<Guid> CreateCharacterAsync(string username, ClassType classType);
    Task TickAsync();
}
