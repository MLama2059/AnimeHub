using AnimeHub.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AnimeHubApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Anime> Animes => Set<Anime>();
        public DbSet<Category> Categories => Set <Category>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<AnimeGenre> AnimeGenres => Set<AnimeGenre>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many
            modelBuilder.Entity<AnimeGenre>()
                .HasKey(ag => new { ag.AnimeId, ag.GenreId });

            modelBuilder.Entity<AnimeGenre>()
                .HasOne(ag => ag.Anime)
                .WithMany(a => a.AnimeGenres)
                .HasForeignKey(ag => ag.AnimeId);

            modelBuilder.Entity<AnimeGenre>()
                .HasOne(ag => ag.Genre)
                .WithMany(g => g.AnimeGenres)
                .HasForeignKey(ag => ag.GenreId);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "TV Series" },
                new Category { Id = 2, Name = "Movie" },
                new Category { Id = 3, Name = "OVA" }
            );

            // Seed Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Fantasy" },
                new Genre { Id = 3, Name = "Adventure" },
                new Genre { Id = 4, Name = "Romance" },
                new Genre { Id = 5, Name = "Horror" },
                new Genre { Id = 6, Name = "Sci-Fi" },
                new Genre { Id = 7, Name = "Slice of Life" }
            );

            // Seed Anime
            modelBuilder.Entity<Anime>().HasData(
                new Anime
                {
                    Id = 1,
                    Title = "Naruto Shippuden",
                    Episodes = 500,
                    YearPublished = 2007,
                    CategoryId = 1, // TV Series
                    Description = "Ninja action adventure",
                    Author = "Masashi Kishimoto",
                    Rating = 9.0,
                    ImageUrl = null
                },
                new Anime
                {
                    Id = 2,
                    Title = "Demon Slayer: Kimetsu no Yaiba",
                    Episodes = 55,
                    YearPublished = 2019,
                    CategoryId = 1, // TV Series
                    Description = "Fantasy action adventure",
                    Author = "Koyoharu Gotouge",
                    Rating = 9.2,
                    ImageUrl = null
                },
                new Anime
                {
                    Id = 3,
                    Title = "Spirited Away",
                    Episodes = 1,
                    YearPublished = 2001,
                    CategoryId = 2, // Movie
                    Description = "Fantasy adventure movie",
                    Author = "Studio Ghibli",
                    Rating = 9.5,
                    ImageUrl = null
                }
            );

            // Seed AnimeGenre relations
            modelBuilder.Entity<AnimeGenre>().HasData(
                // Naruto Shippuden
                new AnimeGenre { AnimeId = 1, GenreId = 1 }, // Action
                new AnimeGenre { AnimeId = 1, GenreId = 3 }, // Adventure

                // Demon Slayer
                new AnimeGenre { AnimeId = 2, GenreId = 1 }, // Action
                new AnimeGenre { AnimeId = 2, GenreId = 2 }, // Fantasy

                // Spirited Away
                new AnimeGenre { AnimeId = 3, GenreId = 2 }, // Fantasy
                new AnimeGenre { AnimeId = 3, GenreId = 3 }  // Adventure
            );
        }

    }
}
