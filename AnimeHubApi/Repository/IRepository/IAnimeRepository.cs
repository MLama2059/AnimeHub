using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IAnimeRepository
    {
        Task<PagedList<AnimeListReadDto>> GetAllAsync(APIParams apiParams);
        Task<Anime?> GetByIdAsync(int id);
        Task<Anime> AddAsync(Anime anime, List<int> genreIds, HashSet<int> studioIds);
        Task<bool> UpdateAsync(Anime anime, List<int> genreIds, HashSet<int> studioIds);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);

        // Method to get top-rated anime
        Task<IEnumerable<Anime>> GetTopRatedAnimesAsync(int count);
    }
}
