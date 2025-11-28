using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Enums;
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
        private readonly IFileService _fileService;

        public AnimeRepository(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
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
            var projectedQuery = query.Select(a => new AnimeListReadDto
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
                var orderByParts = apiParams.OrderBy.ToLowerInvariant().Split('_');
                var sortField = orderByParts[0]; // e.g., "title", "rating"
                var sortDirection = orderByParts.Length > 1 && orderByParts[1] == "desc" ? "desc" : "asc";

                switch (sortField)
                {
                    case "title":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.Title)
                            : projectedQuery.OrderBy(a => a.Title);
                        break;

                    case "episodes":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.Episodes)
                            : projectedQuery.OrderBy(a => a.Episodes);
                        break;

                    case "premiered":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.PremieredYear).ThenByDescending(a => a.Season)
                            : projectedQuery.OrderBy(a => a.PremieredYear).ThenBy(a => a.Season);
                        break;

                    case "status":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.Status)
                            : projectedQuery.OrderBy(a => a.Status);
                        break;

                    case "categoryname":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.CategoryName)
                            : projectedQuery.OrderBy(a => a.CategoryName);
                        break;

                    case "rating":
                        projectedQuery = sortDirection == "desc"
                            ? projectedQuery.OrderByDescending(a => a.Rating)
                            : projectedQuery.OrderBy(a => a.Rating);
                        break;

                    default:
                        projectedQuery = projectedQuery.OrderBy(a => a.Title);
                        break;
                }
            }
            else
            {
                projectedQuery = projectedQuery.OrderBy(a => a.Title);
            }

            // Apply pagination
            return await PagedList<AnimeListReadDto>.CreateAsync(
                projectedQuery,
                apiParams.PageNumber,
                apiParams.PageSize);
        }

        public async Task<AnimeReadDto?> GetByIdAsync(int id)
        {
            return await _context.Animes
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
                    TrailerPosterUrl = a.TrailerPosterUrl,
                    TrailerUrl = a.TrailerUrl,
                    Rating = a.Rating,
                    Status = a.Status.ToString(), // (Enum to String)
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category.Name,
                    Genres = a.AnimeGenres.Select(ag => ag.Genre.Name).ToList(),
                    GenreIds = a.AnimeGenres.Select(ag => ag.GenreId).ToList(),
                    Studios = a.AnimeStudios.Select(ast => ast.Studio.Name).ToList(),
                    StudioIds = a.AnimeStudios.Select(ast => ast.StudioId).ToHashSet(),
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Implementation to get top-rated anime
        public async Task<IEnumerable<AnimeListReadDto>> GetTopRatedAnimesAsync(int count)
        {
            return await _context.Animes
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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AnimeReadDto> AddAsync(AnimeCreateDto animeDto)
        {
            var anime = new Anime
            {
                Title = animeDto.Title,
                Episodes = animeDto.Episodes,
                Season = (Season)animeDto.Season,
                PremieredYear = animeDto.PremieredYear,
                Description = animeDto.Description,
                ImageUrl = animeDto.ImageUrl,
                TrailerPosterUrl = animeDto.TrailerPosterUrl,
                TrailerUrl = animeDto.TrailerUrl,
                Rating = animeDto.Rating,
                Status = (Status)animeDto.Status,
                CategoryId = animeDto.CategoryId,
                AnimeGenres = animeDto.GenreIds?.Select(id => new AnimeGenre { GenreId = id }).ToList() ?? new(),
                AnimeStudios = animeDto.StudioIds?.Select(id => new AnimeStudio { StudioId = id }).ToList() ?? new()
            };

            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(anime.Id);
        }

        public async Task<bool> UpdateAsync(int id, AnimeUpdateDto animeDto)
        {
            var anime = await _context.Animes
                .Include(a => a.AnimeGenres)
                .Include(a => a.AnimeStudios)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anime == null)
                return false;

            anime.Title = animeDto.Title;
            anime.Episodes = animeDto.Episodes;
            anime.Season = (Season)animeDto.Season;
            anime.PremieredYear = animeDto.PremieredYear;
            anime.Description = animeDto.Description;
            anime.ImageUrl = animeDto.ImageUrl;
            anime.TrailerPosterUrl = animeDto.TrailerPosterUrl;
            anime.TrailerUrl = animeDto.TrailerUrl;
            anime.Rating = animeDto.Rating;
            anime.Status = (Status)animeDto.Status;
            anime.CategoryId = animeDto.CategoryId;

            anime.AnimeGenres.Clear();
            anime.AnimeStudios.Clear();

            if (animeDto.GenreIds != null)
                anime.AnimeGenres = animeDto.GenreIds.Select(genreId => new AnimeGenre { AnimeId = id, GenreId = genreId }).ToList();

            if (animeDto.StudioIds != null)
                anime.AnimeStudios = animeDto.StudioIds.Select(studioId => new AnimeStudio { AnimeId = id, StudioId = studioId }).ToList();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Retrieve the entity and its file paths before deleting it
            var anime = await _context.Animes
                .Where(a => a.Id == id)
                .Select(a => new {
                    Entity = a,
                    a.ImageUrl,
                    a.TrailerUrl,
                    a.TrailerPosterUrl
                })
                .FirstOrDefaultAsync();

            if (anime == null)
                return false;

            // Delete the associated files from the file system
            var filesToDelete = new List<string?>
            { 
                anime.ImageUrl,
                anime.TrailerUrl,
                anime.TrailerPosterUrl
            };

            // Delete files using the service
            _fileService.DeleteFiles(filesToDelete);

            _context.Animes.Remove(anime.Entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(string? imageUrl, string? trailerUrl, string? trailerPosterUrl)> GetFilePathsAsync(int id)
        {
            var anime = await _context.Animes
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new { a.ImageUrl, a.TrailerUrl, a.TrailerPosterUrl })
                .FirstOrDefaultAsync();

            if (anime == null) 
                return (null, null, null);

            return (anime.ImageUrl, anime.TrailerUrl, anime.TrailerPosterUrl);
        }

        public bool Exists(int id)
        {
            return _context.Animes.Any(a => a.Id == id);
        }

    }
}
