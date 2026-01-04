using AnimeHub.Shared.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models
{
    public class AnimeProposal
    {
        public int Id { get; set; }

        // Metadata
        public int UserId { get; set; } // The User who submitted it
        public User User { get; set; } = null!;

        public ProposalStatus ProposalStatus { get; set; } = ProposalStatus.Pending;
        public ProposalType ProposalType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? AdminFeedback { get; set; } // Reason for rejection

        // If this is an UPDATE, which Anime are we changing?
        public int? TargetAnimeId { get; set; }
        public Anime? TargetAnime { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [Range(1, 5000)]
        public int? Episodes { get; set; }
        public Season? Season { get; set; }
        [Range(1900, 2100)]
        public int? PremieredYear { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        // Properties for the trailer
        public string? TrailerUrl { get; set; }
        public string? TrailerPosterUrl { get; set; }
        [Range(0, 10)]
        public double? Rating { get; set; }
        public Status? Status { get; set; }

        // Foreign Key -> Category
        public int? CategoryId { get; set; }

        // Instead of complex join tables, we store IDs as a JSON string.
        // Example: "[1, 5, 20]"
        public string SerializedGenreIds { get; set; } = "[]";
        public string SerializedStudioIds { get; set; } = "[]";
    }
}
