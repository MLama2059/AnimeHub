using AnimeHub.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AnimeHub.Shared.Models
{
    public class Anime
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [Range(1, 5000)]
        public int? Episodes { get; set; }
        public Season? Season { get; set; }
        [Range(1900, 2100)]
        public int? PremieredYear { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        // Properties for the trailer
        public string? TrailerUrl { get; set; }
        public string? TrailerPosterUrl { get; set; }
        [Range(0, 10)]
        public double? Rating { get; set; }
        public Status? Status { get; set; }

        // Foreign Key -> Category
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Many-to-many with Genre
        public ICollection<AnimeGenre> AnimeGenres { get; set; } = new List<AnimeGenre>();
        public ICollection<AnimeStudio> AnimeStudios { get; set; } = new HashSet<AnimeStudio>();
    }
}
