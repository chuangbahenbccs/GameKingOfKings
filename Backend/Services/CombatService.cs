using KingOfKings.Backend.Models;

namespace KingOfKings.Backend.Services
{
    /// <summary>
    /// Implementation of the combat service.
    /// 戰鬥服務的實作。
    /// </summary>
    public class CombatService : ICombatService
    {
        private readonly Random _random = new Random();

        public int CalculateDamage(PlayerCharacter attacker, Monster target)
        {
            // Basic formula: (Atk - Def) * Random(0.9, 1.1)
            // 基礎公式：(攻擊力 - 防禦力) * 隨機(0.9, 1.1)
            
            // Assuming STR is the main stat for physical attack for now
            // 暫時假設力量 (STR) 是物理攻擊的主要屬性
            int attackPower = attacker.Stats.Str * 2; 
            int damage = attackPower - target.Defense;

            if (damage < 0) damage = 0;

            double multiplier = 0.9 + (_random.NextDouble() * 0.2); // 0.9 to 1.1
            return (int)(damage * multiplier);
        }

        public string ProcessCombatRound(PlayerCharacter player, Monster monster)
        {
            // Player attacks Monster
            // 玩家攻擊怪物
            int playerDmg = CalculateDamage(player, monster);
            monster.CurrentHp -= playerDmg;
            string log = $"You hit {monster.Name} for {playerDmg} damage!";

            if (monster.CurrentHp <= 0)
            {
                monster.CurrentHp = 0;
                log += $" {monster.Name} has been slain!";
                return log;
            }

            // Monster attacks Player
            // 怪物攻擊玩家
            // Simplified monster damage for now
            int monsterDmg = Math.Max(0, monster.Attack - (player.Stats.Con / 2)); 
            player.CurrentHp -= monsterDmg;
            log += $" {monster.Name} hits you for {monsterDmg} damage!";

            if (player.CurrentHp <= 0)
            {
                player.CurrentHp = 0;
                log += " You have died...";
            }

            return log;
        }
    }
}
