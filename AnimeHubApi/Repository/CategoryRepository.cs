using AnimeHub.Shared.Models;
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

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Animes)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Animes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.Id);
            if (existingCategory == null)
                return false;

            existingCategory.Name = category.Name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Categories.Any(u => u.Id == id);
        }
    }
}
