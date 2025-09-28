using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models
{
    public class Studio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AnimeStudio> AnimeStudios { get; set; } = new List<AnimeStudio>();
    }
}
