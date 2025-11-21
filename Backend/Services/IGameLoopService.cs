namespace KingOfKings.Backend.Services
{
    /// <summary>
    /// Interface for the main game loop service.
    /// 遊戲主迴圈服務的介面。
    /// </summary>
    public interface IGameLoopService
    {
        /// <summary>
        /// Starts the game loop.
        /// 啟動遊戲迴圈。
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken);
    }
}
