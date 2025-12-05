using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.UserAnime;
using AnimeHub.Shared.Models.Enums;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AnimeHubApi.Repository
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WatchlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToWatchlistAsync(int userId, int animeId)
        {
            var exists = await _context.UserAnimes.AnyAsync(ua => ua.UserId == userId && ua.AnimeId == animeId);
            if (exists) return false;
            
            var userAnime = new UserAnime
            {
                UserId = userId,
                AnimeId = animeId,
                DateAdded = DateTime.UtcNow,
                Status = WatchStatus.PlanToWatch
            };

            _context.UserAnimes.Add(userAnime);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveFromWatchlistAsync(int userId, int animeId)
        {
            var item = await _context.UserAnimes.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AnimeId == animeId);
            if (item == null) return false;

            _context.UserAnimes.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<WatchStatus?> GetWatchStatusAsync(int userId, int animeId)
        {
            return await _context.UserAnimes
                    .Where(ua => ua.UserId == userId && ua.AnimeId == animeId)
                    .Select(ua => (WatchStatus?)ua.Status)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateWatchStatusAsync(int userId, int animeId, WatchStatus newStatus)
        {
            var item = await _context.UserAnimes.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AnimeId == animeId);

            if (item == null) return false;

            item.Status = newStatus;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<UserAnimeReadDto>> GetMyWatchlistAsync(int userId)
        {
            var watchlist = await _context.UserAnimes
                    .AsNoTracking()
                    .Where(ua => ua.UserId == userId)
                    .Include(ua => ua.Anime)
                    .OrderByDescending(ua => ua.DateAdded)
                    .Select(ua => new UserAnimeReadDto
                    {
                        AnimeId = ua.AnimeId,
                        Title = ua.Anime.Title,
                        ImageUrl = ua.Anime.ImageUrl,
                        WatchStatus = ua.Status
                    })
                    .ToListAsync();

            return watchlist;
        }
    }
}
