using AnimeHub.Shared.Models;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllAsync();
        Task<Genre?> GetByIdAsync(int id);
        Task<Genre> AddAsync(Genre genre);
        Task<bool> UpdateAsync(Genre genre);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);
    }
}
