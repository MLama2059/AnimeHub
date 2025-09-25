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
                new Genre { Id = 2, Name = "Adventure" },
                new Genre { Id = 3, Name = "Cars" },
                new Genre { Id = 4, Name = "Comedy" },
                new Genre { Id = 5, Name = "Dementia" },
                new Genre { Id = 6, Name = "Demons" },
                new Genre { Id = 7, Name = "Drama" },
                new Genre { Id = 8, Name = "Ecchi" },
                new Genre { Id = 9, Name = "Fantasy" },
                new Genre { Id = 10, Name = "Game" },
                new Genre { Id = 11, Name = "Harem" },
                new Genre { Id = 12, Name = "Historical" },
                new Genre { Id = 13, Name = "Horror" },
                new Genre { Id = 14, Name = "Isekai" },
                new Genre { Id = 15, Name = "Josei" },
                new Genre { Id = 16, Name = "Kids" },
                new Genre { Id = 17, Name = "Magic" },
                new Genre { Id = 18, Name = "Martial Arts" },
                new Genre { Id = 19, Name = "Mecha" },
                new Genre { Id = 20, Name = "Military" },
                new Genre { Id = 21, Name = "Music" },
                new Genre { Id = 22, Name = "Mystery" },
                new Genre { Id = 23, Name = "Parody" },
                new Genre { Id = 24, Name = "Police" },
                new Genre { Id = 25, Name = "Psychological" },
                new Genre { Id = 26, Name = "Romance" },
                new Genre { Id = 27, Name = "Samurai" },
                new Genre { Id = 28, Name = "School" },
                new Genre { Id = 29, Name = "Sci-Fi" },
                new Genre { Id = 30, Name = "Seinen" },
                new Genre { Id = 31, Name = "Shoujo" },
                new Genre { Id = 32, Name = "Shoujo Ai" },
                new Genre { Id = 33, Name = "Shounen" },
                new Genre { Id = 34, Name = "Shounen Ai" },
                new Genre { Id = 35, Name = "Slice of Life" },
                new Genre { Id = 36, Name = "Space" },
                new Genre { Id = 37, Name = "Sports" },
                new Genre { Id = 38, Name = "Super Power" },
                new Genre { Id = 39, Name = "Supernatural" },
                new Genre { Id = 40, Name = "Thriller" },
                new Genre { Id = 41, Name = "Vampire" }
            );

            // Seed Anime
            modelBuilder.Entity<Anime>().HasData(
                new Anime
                {
                    Id = 1,
                    Title = "Naruto Shippuden",
                    Episodes = 500,
                    PremieredYear = 2007,
                    Status = "Completed",
                    CategoryId = 1, // TV Series
                    Description = "Ninja action adventure",
                    Studio = "Studio Pierrot",
                    Rating = 9.0,
                    ImageUrl = null
                },
                new Anime
                {
                    Id = 2,
                    Title = "Demon Slayer: Kimetsu no Yaiba",
                    Episodes = 55,
                    PremieredYear = 2019,
                    Status = "Airing",
                    CategoryId = 1, // TV Series
                    Description = "Fantasy action adventure",
                    Studio = "Ufotable",
                    Rating = 9.2,
                    ImageUrl = null
                },
                new Anime
                {
                    Id = 3,
                    Title = "Spirited Away",
                    Episodes = 1,
                    PremieredYear = 2001,
                    Status = "Completed",
                    CategoryId = 2, // Movie
                    Description = "Fantasy adventure movie",
                    Studio = "Studio Ghibli",
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