using AnimeHub.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimeHubApi.Data
{
    public class AnimeDbContext(DbContextOptions<AnimeDbContext> options) : DbContext(options)
    {
        public DbSet<Anime> Animes => Set<Anime>();
        public DbSet<Category> Categories => Set <Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Anime>().HasData(
                new Anime
                {
                    Id = 1,
                    Title = "Naruto Shippuden",
                    Genre = "Action/Adventure",
                    Episodes = 500,
                    YearPublished = 2007
                },
                new Anime
                {
                    Id = 2,
                    Title = "Demon Slayer",
                    Genre = "Action/Fantasy",
                    Episodes = 55, // (as of Season 3 end)
                    YearPublished = 2019
                },
                new Anime
                {
                    Id = 3,
                    Title = "Jujutsu Kaisen",
                    Genre = "Action/Supernatural",
                    Episodes = 47, // (as of Season 2 end)
                    YearPublished = 2020
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Shounen" },
                new Category { Id = 2, Name = "Fantasy" },
                new Category { Id = 3, Name = "Sci-Fi" },
                new Category { Id = 4, Name = "Slice of Life" },
                new Category { Id = 5, Name = "Isekai" },
                new Category { Id = 6, Name = "Horror" },
                new Category { Id = 7, Name = "Mecha" }
            );
        }
    }
}
