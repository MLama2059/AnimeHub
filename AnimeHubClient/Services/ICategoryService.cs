using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.Category;

namespace AnimeHubClient.Services
{
    public interface ICategoryService
    {
        Task<CategoryReadDto?> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(CategoryCreateDto dto);
        Task<bool> UpdateCategoryAsync(CategoryUpdateDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
