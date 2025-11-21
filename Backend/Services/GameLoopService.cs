using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KingOfKings.Backend.Services
{
    /// <summary>
    /// Background service that runs the game loop (ticks).
    /// 執行遊戲迴圈 (ticks) 的背景服務。
    /// </summary>
    public class GameLoopService : BackgroundService, IGameLoopService
    {
        private readonly ILogger<GameLoopService> _logger;
        private readonly TimeSpan _tickRate = TimeSpan.FromSeconds(1); // 1 tick per second

        public GameLoopService(ILogger<GameLoopService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Game Loop Service is starting.");
            // 遊戲迴圈服務正在啟動。

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Perform game tick logic here (e.g., regen HP/MP, respawn monsters)
                    // 在此執行遊戲 tick 邏輯 (例如：回復 HP/MP，重生怪物)
                    
                    // _logger.LogInformation("Game Tick...");

                    await Task.Delay(_tickRate, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Game Loop");
                }
            }

            _logger.LogInformation("Game Loop Service is stopping.");
        }
    }
}
