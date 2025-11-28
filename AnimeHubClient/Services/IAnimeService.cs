using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHub.Shared.Models.Dtos.Genre;
using AnimeHub.Shared.Models.Dtos.Studio;
using Microsoft.AspNetCore.Components.Forms;

namespace AnimeHubClient.Services
{
    public interface IAnimeService
    {
        // CRUD Operations
        Task<AnimeReadDto?> GetAnimeByIdAsync(int id);
        Task<bool> CreateAnimeAsync(AnimeCreateDto dto);
        Task<bool> UpdateAnimeAsync(AnimeUpdateDto dto);
        Task<bool> DeleteAnimeAsync(int id);

        // Lookup Data Loader
        Task<List<AnimeListReadDto>?> GetTopRatedAnimesAsync();

        // File Uploads
        Task<string?> UploadImageAsync(IBrowserFile file, string? oldImageUrl);
        Task<string?> UploadTrailerPosterAsync(IBrowserFile file, string? oldPosterUrl);
        Task<string?> UploadTrailerAsync(IBrowserFile file, string? oldTrailerUrl);

        // Methods for AnimeList.razor
        Task<(List<AnimeListReadDto>? Items, PagedListMetadata? Metadata)> GetPagedAnimeListAsync(Dictionary<string, string?> queryParams, CancellationToken cancellationToken);
      
        // Method for AnimeCatalog.razor
        Task<(List<AnimeListReadDto>? Items, PagedListMetadata? Metadata)> GetAnimeCatalogAsync(int pageNumber, int pageSize, string orderBy, string? filterQuery, string? filterOn = "Title");
    }
}
