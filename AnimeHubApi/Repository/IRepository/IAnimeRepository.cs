using AnimeHub.Shared.Models;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IAnimeRepository
    {
        Task<List<Anime>> GetAllAsync();
        Task<Anime?> GetByIdAsync(int id);
        Task<Anime> AddAsync(Anime anime);
        Task<bool> UpdateAsync(int id, Anime anime);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);
    }
}
