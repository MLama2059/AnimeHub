using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos;
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

        public async Task<PagedList<UserAnimeReadDto>> GetMyWatchlistAsync(int userId, APIParams apiParams)
        {
            // Base query: Only retrieve items for the given user, and include the related Anime details.
            var query = _context.UserAnimes
                .AsNoTracking()
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Anime);

            // 1. Apply Filtering by Title (Search Box)
            if (!string.IsNullOrWhiteSpace(apiParams.FilterOn) && !string.IsNullOrWhiteSpace(apiParams.FilterQuery))
            {
                var filterOn = apiParams.FilterOn.ToLowerInvariant();
                var filterQuery = apiParams.FilterQuery.ToLowerInvariant();

                if (filterOn.Equals("title"))
                {
                    query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<UserAnime, Anime?>)query.Where(ua => ua.Anime != null && ua.Anime.Title.ToLower().Contains(filterQuery));
                }
            }

            // 2. Apply Filtering by Watch Status (New requirement)
            // We'll use a new custom property in APIParams or query string, let's call it StatusFilter
            if (apiParams.StatusFilter != null && apiParams.StatusFilter > 0)
            {
                // Assuming WatchStatus enum values start at 1 (or 0 for planning)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<UserAnime, Anime?>)query.Where(ua => (int)ua.Status == apiParams.StatusFilter);
            }

            // 3. Apply Projection (Map to DTO)
            var projectedQuery = query.Select(ua => new UserAnimeReadDto
            {
                AnimeId = ua.AnimeId,
                Title = ua.Anime.Title ?? "Unknown Title", // Null check for safety
                ImageUrl = ua.Anime.ImageUrl,
                WatchStatus = ua.Status,
                DateAdded = ua.DateAdded // Include DateAdded for sorting
            });

            // 4. Apply Sorting (Title or Date Added)
            if (!string.IsNullOrWhiteSpace(apiParams.OrderBy))
            {
                var orderByParts = apiParams.OrderBy.ToLowerInvariant().Split('_');
                var sortField = orderByParts[0];
                var sortDirection = orderByParts.Length > 1 && orderByParts[1] == "desc" ? "desc" : "asc";

                switch (sortField)
                {
                    case "title":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.Title)
                            : projectedQuery.OrderBy(a => a.Title);
                        break;

                    case "dateadded":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.DateAdded)
                            : projectedQuery.OrderBy(a => a.DateAdded);
                        break;

                    default: // Default: Date Added Descending
                        projectedQuery = projectedQuery.OrderByDescending(a => a.DateAdded);
                        break;
                }
            }
            else
            {
                // Default sorting
                projectedQuery = projectedQuery.OrderByDescending(a => a.DateAdded);
            }

            // 5. Apply Pagination
            return await PagedList<UserAnimeReadDto>.CreateAsync(
                projectedQuery,
                apiParams.PageNumber,
                apiParams.PageSize);
        }
    }
}
