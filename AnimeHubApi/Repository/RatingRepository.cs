using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Rating;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AnimeHubApi.Repository
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RatingReadDto>> GetRatingsAsync(int animeId)
        {
            return await _context.UserAnimeRatings
                .Where(r => r.AnimeId == animeId)
                .Include(r => r.User) // Include User to get the Username
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RatingReadDto
                {
                    Id = r.Id,
                    AnimeId = r.AnimeId,
                    UserId = r.UserId,
                    UserName = r.User!.Username,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();

        }

        public async Task<RatingReadDto> GetUserRatingsAsync(int userId, int animeId)
        {
            var rating = await _context.UserAnimeRatings
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.AnimeId == animeId);

            if (rating == null)
                return null;

            return new RatingReadDto
            {
                Id = rating.Id,
                AnimeId = rating.AnimeId,
                UserId = rating.UserId,
                UserName = rating.User!.Username,
                Rating = rating.Rating,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt
            };
        }

        public async Task<RatingReadDto> AddOrUpdateRatingAsync(int userId, RatingCreateDto ratingDto)
        {
            // Check if rating already exists
            var existingRating = await _context.UserAnimeRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.AnimeId == ratingDto.AnimeId);

            if (existingRating != null)
            {
                // Update existing
                existingRating.Rating = ratingDto.Rating;
                existingRating.Comment = ratingDto.Comment;
                existingRating.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new
                var newRating = new UserAnimeRating
                {
                    UserId = userId,
                    AnimeId = ratingDto.AnimeId,
                    Rating = ratingDto.Rating,
                    Comment = ratingDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserAnimeRatings.Add(newRating);
            }

            await _context.SaveChangesAsync();

            return await GetUserRatingsAsync(userId, ratingDto.AnimeId)
                   ?? throw new Exception("Failed to retrieve rating after saving.");
        }

    }
}
