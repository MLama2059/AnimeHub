using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.Anime
{
    public class AnimeUpdateDto
    {
        public int Id { get; set; }  // Needed to identify which anime to update
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Studio { get; set; }
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
        public int? Episodes { get; set; }
        public int? PremieredYear { get; set; }
        [Required]
        public string? Status { get; set; }

        [Required]
        public int CategoryId { get; set; } // Link to category

        public List<int> GenreIds { get; set; } = new(); // Link to genres (many-to-many)
    }
}
