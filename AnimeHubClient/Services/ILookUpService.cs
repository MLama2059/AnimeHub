using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHub.Shared.Models.Dtos.Genre;
using AnimeHub.Shared.Models.Dtos.Studio;

namespace AnimeHubClient.Services
{
    public interface ILookUpService
    {
        Task<List<CategoryReadDto>?> GetCategoriesAsync();
        Task<List<GenreReadDto>?> GetGenresAsync();
        Task<List<StudioReadDto>?> GetStudiosAsync();
    }
}
