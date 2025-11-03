using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Studio;
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

        public async Task<List<StudioReadDto>> GetAllAsync()
        {
            return await _context.Studios
                .Include(s => s.AnimeStudios)
                .ThenInclude(ast => ast.Anime)
                .Select(s => new StudioReadDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<StudioReadDto?> GetByIdAsync(int id)
        {
            var studio = await _context.Studios
                .AsNoTracking()
                .Include(s => s.AnimeStudios)
                .ThenInclude(ast => ast.Anime)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studio == null)
                return null;

            return new StudioReadDto
            {
                Id = studio.Id,
                Name = studio.Name
            };
        }

        public async Task<StudioReadDto> AddAsync(StudioUpsertDto createDto)
        {
            var studio = new Studio
            {
                Name = createDto.Name
            };

            _context.Studios.Add(studio);
            await _context.SaveChangesAsync();
            return new StudioReadDto
            {
                Id = studio.Id,
                Name = studio.Name
            };
        }

        public async Task<bool> UpdateAsync(int id, StudioUpsertDto updateDto)
        {
            var existingStudio = await _context.Studios.FindAsync(id);
            if (existingStudio == null)
                return false;

            existingStudio.Name = updateDto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
                return false;

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(u => u.Id == id);
        }
    }
}
