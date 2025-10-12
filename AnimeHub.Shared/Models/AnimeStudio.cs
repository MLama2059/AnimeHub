using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models
{
    public class AnimeStudio
    {
        // Composite Key properties
        public int AnimeId { get; set; }
        public int StudioId { get; set; }

        // Navigation Properties
        public Anime? Anime { get; set; }
        public Studio? Studio { get; set; }
    }
}
