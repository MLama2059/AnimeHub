using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Enums;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IWatchlistRepository
    {
        Task<bool> AddToWatchlistAsync(int userId, int animeId);
        Task<bool> RemoveFromWatchlistAsync(int userId, int animeId);
        Task<WatchStatus?> GetWatchStatusAsync(int userId, int animeId);
        Task<bool> UpdateWatchStatusAsync(int userId, int animeId, WatchStatus newStatus);
        Task<List<AnimeListReadDto>> GetMyWatchlistAsync(int userId);
    }
}
