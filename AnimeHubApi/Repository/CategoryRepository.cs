using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnimeHubApi.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryReadDto>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Animes)
                .Select(c => new CategoryReadDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Animes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return null;

            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryReadDto> AddAsync(CategoryCreateDto createDto)
        {
            var category = new Category
            {
                Name = createDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto updateDto)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                return false;

            existingCategory.Name = updateDto.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(u => u.Id == id);
        }
    }
}
