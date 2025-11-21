using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredClass = table.Column<int>(type: "INTEGER", nullable: false),
                    ManaCost = table.Column<int>(type: "INTEGER", nullable: false),
                    CooldownSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    DamageMultiplier = table.Column<double>(type: "REAL", nullable: false),
                    BaseEffectValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "BaseEffectValue", "CooldownSeconds", "DamageMultiplier", "Description", "ManaCost", "Name", "RequiredClass", "Type" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), 0, 3, 1.5, "A powerful melee attack that deals 1.5x damage", 5, "Bash", 0, 0 },
                    { new Guid("00000000-0000-0000-0000-000000000002"), 0, 5, 2.0, "A devastating blow that deals 2.0x damage", 10, "Power Strike", 0, 0 },
                    { new Guid("00000000-0000-0000-0000-000000000003"), 0, 4, 2.0, "Launches a ball of fire dealing 2.0x magic damage", 15, "Fireball", 1, 0 },
                    { new Guid("00000000-0000-0000-0000-000000000004"), 0, 3, 1.8, "Shoots a shard of ice dealing 1.8x magic damage", 12, "Ice Bolt", 1, 0 },
                    { new Guid("00000000-0000-0000-0000-000000000005"), 50, 5, 0.0, "Restores health to yourself or an ally", 10, "Heal", 2, 1 },
                    { new Guid("00000000-0000-0000-0000-000000000006"), 100, 8, 0.0, "A powerful healing spell that restores significant health", 20, "Holy Light", 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
