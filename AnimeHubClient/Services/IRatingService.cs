using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.Rating;

namespace AnimeHubClient.Services
{
    public interface IRatingService
    {
        Task<List<RatingReadDto>> GetRatingsForAnimeAsync(int animeId);
        Task<RatingReadDto?> GetUserRatingAsync(int animeId);
        Task<RatingReadDto?> UpsertRatingAsync(RatingCreateDto ratingDto);
    }
}
