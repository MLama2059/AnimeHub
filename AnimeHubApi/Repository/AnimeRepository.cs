using AnimeHubApi.Data;
using AnimeHub.Shared.Models;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AnimeHubApi.Repository
{
    public class AnimeRepository : IAnimeRepository
    {
        private readonly ApplicationDbContext _context;

        public AnimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }  

        public async Task<List<Anime>> GetAllAsync()
        {
            return await _context.Animes
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre)
                .Include(a => a.AnimeStudios).ThenInclude(ast => ast.Studio)
                .ToListAsync();
        }

        public async Task<Anime?> GetByIdAsync(int id)
        {
            return await _context.Animes
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre)
                .Include(a => a.AnimeStudios).ThenInclude(ast => ast.Studio)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Implementation to get top-rated anime
        public async Task<IEnumerable<Anime>> GetTopRatedAnimesAsync(int count)
        {
            return await _context.Animes
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre)
                .Include(a => a.AnimeStudios).ThenInclude(ast => ast.Studio)
                .OrderByDescending(a => a.Rating)
                .Take(count)
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
