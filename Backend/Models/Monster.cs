using System.ComponentModel.DataAnnotations;

namespace KingOfKings.Backend.Models
{
    /// <summary>
    /// Represents a monster or enemy in the game.
    /// 代表遊戲中的怪物或敵人。
    /// </summary>
    public class Monster
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the monster.
        /// 怪物的名稱。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Current Health Points.
        /// 當前生命值。
        /// </summary>
        public int CurrentHp { get; set; }

        /// <summary>
        /// Maximum Health Points.
        /// 最大生命值。
        /// </summary>
        public int MaxHp { get; set; }

        /// <summary>
        /// Attack power.
        /// 攻擊力。
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Defense power.
        /// 防禦力。
        /// </summary>
        public int Defense { get; set; }

        /// <summary>
        /// Experience points awarded when killed.
        /// 被擊殺時給予的經驗值。
        /// </summary>
        public int ExpReward { get; set; }

        /// <summary>
        /// The ID of the room where the monster is located.
        /// 怪物所在房間的 ID。
        /// </summary>
        public int LocationId { get; set; }
    }
}
