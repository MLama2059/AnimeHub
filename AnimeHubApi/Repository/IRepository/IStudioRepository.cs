using AnimeHub.Shared.Models;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IStudioRepository
    {
        Task<List<Studio>> GetAllAsync();
        Task<Studio?> GetByIdAsync(int id);
        Task<Studio> AddAsync(Studio studio);
        Task<bool> UpdateAsync(Studio studio);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);
    }
}
