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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Owned Type for Stats
        modelBuilder.Entity<PlayerCharacter>()
            .OwnsOne(p => p.Stats);

        // Seed initial data (Newbie Village)
        modelBuilder.Entity<WorldRoom>().HasData(
            new WorldRoom { Id = 1, Name = "Village Square", Description = "You are standing in the center of a peaceful village. A fountain bubbles softly nearby.", ExitsJson = "{\"n\":2,\"e\":3}" },
            new WorldRoom { Id = 2, Name = "Training Grounds", Description = "Wooden dummies stand in rows here. The sound of striking wood fills the air.", ExitsJson = "{\"s\":1}" },
            new WorldRoom { Id = 3, Name = "Village Elder's House", Description = "A cozy hut with the smell of herbs.", ExitsJson = "{\"w\":1}" }
        );
    }
}
