using KingOfKings.Backend.Hubs;
using KingOfKings.Backend.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace KingOfKings.Backend.Services
{
    public class CombatManager
    {
        private readonly ConcurrentDictionary<Guid, CombatSession> _activeSessions = new();
        private readonly IServiceProvider _serviceProvider;

        public CombatManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void StartCombat(PlayerCharacter player, Monster monster)
        {
            if (_activeSessions.ContainsKey(player.Id))
            {
                // Already in combat
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();
                var combatService = scope.ServiceProvider.GetRequiredService<ICombatService>();
                var skillService = scope.ServiceProvider.GetRequiredService<ISkillService>();

                var session = new CombatSession(player, monster, hubContext, combatService, skillService, OnCombatEnd, _serviceProvider);
                if (_activeSessions.TryAdd(player.Id, session))
                {
                    session.Start();
                }
            }
        }

        public CombatSession? GetSession(Guid playerId)
        {
            _activeSessions.TryGetValue(playerId, out var session);
            return session;
        }

        private void OnCombatEnd(Guid playerId)
        {
            _activeSessions.TryRemove(playerId, out _);
        }
    }
}
