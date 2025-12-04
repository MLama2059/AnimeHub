using AnimeHub.Shared.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AnimeHub.Shared.Models
{
    public class UserAnime
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; } // Navigation property
        [Required]
        public int AnimeId { get; set; }
        [ForeignKey("AnimeId")]
        public Anime? Anime { get; set; } // Navigation property
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public WatchStatus Status { get; set; } = WatchStatus.PlanToWatch;
    }
}
