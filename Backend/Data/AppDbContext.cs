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
    public DbSet<Monster> Monsters { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<MonsterSpawn> MonsterSpawns { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<LootTableEntry> LootTableEntries { get; set; }
    public DbSet<ActiveCombat> ActiveCombats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Owned Type for Stats
        modelBuilder.Entity<PlayerCharacter>()
            .OwnsOne(p => p.Stats);

        // Seed initial data (Newbie Village)
        modelBuilder.Entity<WorldRoom>().HasData(
            new WorldRoom { Id = 1, Name = "Village Square", Description = "You are standing in the center of a peaceful village. A fountain bubbles softly nearby.", ExitsJson = "{\"n\":2,\"e\":3,\"s\":4}" },
            new WorldRoom { Id = 2, Name = "Training Grounds", Description = "Wooden dummies stand in rows here. The sound of striking wood fills the air.", ExitsJson = "{\"s\":1}" },
            new WorldRoom { Id = 3, Name = "Village Elder's House", Description = "A cozy hut with the smell of herbs.", ExitsJson = "{\"w\":1}" },
            new WorldRoom { Id = 4, Name = "Village Outskirts", Description = "The edge of the village. Grass sways in the breeze. You can see slimes wandering about.", ExitsJson = "{\"n\":1,\"s\":5}" },
            new WorldRoom { Id = 5, Name = "Slime Field", Description = "A field crawling with slimes. They seem mostly harmless, but they might attack if provoked.", ExitsJson = "{\"n\":4}" }
        );

        // Seed monster templates
        modelBuilder.Entity<Monster>().HasData(
            new Monster { Id = 1, Name = "Slime", MaxHp = 30, CurrentHp = 30, Attack = 5, Defense = 2, ExpReward = 10, LocationId = 4 },
            new Monster { Id = 2, Name = "Rat", MaxHp = 20, CurrentHp = 20, Attack = 8, Defense = 1, ExpReward = 8, LocationId = 4 },
            new Monster { Id = 3, Name = "King Slime", MaxHp = 150, CurrentHp = 150, Attack = 15, Defense = 5, ExpReward = 100, LocationId = 5 }
        );

        // Seed items
        modelBuilder.Entity<Item>().HasData(
            new Item { Id = 1, Name = "Slime Gel", Type = ItemType.Quest, PropertiesJson = "{}" },
            new Item { Id = 2, Name = "Rat Tail", Type = ItemType.Quest, PropertiesJson = "{}" },
            new Item { Id = 3, Name = "Novice Ring", Type = ItemType.Armor, PropertiesJson = "{\"Str\":1,\"Int\":1,\"Wis\":1,\"Dex\":1,\"Con\":1}" },
            new Item { Id = 4, Name = "Rusty Sword", Type = ItemType.Weapon, PropertiesJson = "{\"Atk\":5}" },
            new Item { Id = 5, Name = "Wooden Staff", Type = ItemType.Weapon, PropertiesJson = "{\"Atk\":3,\"Int\":2}" },
            new Item { Id = 6, Name = "Health Potion", Type = ItemType.Consumable, PropertiesJson = "{\"HealHp\":30}" },
            new Item { Id = 7, Name = "Mana Potion", Type = ItemType.Consumable, PropertiesJson = "{\"HealMp\":20}" }
        );

        // Seed skills
        modelBuilder.Entity<Skill>().HasData(
            // Warrior skills
            new Skill { Id = 1, SkillId = "bash", Name = "Bash", Description = "A powerful physical strike.", Type = SkillType.Physical, TargetType = SkillTargetType.SingleEnemy, MpCost = 5, Cooldown = 3, BasePower = 20, ScalingStat = "STR", ScalingMultiplier = 1.5, RequiredClass = ClassType.Warrior, RequiredLevel = 1 },
            new Skill { Id = 2, SkillId = "taunt", Name = "Taunt", Description = "Force the enemy to focus on you.", Type = SkillType.Physical, TargetType = SkillTargetType.SingleEnemy, MpCost = 3, Cooldown = 10, BasePower = 0, ScalingStat = "CON", ScalingMultiplier = 0, RequiredClass = ClassType.Warrior, RequiredLevel = 3 },
            new Skill { Id = 3, SkillId = "ironskin", Name = "Iron Skin", Description = "Temporarily increase defense.", Type = SkillType.Physical, TargetType = SkillTargetType.Self, MpCost = 10, Cooldown = 30, BasePower = 0, ScalingStat = "CON", ScalingMultiplier = 0.5, RequiredClass = ClassType.Warrior, RequiredLevel = 5 },
            // Mage skills
            new Skill { Id = 4, SkillId = "fireball", Name = "Fireball", Description = "Hurl a ball of fire at the enemy.", Type = SkillType.Magical, TargetType = SkillTargetType.SingleEnemy, MpCost = 10, Cooldown = 2, BasePower = 25, ScalingStat = "INT", ScalingMultiplier = 2.0, RequiredClass = ClassType.Mage, RequiredLevel = 1 },
            new Skill { Id = 5, SkillId = "icestorm", Name = "Ice Storm", Description = "Unleash a storm of ice on all enemies.", Type = SkillType.Magical, TargetType = SkillTargetType.AllEnemies, MpCost = 25, Cooldown = 10, BasePower = 15, ScalingStat = "INT", ScalingMultiplier = 1.5, RequiredClass = ClassType.Mage, RequiredLevel = 5 },
            new Skill { Id = 6, SkillId = "manashield", Name = "Mana Shield", Description = "Convert damage to MP cost.", Type = SkillType.Magical, TargetType = SkillTargetType.Self, MpCost = 20, Cooldown = 60, BasePower = 0, ScalingStat = "INT", ScalingMultiplier = 0, RequiredClass = ClassType.Mage, RequiredLevel = 3 },
            // Priest skills
            new Skill { Id = 7, SkillId = "heal", Name = "Heal", Description = "Restore HP to yourself.", Type = SkillType.Healing, TargetType = SkillTargetType.Self, MpCost = 8, Cooldown = 3, BasePower = 30, ScalingStat = "WIS", ScalingMultiplier = 2.0, RequiredClass = ClassType.Priest, RequiredLevel = 1 },
            new Skill { Id = 8, SkillId = "bless", Name = "Bless", Description = "Increase STR and DEX temporarily.", Type = SkillType.Healing, TargetType = SkillTargetType.Self, MpCost = 15, Cooldown = 30, BasePower = 0, ScalingStat = "WIS", ScalingMultiplier = 0.3, RequiredClass = ClassType.Priest, RequiredLevel = 3 },
            new Skill { Id = 9, SkillId = "smite", Name = "Smite", Description = "Holy damage that scales with WIS.", Type = SkillType.Magical, TargetType = SkillTargetType.SingleEnemy, MpCost = 12, Cooldown = 4, BasePower = 20, ScalingStat = "WIS", ScalingMultiplier = 1.8, RequiredClass = ClassType.Priest, RequiredLevel = 1 }
        );

        // Seed loot table
        modelBuilder.Entity<LootTableEntry>().HasData(
            new LootTableEntry { Id = 1, MonsterId = 1, ItemId = 1, DropRate = 50, MinQuantity = 1, MaxQuantity = 2 }, // Slime -> Slime Gel
            new LootTableEntry { Id = 2, MonsterId = 2, ItemId = 2, DropRate = 60, MinQuantity = 1, MaxQuantity = 1 }, // Rat -> Rat Tail
            new LootTableEntry { Id = 3, MonsterId = 3, ItemId = 3, DropRate = 100, MinQuantity = 1, MaxQuantity = 1 }, // King Slime -> Novice Ring
            new LootTableEntry { Id = 4, MonsterId = 1, ItemId = 6, DropRate = 10, MinQuantity = 1, MaxQuantity = 1 }, // Slime -> Health Potion
            new LootTableEntry { Id = 5, MonsterId = 2, ItemId = 6, DropRate = 15, MinQuantity = 1, MaxQuantity = 1 }  // Rat -> Health Potion
        );
    }
}
