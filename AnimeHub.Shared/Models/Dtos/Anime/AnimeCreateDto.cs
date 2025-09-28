using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.Anime
{
    public class AnimeCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? Episodes { get; set; }
        public int Season { get; set; } // Use int to represent Season enum
        public int? PremieredYear { get; set; }
        public int Status { get; set; }

        [Required]
        public int CategoryId { get; set; } // Link to category

        public List<int> GenreIds { get; set; } = new(); // Link to genres (many-to-many)
        public HashSet<int> StudioIds { get; set; } = new(); // Link to studios (many-to-many)
    }
}
