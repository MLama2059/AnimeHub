using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IAnimeRepository
    {
        Task<PagedList<AnimeListReadDto>> GetAllAsync(APIParams apiParams);
        Task<AnimeReadDto?> GetByIdAsync(int id);
        Task<IEnumerable<AnimeListReadDto>> GetTopRatedAnimesAsync(int count);
        Task<AnimeReadDto> AddAsync(AnimeCreateDto animeDto);
        Task<bool> UpdateAsync(int id, AnimeUpdateDto animeDto);
        Task<bool> DeleteAsync(int id);
        // Method to fetch file paths based on ID
        Task<(string? imageUrl, string? trailerUrl, string? trailerPosterUrl)> GetFilePathsAsync(int id);
        bool Exists(int id);
    }
}
