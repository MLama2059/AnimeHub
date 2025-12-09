using AnimeHub.Shared.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.UserAnime
{
    public class UserAnimeReadDto
    {
        public int AnimeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public WatchStatus WatchStatus { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
