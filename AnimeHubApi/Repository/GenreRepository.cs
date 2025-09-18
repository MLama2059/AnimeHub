using AnimeHub.Shared.Models;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AnimeHubApi.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;
        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> AddAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return genre;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre is null)
                return false;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(u => u.Id == id);
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres
                .Include(g => g.AnimeGenres)
                .ThenInclude(ag => ag.Anime)
                .ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres
                .Include(g => g.AnimeGenres)
                .ThenInclude(ag => ag.Anime)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<bool> UpdateAsync(Genre genre)
        {
            var existingGenre = await _context.Genres.FindAsync(genre.Id);
            if (existingGenre is null)
                return false;

            existingGenre.Name = genre.Name;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
