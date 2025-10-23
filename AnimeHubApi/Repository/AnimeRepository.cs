using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnimeHubApi.Repository
{
    public class AnimeRepository : IAnimeRepository
    {
        private readonly ApplicationDbContext _context;

        public AnimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<AnimeListReadDto>> GetAllAsync(APIParams apiParams)
        {
            // Use .AsNoTracking() for read-only queries for better performance
            var query = _context.Animes
                .Include(a => a.Category)
                .AsNoTracking();

            // Apply Searching
            if (!string.IsNullOrWhiteSpace(apiParams.FilterOn) && !string.IsNullOrWhiteSpace(apiParams.FilterQuery))
            {
                var filterOn = apiParams.FilterOn.ToLowerInvariant();
                var filterQuery = apiParams.FilterQuery.ToLowerInvariant();

                if (filterOn.Equals("title"))
                {
                    query = query.Where(a => a.Title.ToLower().Contains(filterQuery));
                }
                // Add more 'else if' blocks here if you want to filter other fields later (e.g., CategoryName)
            }

            // Apply Projection
            var projectedQuery = query
                .Select(a => new AnimeListReadDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ImageUrl = a.ImageUrl,
                    Rating = a.Rating,
                    Episodes = a.Episodes,
                    Season = a.Season.ToString(),
                    PremieredYear = a.PremieredYear,
                    Status = a.Status.ToString(),
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name
                });

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(apiParams.OrderBy))
            {
                switch (apiParams.OrderBy.ToLowerInvariant())
                {
                    case "title":
                        projectedQuery = projectedQuery.OrderBy(a => a.Title);
                        break;

                    case "rating":
                        projectedQuery = projectedQuery.OrderByDescending(a => a.Rating);
                        break;

                    case "premiered":
                        projectedQuery = projectedQuery.OrderByDescending(a => a.PremieredYear).ThenByDescending(a => a.Season);
                        break;

                    default:
                        projectedQuery = projectedQuery.OrderBy(a => a.Title);
                        break;
                }
            }
            else
            {
                // Default ordering
                projectedQuery = projectedQuery.OrderBy(a => a.Title);
            }

            // Apply pagination
            return await PagedList<AnimeListReadDto>.CreateAsync(
                projectedQuery,
                apiParams.PageNumber,
                apiParams.PageSize);
        }

        public async Task<AnimeReadDto> GetByIdAsync(int id)
        {
            return await _context.Animes
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre)
                .Include(a => a.AnimeStudios).ThenInclude(ast => ast.Studio)
                .Select(a => new AnimeReadDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Episodes = a.Episodes,
                    Season = a.Season.ToString(), // (Enum to String)
                    PremieredYear = a.PremieredYear,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Rating = a.Rating,
                    Status = a.Status.ToString(), // (Enum to String)
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    Genres = a.AnimeGenres.Select(ag => ag.Genre.Name).ToList(),
                    GenreIds = a.AnimeGenres.Select(ag => ag.GenreId).ToList(),
                    Studios = a.AnimeStudios.Select(ast => ast.Studio.Name).ToList(),
                    StudioIds = a.AnimeStudios.Select(ast => ast.StudioId).ToHashSet(),
                }).FirstOrDefaultAsync(a => a.Id == id);
        }

        // Implementation to get top-rated anime
        public async Task<IEnumerable<AnimeListReadDto>> GetTopRatedAnimesAsync(int count)
        {
            return await _context.Animes
                .AsNoTracking()
                .OrderByDescending(a => a.Rating)
                .Take(count)
                .Select(a => new AnimeListReadDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ImageUrl = a.ImageUrl,
                    Rating = a.Rating,
                    Episodes = a.Episodes,
                    Season = a.Season.ToString(),
                    PremieredYear = a.PremieredYear,
                    Status = a.Status.ToString(),
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name
                })
                .ToListAsync();
        }

        public async Task<Anime> AddAsync(Anime anime, List<int> genreIds, HashSet<int> studioIds)
        {
            // Attach genres to the anime
            if (genreIds != null)
            {
                foreach (var genreId in genreIds)
                {
                    anime.AnimeGenres.Add(new AnimeGenre { GenreId = genreId });
                }
            }

            // Attach studios to the anime
            if (studioIds != null)
            {
                foreach (var studioId in studioIds)
                {
                    anime.AnimeStudios.Add(new AnimeStudio { StudioId = studioId });
                }
            }

            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();

            // Re-fetch anime including Category, Genres and Studios
            return await _context.Animes
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre)
                .Include(a => a.AnimeStudios).ThenInclude(ast => ast.Studio)
                .FirstOrDefaultAsync(a => a.Id == anime.Id)
                !;
        }

        public async Task<bool> UpdateAsync(Anime anime, List<int> genreIds, HashSet<int> studioIds)
        {
            var existingAnime = await _context.Animes
                .Include(a => a.AnimeGenres)
                .Include(a => a.AnimeStudios)
                .FirstOrDefaultAsync(a => a.Id == anime.Id);

            if (existingAnime == null)
                return false;

            existingAnime.Title = anime.Title;
            existingAnime.Episodes = anime.Episodes;
            existingAnime.Season = anime.Season;
            existingAnime.PremieredYear = anime.PremieredYear;
            existingAnime.Description = anime.Description;
            existingAnime.ImageUrl = anime.ImageUrl;
            existingAnime.Rating = anime.Rating;
            existingAnime.Status = anime.Status;
            existingAnime.CategoryId = anime.CategoryId;

            // Update genres
            existingAnime.AnimeGenres.Clear();
            foreach (var genreId in genreIds)
            {
                existingAnime.AnimeGenres.Add(new AnimeGenre { AnimeId = anime.Id, GenreId = genreId });
            }

            // Update studios
            existingAnime.AnimeStudios.Clear();
            foreach (var studioId in studioIds)
            {
                existingAnime.AnimeStudios.Add(new AnimeStudio { AnimeId = anime.Id, StudioId = studioId });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var anime = await _context.Animes
                .Include(a => a.AnimeGenres)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anime is null)
                return false;

            if (!string.IsNullOrEmpty(anime.ImageUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), anime.ImageUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));

                // Delete the file if it exists
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Animes.Any(a => a.Id == id);
        }
    }
}
