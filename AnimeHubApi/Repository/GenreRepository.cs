using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Genre;
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

        public async Task<List<GenreReadDto>> GetAllAsync()
        {
            return await _context.Genres
                .Include(g => g.AnimeGenres)
                .ThenInclude(ag => ag.Anime)
                .Select(g => new GenreReadDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GenreReadDto?> GetByIdAsync(int id)
        {
            var genre = await _context.Genres
                .AsNoTracking()
                .Include(g => g.AnimeGenres)
                .ThenInclude(ag => ag.Anime)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
                return null;

            return new GenreReadDto
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }

        public async Task<GenreReadDto> AddAsync(GenreUpsertDto createDto)
        {
            var genre = new Genre
            {
                Name = createDto.Name
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return new GenreReadDto
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }

        public async Task<bool> UpdateAsync(int id, GenreUpsertDto updateDto)
        {
            var existingGenre = await _context.Genres.FindAsync(id);
            if (existingGenre == null)
                return false;

            existingGenre.Name = updateDto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return false;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(u => u.Id == id);
        }
    }
}
