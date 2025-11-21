using KingOfKings.Backend.Models;

namespace KingOfKings.Backend.Services
{
    /// <summary>
    /// Interface for combat-related operations.
    /// 戰鬥相關操作的介面。
    /// </summary>
    public interface ICombatService
    {
        /// <summary>
        /// Calculates damage dealt by an attacker to a defender.
        /// 計算攻擊者對防禦者造成的傷害。
        /// </summary>
        /// <param name="attacker">The attacker (attacking player) (攻擊者).</param>
        /// <param name="target">The target (monster) (目標怪物).</param>
        /// <returns>The amount of damage dealt (造成的傷害量).</returns>
        int CalculateDamage(PlayerCharacter attacker, Monster target);

        /// <summary>
        /// Processes a combat round between a player and a monster.
        /// 處理玩家與怪物之間的一回合戰鬥。
        /// </summary>
        /// <param name="player">The player (玩家).</param>
        /// <param name="monster">The monster (怪物).</param>
        /// <returns>A combat log message (戰鬥紀錄訊息).</returns>
        string ProcessCombatRound(PlayerCharacter player, Monster monster);
    }
}
