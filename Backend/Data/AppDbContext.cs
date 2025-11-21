using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
    public DbSet<WorldRoom> Rooms { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Monster> Monsters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Owned Type for Stats
        modelBuilder.Entity<PlayerCharacter>()
            .OwnsOne(p => p.Stats);

        // Seed initial data (Newbie Village) - 中文化
        modelBuilder.Entity<WorldRoom>().HasData(
            new WorldRoom { Id = 1, Name = "村莊廣場", Description = "你站在一個和平村莊的中心。附近有一座噴泉正輕柔地冒著水泡。北邊通往訓練場，東邊是村長的家。", ExitsJson = "{\"n\":2,\"e\":3}" },
            new WorldRoom { Id = 2, Name = "訓練場", Description = "木頭人偶排成一列列站在這裡。空氣中充滿了敲擊木頭的聲音。南邊是村莊廣場。", ExitsJson = "{\"s\":1}" },
            new WorldRoom { Id = 3, Name = "村長的家", Description = "一間舒適的小屋，空氣中瀰漫著草藥的香味。西邊可以回到村莊廣場。", ExitsJson = "{\"w\":1}" }
        );

        // Seed initial monsters - 初始怪物資料
        modelBuilder.Entity<Monster>().HasData(
            // 訓練場的怪物
            new Monster { Id = 1, Name = "木頭人偶", CurrentHp = 50, MaxHp = 50, Attack = 5, Defense = 2, ExpReward = 10, LocationId = 2 },
            new Monster { Id = 2, Name = "訓練假人", CurrentHp = 30, MaxHp = 30, Attack = 3, Defense = 1, ExpReward = 5, LocationId = 2 },

            // 村莊廣場附近的怪物
            new Monster { Id = 3, Name = "野狼", CurrentHp = 80, MaxHp = 80, Attack = 12, Defense = 5, ExpReward = 25, LocationId = 1 },
            new Monster { Id = 4, Name = "wolf", CurrentHp = 80, MaxHp = 80, Attack = 12, Defense = 5, ExpReward = 25, LocationId = 1 },

            // 村長家附近的怪物
            new Monster { Id = 5, Name = "大老鼠", CurrentHp = 40, MaxHp = 40, Attack = 8, Defense = 3, ExpReward = 15, LocationId = 3 }
        );

        // Seed initial skills
        modelBuilder.Entity<Skill>().HasData(
            // Warrior Skills
            new Skill 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Bash",
                Description = "A powerful melee attack that deals 1.5x damage",
                RequiredClass = ClassType.Warrior,
                ManaCost = 5,
                CooldownSeconds = 3,
                DamageMultiplier = 1.5,
                BaseEffectValue = 0,
                Type = SkillType.Damage
            },
            new Skill 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Power Strike",
                Description = "A devastating blow that deals 2.0x damage",
                RequiredClass = ClassType.Warrior,
                ManaCost = 10,
                CooldownSeconds = 5,
                DamageMultiplier = 2.0,
                BaseEffectValue = 0,
                Type = SkillType.Damage
            },
            // Mage Skills
            new Skill 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Fireball",
                Description = "Launches a ball of fire dealing 2.0x magic damage",
                RequiredClass = ClassType.Mage,
                ManaCost = 15,
                CooldownSeconds = 4,
                DamageMultiplier = 2.0,
                BaseEffectValue = 0,
                Type = SkillType.Damage
            },
            new Skill 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "Ice Bolt",
                Description = "Shoots a shard of ice dealing 1.8x magic damage",
                RequiredClass = ClassType.Mage,
                ManaCost = 12,
                CooldownSeconds = 3,
                DamageMultiplier = 1.8,
                BaseEffectValue = 0,
                Type = SkillType.Damage
            },
            // Priest Skills
            new Skill 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Name = "Heal",
                Description = "Restores health to yourself or an ally",
                RequiredClass = ClassType.Priest,
                ManaCost = 10,
                CooldownSeconds = 5,
                DamageMultiplier = 0,
                BaseEffectValue = 50,
                Type = SkillType.Heal
            },
            new Skill 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                Name = "Holy Light",
                Description = "A powerful healing spell that restores significant health",
                RequiredClass = ClassType.Priest,
                ManaCost = 20,
                CooldownSeconds = 8,
                DamageMultiplier = 0,
                BaseEffectValue = 100,
                Type = SkillType.Heal
            }
        );
    }
}
