using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models
{
    public class AnimeGenre
    {
        public int AnimeId { get; set; }
        public Anime Anime { get; set; } = null!;
        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
    }
}
