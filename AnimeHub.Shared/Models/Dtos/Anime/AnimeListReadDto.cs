using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.Anime
{
    public class AnimeListReadDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public int? Episodes { get; set; }
        public string? Season { get; set; }
        public int? PremieredYear { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
