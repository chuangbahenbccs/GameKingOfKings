using KingOfKings.Backend.Models;

namespace KingOfKings.Backend.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if DB has been seeded
            if (context.Rooms.Any())
            {
                return;   // DB has been seeded
            }

            // Seed Rooms
            // Seed Rooms (Newbie Village)
            var rooms = new WorldRoom[]
            {
                new WorldRoom 
                { 
                    Id = 1, 
                    Name = "Newbie Village Square", 
                    Description = "The center of a small, peaceful village. You see a fountain here. Paths lead to the Training Field (North) and the Elder's House (East).", 
                    ExitsJson = "{\"n\":2,\"e\":3}" 
                },
                new WorldRoom 
                { 
                    Id = 2, 
                    Name = "Training Field", 
                    Description = "A field with several wooden dummies. New adventurers practice here. The Village Square is to the South.", 
                    ExitsJson = "{\"s\":1}" 
                },
                new WorldRoom 
                { 
                    Id = 3, 
                    Name = "Village Elder's House", 
                    Description = "A cozy hut belonging to the village elder. You smell medicinal herbs. The Village Square is to the West.", 
                    ExitsJson = "{\"w\":1}" 
                }
            };

            context.Rooms.AddRange(rooms);

            // Seed Skills
            var skills = new Skill[]
            {
                // Warrior - 戰士技能
                new Skill { Id = Guid.NewGuid(), Name = "Bash", Description = "重擊 - 造成大量物理傷害", RequiredClass = ClassType.Warrior, ManaCost = 5, CooldownSeconds = 3, Type = SkillType.Damage, DamageMultiplier = 1.8, BaseEffectValue = 20 },
                new Skill { Id = Guid.NewGuid(), Name = "Taunt", Description = "嘲諷 - 激怒敵人", RequiredClass = ClassType.Warrior, ManaCost = 5, CooldownSeconds = 10, Type = SkillType.Debuff, DamageMultiplier = 0, BaseEffectValue = 0 },
                new Skill { Id = Guid.NewGuid(), Name = "Iron Skin", Description = "鐵皮 - 暫時增加防禦力", RequiredClass = ClassType.Warrior, ManaCost = 10, CooldownSeconds = 30, Type = SkillType.Buff, DamageMultiplier = 0, BaseEffectValue = 10 },

                // Mage - 法師技能
                new Skill { Id = Guid.NewGuid(), Name = "Fireball", Description = "火球術 - 投擲熾熱的火球", RequiredClass = ClassType.Mage, ManaCost = 10, CooldownSeconds = 0, Type = SkillType.Damage, DamageMultiplier = 2.5, BaseEffectValue = 25 },
                new Skill { Id = Guid.NewGuid(), Name = "Ice Storm", Description = "冰風暴 - 冰凍之雨造成範圍傷害", RequiredClass = ClassType.Mage, ManaCost = 20, CooldownSeconds = 10, Type = SkillType.Damage, DamageMultiplier = 1.8, BaseEffectValue = 40 },
                new Skill { Id = Guid.NewGuid(), Name = "Mana Shield", Description = "魔法護盾 - 使用魔力吸收傷害", RequiredClass = ClassType.Mage, ManaCost = 15, CooldownSeconds = 60, Type = SkillType.Buff, DamageMultiplier = 0, BaseEffectValue = 0 },

                // Priest - 牧師技能
                new Skill { Id = Guid.NewGuid(), Name = "Heal", Description = "治癒術 - 恢復生命值", RequiredClass = ClassType.Priest, ManaCost = 10, CooldownSeconds = 5, Type = SkillType.Heal, DamageMultiplier = 2.0, BaseEffectValue = 40 },
                new Skill { Id = Guid.NewGuid(), Name = "Bless", Description = "祝福 - 增加目標屬性", RequiredClass = ClassType.Priest, ManaCost = 15, CooldownSeconds = 60, Type = SkillType.Buff, DamageMultiplier = 0, BaseEffectValue = 5 },
                new Skill { Id = Guid.NewGuid(), Name = "Smite", Description = "神聖打擊 - 神聖傷害", RequiredClass = ClassType.Priest, ManaCost = 10, CooldownSeconds = 2, Type = SkillType.Damage, DamageMultiplier = 2.0, BaseEffectValue = 30 }
            };

            context.Skills.AddRange(skills);
            context.SaveChanges();
        }
    }
}
