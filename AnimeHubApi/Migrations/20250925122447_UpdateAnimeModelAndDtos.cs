using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeHubApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnimeModelAndDtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearPublished",
                table: "Animes");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Animes",
                newName: "Studio");

            migrationBuilder.AlterColumn<int>(
                name: "Episodes",
                table: "Animes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PremieredYear",
                table: "Animes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Animes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PremieredYear", "Status", "Studio" },
                values: new object[] { 2007, "Completed", "Studio Pierrot" });

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PremieredYear", "Status", "Studio" },
                values: new object[] { 2019, "Airing", "Ufotable" });

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PremieredYear", "Status" },
                values: new object[] { 2001, "Completed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremieredYear",
                table: "Animes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Animes");

            migrationBuilder.RenameColumn(
                name: "Studio",
                table: "Animes",
                newName: "Author");

            migrationBuilder.AlterColumn<int>(
                name: "Episodes",
                table: "Animes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearPublished",
                table: "Animes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Author", "YearPublished" },
                values: new object[] { "Masashi Kishimoto", 2007 });

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Author", "YearPublished" },
                values: new object[] { "Koyoharu Gotouge", 2019 });

            migrationBuilder.UpdateData(
                table: "Animes",
                keyColumn: "Id",
                keyValue: 3,
                column: "YearPublished",
                value: 2001);
        }
    }
}
