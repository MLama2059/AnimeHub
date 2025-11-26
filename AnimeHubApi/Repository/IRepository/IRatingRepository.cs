using AnimeHub.Shared.Models.Dtos.Rating;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IRatingRepository
    {
        // Get all reviews for a specific anime
        Task<List<RatingReadDto>> GetRatingsAsync(int animeId);
        // Get a specific user's rating for a specific anime (to check if they already rated)
        Task<RatingReadDto> GetUserRatingsAsync(int userId, int animeId);
        // Add or Update a rating
        Task<RatingReadDto> AddOrUpdateRatingAsync(int userId, RatingCreateDto ratingDto);
        // Delete a specific rating
        Task<bool> DeleteRatingAsync(int ratingId, int userId);
        // Recalculates and updates the Anime's average rating
        Task UpdateAnimeAverageRating(int animeId);
    }
}
