using AnimeHub.Shared.Models;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IAnimeRepository
    {
        Task<List<Anime>> GetAllAsync();
        Task<Anime?> GetByIdAsync(int id);
        Task<Anime> AddAsync(Anime anime, List<int> genreIds, List<int> studioIds);
        Task<bool> UpdateAsync(Anime anime, List<int> genreIds, List<int> studioIds);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);

        // Method to get top-rated anime
        Task<IEnumerable<Anime>> GetTopRatedAnimesAsync(int count);
    }
}
