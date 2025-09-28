using AnimeHub.Shared.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.Anime
{
    public class AnimeReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? Episodes { get; set; }
        public string Season { get; set; } = string.Empty; // Displays the word "Winter", "Spring", etc.
        public int? PremieredYear { get; set; }

        // Linked entities
        public int CategoryId { get; set; } // For AnimeUpsert page
        public string CategoryName { get; set; } = string.Empty; // From Category table
        public List<int> GenreIds { get; set; } = new(); // For AnimeUpsert page
        public List<string> Genres { get; set; } = new(); // List of genre names
        public List<string> Studios { get; set; } = new(); // List of studio names
        public HashSet<int> StudioIds { get; set; } = new(); // For AnimeUpsert page

        // Extra info
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
