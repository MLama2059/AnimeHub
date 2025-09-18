using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.Anime
{
    public class AnimeReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Episodes { get; set; }
        public int YearPublished { get; set; }

        // Linked entities
        public string CategoryName { get; set; } = string.Empty;  // From Category table
        public List<string> Genres { get; set; } = new();         // List of genre names

        // Extra info
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
    }
}
