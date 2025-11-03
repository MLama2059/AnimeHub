using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Category;

namespace AnimeHubApi.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryReadDto>> GetAllAsync();
        Task<CategoryReadDto?> GetByIdAsync(int id);
        Task<CategoryReadDto> AddAsync(CategoryCreateDto createDto);
        Task<bool> UpdateAsync(int id, CategoryUpdateDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
