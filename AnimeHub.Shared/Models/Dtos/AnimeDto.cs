using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos
{
    public class AnimeDto
    {
        public string Title { get; set; } = string.Empty;
        public int Episodes { get; set; }
        public int YearPublished { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public double Rating { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }
}
