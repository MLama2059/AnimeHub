using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeHubApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTrailerToAnime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrailerPosterUrl",
                table: "Animes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrailerUrl",
                table: "Animes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "TrailerPosterUrl", "TrailerUrl" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "TrailerPosterUrl", "TrailerUrl" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "TrailerPosterUrl", "TrailerUrl" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrailerPosterUrl",
                table: "Animes");

            migrationBuilder.DropColumn(
                name: "TrailerUrl",
                table: "Animes");
        }
    }
}
