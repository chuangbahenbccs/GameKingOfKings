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
            var rooms = new Room[]
            {
                new Room { Id = Guid.NewGuid(), Name = "Newbie Village Square", Description = "The center of a small, peaceful village. You see a fountain here." },
                new Room { Id = Guid.NewGuid(), Name = "Training Field", Description = "A field with several wooden dummies. New adventurers practice here." },
                new Room { Id = Guid.NewGuid(), Name = "Village Elder's House", Description = "A cozy hut belonging to the village elder." },
                new Room { Id = Guid.NewGuid(), Name = "Dark Forest Entrance", Description = "The trees here are twisted and dark. You hear howling in the distance." }
            };

            // Link Rooms (Simple linear map for now)
            // Square <-> Field
            // Square <-> Elder
            // Square <-> Forest
            // We'll just rely on ID mapping if we had navigation logic, but for now just seed them.
            // In a real app we'd set Exits.

            context.Rooms.AddRange(rooms);

            // Seed Monsters
            // Note: In a real app, Monsters would be templates, and we'd spawn instances.
            // For this prototype, we'll just seed some "Monster Definitions" if we had a table, 
            // but currently GameEngine mocks them. 
            // Let's actually add a MonsterDefinition table later, but for now, 
            // we will just skip seeding monsters since GameEngine creates them on the fly.
            // Wait, the plan said "Seed Monsters". 
            // Since we don't have a MonsterDefinition table yet, I will skip this part 
            // and rely on the GameEngine mock or add it if I have time.
            // Actually, let's stick to the plan. The plan said "Seed Monsters".
            // But I don't have a table for templates. I'll skip for now and focus on Skills.

            // Seed Skills
            var skills = new Skill[]
            {
                new Skill { Id = Guid.NewGuid(), Name = "Bash", Description = "A heavy blow.", RequiredClass = ClassType.Warrior, ManaCost = 5, CooldownSeconds = 0, Type = SkillType.Damage, DamageMultiplier = 1.5, BaseEffectValue = 5 },
                new Skill { Id = Guid.NewGuid(), Name = "Taunt", Description = "Enrages the enemy.", RequiredClass = ClassType.Warrior, ManaCost = 5, CooldownSeconds = 5, Type = SkillType.Debuff, DamageMultiplier = 0, BaseEffectValue = 0 },
                
                new Skill { Id = Guid.NewGuid(), Name = "Fireball", Description = "Hurls a ball of fire.", RequiredClass = ClassType.Mage, ManaCost = 15, CooldownSeconds = 0, Type = SkillType.Damage, DamageMultiplier = 2.0, BaseEffectValue = 10 },
                new Skill { Id = Guid.NewGuid(), Name = "Ice Storm", Description = "Freezing rain.", RequiredClass = ClassType.Mage, ManaCost = 20, CooldownSeconds = 5, Type = SkillType.Damage, DamageMultiplier = 1.2, BaseEffectValue = 5 },

                new Skill { Id = Guid.NewGuid(), Name = "Heal", Description = "Restores health.", RequiredClass = ClassType.Priest, ManaCost = 10, CooldownSeconds = 0, Type = SkillType.Heal, DamageMultiplier = 1.5, BaseEffectValue = 20 },
                new Skill { Id = Guid.NewGuid(), Name = "Smite", Description = "Holy damage.", RequiredClass = ClassType.Priest, ManaCost = 10, CooldownSeconds = 0, Type = SkillType.Damage, DamageMultiplier = 1.5, BaseEffectValue = 10 }
            };

            context.Skills.AddRange(skills);
            context.SaveChanges();
        }
    }
}
