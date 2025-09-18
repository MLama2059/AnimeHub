using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
        // Navigation property
        public ICollection<AnimeGenre> AnimeGenres { get; set; } = new List<AnimeGenre>();
    }
}
