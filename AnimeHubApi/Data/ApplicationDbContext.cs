using AnimeHub.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using AnimeHub.Shared.Models.Enums; // Add this line!

namespace AnimeHubApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Anime> Animes => Set<Anime>();
        public DbSet<Category> Categories => Set <Category>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<AnimeGenre> AnimeGenres => Set<AnimeGenre>();
        public DbSet<Studio> Studios => Set<Studio>();
        public DbSet<AnimeStudio> AnimeStudios => Set<AnimeStudio>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserAnimeRating> UserAnimeRatings => Set<UserAnimeRating>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many (Anime <-> Genre)
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

            // Configure many-to-many (Anime <-> Studio)
            modelBuilder.Entity<AnimeStudio>()
                .HasKey(ast => new { ast.AnimeId, ast.StudioId });

            modelBuilder.Entity<AnimeStudio>()
                .HasOne(ast => ast.Anime)
                .WithMany(a => a.AnimeStudios)
                .HasForeignKey(ast => ast.AnimeId);

            modelBuilder.Entity<AnimeStudio>()
                .HasOne(ast => ast.Studio)
                .WithMany(s => s.AnimeStudios)
                .HasForeignKey(ast => ast.StudioId);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "TV Series" },
                new Category { Id = 2, Name = "Movie" },
                new Category { Id = 3, Name = "OVA" }
            );

            // Seed Studios
            modelBuilder.Entity<Studio>().HasData(
                new Studio { Id = 1, Name = "8bit" },
                new Studio { Id = 2, Name = "A-1 Pictures" },
                new Studio { Id = 3, Name = "Bones" },
                new Studio { Id = 4, Name = "CloverWorks" },
                new Studio { Id = 5, Name = "David Production" },
                new Studio { Id = 6, Name = "J.C.STAFF" },
                new Studio { Id = 7, Name = "Kyoto Animation" },
                new Studio { Id = 8, Name = "LIDENFILMS" },
                new Studio { Id = 9, Name = "Madhouse" },
                new Studio { Id = 10, Name = "MAPPA" },
                new Studio { Id = 11, Name = "Production I.G" },
                new Studio { Id = 12, Name = "Silver Link." },
                new Studio { Id = 13, Name = "Studio DEEN" },
                new Studio { Id = 14, Name = "Studio Ghibli" },
                new Studio { Id = 15, Name = "Studio Pierrot" },
                new Studio { Id = 16, Name = "Sunrise" },
                new Studio { Id = 17, Name = "Toei Animation" },
                new Studio { Id = 18, Name = "Trigger" },
                new Studio { Id = 19, Name = "ufotable" },
                new Studio { Id = 20, Name = "White Fox" }
            );

            // Seed Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Adventure" },
                new Genre { Id = 3, Name = "Cars" },
                new Genre { Id = 4, Name = "Comedy" },
                new Genre { Id = 5, Name = "Cyberpunk" },
                new Genre { Id = 6, Name = "Dementia" },
                new Genre { Id = 7, Name = "Demons" },
                new Genre { Id = 8, Name = "Drama" },
                new Genre { Id = 9, Name = "Dystopian" },
                new Genre { Id = 10, Name = "Ecchi" },
                new Genre { Id = 11, Name = "Fantasy" },
                new Genre { Id = 12, Name = "Game" },
                new Genre { Id = 13, Name = "Gore" },
                new Genre { Id = 14, Name = "Harem" },
                new Genre { Id = 15, Name = "Historical" },
                new Genre { Id = 16, Name = "Horror" },
                new Genre { Id = 17, Name = "Isekai" },
                new Genre { Id = 18, Name = "Josei" },
                new Genre { Id = 19, Name = "Kids" },
                new Genre { Id = 20, Name = "Magic" },
                new Genre { Id = 21, Name = "Martial Arts" },
                new Genre { Id = 22, Name = "Mecha" },
                new Genre { Id = 23, Name = "Military" },
                new Genre { Id = 24, Name = "Music" },
                new Genre { Id = 25, Name = "Mystery" },
                new Genre { Id = 26, Name = "Parody" },
                new Genre { Id = 27, Name = "Police" },
                new Genre { Id = 28, Name = "Psychological" },
                new Genre { Id = 29, Name = "Romance" },
                new Genre { Id = 30, Name = "Samurai" },
                new Genre { Id = 31, Name = "School" },
                new Genre { Id = 32, Name = "Sci-Fi" },
                new Genre { Id = 33, Name = "Seinen" },
                new Genre { Id = 34, Name = "Shoujo" },
                new Genre { Id = 35, Name = "Shoujo Ai" },
                new Genre { Id = 36, Name = "Shounen" },
                new Genre { Id = 37, Name = "Shounen Ai" },
                new Genre { Id = 38, Name = "Slice of Life" },
                new Genre { Id = 39, Name = "Space" },
                new Genre { Id = 40, Name = "Sports" },
                new Genre { Id = 41, Name = "Super Power" },
                new Genre { Id = 42, Name = "Supernatural" },
                new Genre { Id = 43, Name = "Suspense" },
                new Genre { Id = 44, Name = "Thriller" },
                new Genre { Id = 45, Name = "Time Travel" },
                new Genre { Id = 46, Name = "Tragedy" },
                new Genre { Id = 47, Name = "Vampire" },
                new Genre { Id = 48, Name = "Workplace" },
                new Genre { Id = 49, Name = "Yaoi" },
                new Genre { Id = 50, Name = "Yuri" }
            );

            // Seed Anime
            modelBuilder.Entity<Anime>().HasData(
                new Anime
                {
                    Id = 1,
                    Title = "Naruto Shippuden",
                    Episodes = 500,
                    Season = Season.Winter,
                    PremieredYear = 2007,
                    Status = Status.Completed,
                    CategoryId = 1, // TV Series
                    Description = "Ninja action adventure",
                    Rating = 9.0,
                    ImageUrl = null
                },
                new Anime
                {
                    Id = 2,
                    Title = "Demon Slayer: Kimetsu no Yaiba",
                    Episodes = 55,
                    Season = Season.Spring,
                    PremieredYear = 2019,
                    Status = Status.Completed,
                    CategoryId = 1, // TV Series
                    Description = "Fantasy action adventure",
                    Rating = 9.2,
                    ImageUrl = null
                },
                new Anime
                {
                    Id = 3,
                    Title = "Spirited Away",
                    Episodes = 1,
                    Season = Season.Summer,
                    PremieredYear = 2001,
                    Status = Status.Completed,
                    CategoryId = 2, // Movie
                    Description = "Fantasy adventure movie",
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

            // Seed AnimeStudio relations
            modelBuilder.Entity<AnimeStudio>().HasData(
                new AnimeStudio { AnimeId = 1, StudioId = 1 }, // Naruto <-> Studio Pierrot
                new AnimeStudio { AnimeId = 2, StudioId = 2 }, // Demon Slayer <-> Ufotable
                new AnimeStudio { AnimeId = 3, StudioId = 3 }  // Spirited Away <-> Studio Ghibli
            );

            // Configure UserAnimeRating
            // Ensure the combination of AnimeId and UserId is unique 
            // (A user cannot rate the same anime twice)
            modelBuilder.Entity<UserAnimeRating>()
                .HasIndex(r => new { r.AnimeId, r.UserId })
                .IsUnique();
        }
    }
}