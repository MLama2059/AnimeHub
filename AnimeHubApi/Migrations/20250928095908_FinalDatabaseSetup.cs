using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimeHubApi.Migrations
{
    /// <inheritdoc />
    public partial class FinalDatabaseSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Episodes = table.Column<int>(type: "int", nullable: true),
                    Season = table.Column<int>(type: "int", nullable: false),
                    PremieredYear = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimeGenres",
                columns: table => new
                {
                    AnimeId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeGenres", x => new { x.AnimeId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_AnimeGenres_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimeGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimeStudios",
                columns: table => new
                {
                    AnimeId = table.Column<int>(type: "int", nullable: false),
                    StudioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimeStudios", x => new { x.AnimeId, x.StudioId });
                    table.ForeignKey(
                        name: "FK_AnimeStudios_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimeStudios_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "TV Series" },
                    { 2, "Movie" },
                    { 3, "OVA" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Action" },
                    { 2, "Adventure" },
                    { 3, "Cars" },
                    { 4, "Comedy" },
                    { 5, "Cyberpunk" },
                    { 6, "Dementia" },
                    { 7, "Demons" },
                    { 8, "Drama" },
                    { 9, "Dystopian" },
                    { 10, "Ecchi" },
                    { 11, "Fantasy" },
                    { 12, "Game" },
                    { 13, "Gore" },
                    { 14, "Harem" },
                    { 15, "Historical" },
                    { 16, "Horror" },
                    { 17, "Isekai" },
                    { 18, "Josei" },
                    { 19, "Kids" },
                    { 20, "Magic" },
                    { 21, "Martial Arts" },
                    { 22, "Mecha" },
                    { 23, "Military" },
                    { 24, "Music" },
                    { 25, "Mystery" },
                    { 26, "Parody" },
                    { 27, "Police" },
                    { 28, "Psychological" },
                    { 29, "Romance" },
                    { 30, "Samurai" },
                    { 31, "School" },
                    { 32, "Sci-Fi" },
                    { 33, "Seinen" },
                    { 34, "Shoujo" },
                    { 35, "Shoujo Ai" },
                    { 36, "Shounen" },
                    { 37, "Shounen Ai" },
                    { 38, "Slice of Life" },
                    { 39, "Space" },
                    { 40, "Sports" },
                    { 41, "Super Power" },
                    { 42, "Supernatural" },
                    { 43, "Suspense" },
                    { 44, "Thriller" },
                    { 45, "Time Travel" },
                    { 46, "Tragedy" },
                    { 47, "Vampire" },
                    { 48, "Workplace" },
                    { 49, "Yaoi" },
                    { 50, "Yuri" }
                });

            migrationBuilder.InsertData(
                table: "Studios",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "8bit" },
                    { 2, "A-1 Pictures" },
                    { 3, "Bones" },
                    { 4, "CloverWorks" },
                    { 5, "David Production" },
                    { 6, "J.C.STAFF" },
                    { 7, "Kyoto Animation" },
                    { 8, "LIDENFILMS" },
                    { 9, "Madhouse" },
                    { 10, "MAPPA" },
                    { 11, "Production I.G" },
                    { 12, "Silver Link." },
                    { 13, "Studio DEEN" },
                    { 14, "Studio Ghibli" },
                    { 15, "Studio Pierrot" },
                    { 16, "Sunrise" },
                    { 17, "Toei Animation" },
                    { 18, "Trigger" },
                    { 19, "ufotable" },
                    { 20, "White Fox" }
                });

            migrationBuilder.InsertData(
                table: "Animes",
                columns: new[] { "Id", "CategoryId", "Description", "Episodes", "ImageUrl", "PremieredYear", "Rating", "Season", "Status", "Title" },
                values: new object[,]
                {
                    { 1, 1, "Ninja action adventure", 500, null, 2007, 9.0, 1, 3, "Naruto Shippuden" },
                    { 2, 1, "Fantasy action adventure", 55, null, 2019, 9.1999999999999993, 2, 3, "Demon Slayer: Kimetsu no Yaiba" },
                    { 3, 2, "Fantasy adventure movie", 1, null, 2001, 9.5, 3, 3, "Spirited Away" }
                });

            migrationBuilder.InsertData(
                table: "AnimeGenres",
                columns: new[] { "AnimeId", "GenreId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "AnimeStudios",
                columns: new[] { "AnimeId", "StudioId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimeGenres_GenreId",
                table: "AnimeGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Animes_CategoryId",
                table: "Animes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimeStudios_StudioId",
                table: "AnimeStudios",
                column: "StudioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimeGenres");

            migrationBuilder.DropTable(
                name: "AnimeStudios");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Animes");

            migrationBuilder.DropTable(
                name: "Studios");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
