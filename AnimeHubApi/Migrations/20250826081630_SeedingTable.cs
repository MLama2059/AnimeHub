using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimeHubApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Animes",
                columns: new[] { "Id", "Episodes", "Genre", "Title", "YearPublished" },
                values: new object[,]
                {
                    { 1, 500, "Action/Adventure", "Naruto Shippuden", 2007 },
                    { 2, 55, "Action/Fantasy", "Demon Slayer", 2019 },
                    { 3, 47, "Action/Supernatural", "Jujutsu Kaisen", 2020 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
