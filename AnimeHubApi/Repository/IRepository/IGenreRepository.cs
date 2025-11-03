using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Genre;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IGenreRepository
    {
        Task<List<GenreReadDto>> GetAllAsync();
        Task<GenreReadDto?> GetByIdAsync(int id);
        Task<GenreReadDto> AddAsync(GenreUpsertDto createDto);
        Task<bool> UpdateAsync(int id, GenreUpsertDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
