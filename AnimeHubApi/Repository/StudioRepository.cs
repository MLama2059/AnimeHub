using AnimeHub.Shared.Models;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AnimeHubApi.Repository
{
    public class StudioRepository : IStudioRepository
    {
        private readonly ApplicationDbContext _context;
        public StudioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Studio>> GetAllAsync()
        {
            return await _context.Studios
                .Include(s => s.AnimeStudios)
                .ThenInclude(ast => ast.Anime)
                .ToListAsync();
        }

        public async Task<Studio?> GetByIdAsync(int id)
        {
            return await _context.Studios
                .Include(s => s.AnimeStudios)
                .ThenInclude(ast => ast.Anime)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Studio> AddAsync(Studio studio)
        {
            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();
            return studio;
        }

        public async Task<bool> UpdateAsync(Studio studio)
        {
            var existingStudio = await _context.Studios.FindAsync(studio.Id);
            if (existingStudio is null)
                return false;

            existingStudio.Name = studio.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio is null)
                return false;

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Studios.Any(u => u.Id == id);
        }
    }
}
