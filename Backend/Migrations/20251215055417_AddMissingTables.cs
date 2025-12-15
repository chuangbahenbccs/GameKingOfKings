using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEquipped = table.Column<bool>(type: "INTEGER", nullable: false),
                    EquippedSlot = table.Column<int>(type: "INTEGER", nullable: false),
                    SlotIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItems_PlayerCharacters_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "PlayerCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentHp = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxHp = table.Column<int>(type: "INTEGER", nullable: false),
                    Attack = table.Column<int>(type: "INTEGER", nullable: false),
                    Defense = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpReward = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkillId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetType = table.Column<int>(type: "INTEGER", nullable: false),
                    MpCost = table.Column<int>(type: "INTEGER", nullable: false),
                    Cooldown = table.Column<int>(type: "INTEGER", nullable: false),
                    BasePower = table.Column<int>(type: "INTEGER", nullable: false),
                    ScalingStat = table.Column<string>(type: "TEXT", nullable: false),
                    ScalingMultiplier = table.Column<double>(type: "REAL", nullable: false),
                    RequiredClass = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiredLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LootTableEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MonsterId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    DropRate = table.Column<double>(type: "REAL", nullable: false),
                    MinQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxQuantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LootTableEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LootTableEntries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LootTableEntries_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterSpawns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MonsterTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentHp = table.Column<int>(type: "INTEGER", nullable: false),
                    InCombat = table.Column<bool>(type: "INTEGER", nullable: false),
                    EngagedPlayerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SpawnedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KilledAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpawns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterSpawns_Monsters_MonsterTemplateId",
                        column: x => x.MonsterTemplateId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveCombats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlayerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MonsterSpawnId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastTick = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextTick = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsResting = table.Column<bool>(type: "INTEGER", nullable: false),
                    QueuedSkillId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveCombats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveCombats_MonsterSpawns_MonsterSpawnId",
                        column: x => x.MonsterSpawnId,
                        principalTable: "MonsterSpawns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveCombats_PlayerCharacters_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "PlayerCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Name", "PropertiesJson", "Type" },
                values: new object[,]
                {
                    { 1, "Slime Gel", "{}", 3 },
                    { 2, "Rat Tail", "{}", 3 },
                    { 3, "Novice Ring", "{\"Str\":1,\"Int\":1,\"Wis\":1,\"Dex\":1,\"Con\":1}", 1 },
                    { 4, "Rusty Sword", "{\"Atk\":5}", 0 },
                    { 5, "Wooden Staff", "{\"Atk\":3,\"Int\":2}", 0 },
                    { 6, "Health Potion", "{\"HealHp\":30}", 2 },
                    { 7, "Mana Potion", "{\"HealMp\":20}", 2 }
                });

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "Id", "Attack", "CurrentHp", "Defense", "ExpReward", "LocationId", "MaxHp", "Name" },
                values: new object[,]
                {
                    { 1, 5, 30, 2, 10, 4, 30, "Slime" },
                    { 2, 8, 20, 1, 8, 4, 20, "Rat" },
                    { 3, 15, 150, 5, 100, 5, 150, "King Slime" }
                });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "ExitsJson",
                value: "{\"n\":2,\"e\":3,\"s\":4}");

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Description", "ExitsJson", "Name" },
                values: new object[,]
                {
                    { 4, "The edge of the village. Grass sways in the breeze. You can see slimes wandering about.", "{\"n\":1,\"s\":5}", "Village Outskirts" },
                    { 5, "A field crawling with slimes. They seem mostly harmless, but they might attack if provoked.", "{\"n\":4}", "Slime Field" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "BasePower", "Cooldown", "Description", "MpCost", "Name", "RequiredClass", "RequiredLevel", "ScalingMultiplier", "ScalingStat", "SkillId", "TargetType", "Type" },
                values: new object[,]
                {
                    { 1, 20, 3, "A powerful physical strike.", 5, "Bash", 0, 1, 1.5, "STR", "bash", 1, 0 },
                    { 2, 0, 10, "Force the enemy to focus on you.", 3, "Taunt", 0, 3, 0.0, "CON", "taunt", 1, 0 },
                    { 3, 0, 30, "Temporarily increase defense.", 10, "Iron Skin", 0, 5, 0.5, "CON", "ironskin", 0, 0 },
                    { 4, 25, 2, "Hurl a ball of fire at the enemy.", 10, "Fireball", 1, 1, 2.0, "INT", "fireball", 1, 1 },
                    { 5, 15, 10, "Unleash a storm of ice on all enemies.", 25, "Ice Storm", 1, 5, 1.5, "INT", "icestorm", 2, 1 },
                    { 6, 0, 60, "Convert damage to MP cost.", 20, "Mana Shield", 1, 3, 0.0, "INT", "manashield", 0, 1 },
                    { 7, 30, 3, "Restore HP to yourself.", 8, "Heal", 2, 1, 2.0, "WIS", "heal", 0, 2 },
                    { 8, 0, 30, "Increase STR and DEX temporarily.", 15, "Bless", 2, 3, 0.29999999999999999, "WIS", "bless", 0, 2 },
                    { 9, 20, 4, "Holy damage that scales with WIS.", 12, "Smite", 2, 1, 1.8, "WIS", "smite", 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "LootTableEntries",
                columns: new[] { "Id", "DropRate", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 1, 50.0, 1, 2, 1, 1 },
                    { 2, 60.0, 2, 1, 1, 2 },
                    { 3, 100.0, 3, 1, 1, 3 },
                    { 4, 10.0, 6, 1, 1, 1 },
                    { 5, 15.0, 6, 1, 1, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveCombats_MonsterSpawnId",
                table: "ActiveCombats",
                column: "MonsterSpawnId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveCombats_PlayerId",
                table: "ActiveCombats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemId",
                table: "InventoryItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_PlayerId",
                table: "InventoryItems",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LootTableEntries_ItemId",
                table: "LootTableEntries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LootTableEntries_MonsterId",
                table: "LootTableEntries",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpawns_MonsterTemplateId",
                table: "MonsterSpawns",
                column: "MonsterTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveCombats");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "LootTableEntries");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "MonsterSpawns");

            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "ExitsJson",
                value: "{\"n\":2,\"e\":3}");
        }
    }
}
