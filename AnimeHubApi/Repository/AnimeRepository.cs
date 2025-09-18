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

        public async Task<Anime> AddAsync(Anime anime, List<int> genreIds)
        {
            // Attach genres to the anime
            foreach (var genreId in genreIds)
            {
                anime.AnimeGenres.Add(new AnimeGenre { GenreId = genreId });
            }

            _context.Animes.Add(anime);
            await _context.SaveChangesAsync();

            // Re-fetch anime including Category and Genres
            return await _context.Animes
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres)
                    .ThenInclude(ag => ag.Genre)
                .FirstOrDefaultAsync(a => a.Id == anime.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var anime = await _context.Animes
                .Include(a => a.AnimeGenres)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anime is null)
                return false;

            _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Animes.Any(a => a.Id == id);
        }

        public async Task<List<Anime>> GetAllAsync()
        {
            return await _context.Animes
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
                .ToListAsync();
        }

        public async Task<Anime?> GetByIdAsync(int id)
        {
            return await _context.Animes
                .Include(a => a.Category)
                .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> UpdateAsync(Anime anime, List<int> genreIds)
        {
            var existingAnime = await _context.Animes
                .Include(a => a.AnimeGenres)
                .FirstOrDefaultAsync(a => a.Id == anime.Id);

            if (existingAnime == null)
                return false;

            existingAnime.Title = anime.Title;
            existingAnime.Episodes = anime.Episodes;
            existingAnime.YearPublished = anime.YearPublished;
            existingAnime.Description = anime.Description;
            existingAnime.Author = anime.Author;
            existingAnime.ImageUrl = anime.ImageUrl;
            existingAnime.Rating = anime.Rating;
            existingAnime.CategoryId = anime.CategoryId;

            // Update genres
            existingAnime.AnimeGenres.Clear();
            foreach (var genreId in genreIds)
            {
                existingAnime.AnimeGenres.Add(new AnimeGenre { AnimeId = anime.Id, GenreId = genreId });
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
