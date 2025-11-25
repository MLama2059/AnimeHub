using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.Rating
{
    public class RatingReadDto
    {
        public int Id { get; set; }
        public int AnimeId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty; // To display "Reviewed by X"
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
