using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KingOfKings.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMonstersAndChineseLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "Id", "Attack", "CurrentHp", "Defense", "ExpReward", "LocationId", "MaxHp", "Name" },
                values: new object[,]
                {
                    { 1, 5, 50, 2, 10, 2, 50, "木頭人偶" },
                    { 2, 3, 30, 1, 5, 2, 30, "訓練假人" },
                    { 3, 12, 80, 5, 25, 1, 80, "野狼" },
                    { 4, 12, 80, 5, 25, 1, 80, "wolf" },
                    { 5, 8, 40, 3, 15, 3, 40, "大老鼠" }
                });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "你站在一個和平村莊的中心。附近有一座噴泉正輕柔地冒著水泡。北邊通往訓練場，東邊是村長的家。", "村莊廣場" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "木頭人偶排成一列列站在這裡。空氣中充滿了敲擊木頭的聲音。南邊是村莊廣場。", "訓練場" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "一間舒適的小屋，空氣中瀰漫著草藥的香味。西邊可以回到村莊廣場。", "村長的家" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "You are standing in the center of a peaceful village. A fountain bubbles softly nearby.", "Village Square" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Wooden dummies stand in rows here. The sound of striking wood fills the air.", "Training Grounds" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "A cozy hut with the smell of herbs.", "Village Elder's House" });
        }
    }
}
