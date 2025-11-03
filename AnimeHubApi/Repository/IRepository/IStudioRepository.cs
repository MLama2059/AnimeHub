using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Studio;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IStudioRepository
    {
        Task<List<StudioReadDto>> GetAllAsync();
        Task<StudioReadDto?> GetByIdAsync(int id);
        Task<StudioReadDto> AddAsync(StudioUpsertDto createDto);
        Task<bool> UpdateAsync(int id, StudioUpsertDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
