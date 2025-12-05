using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.UserAnime;
using AnimeHub.Shared.Models.Enums;

namespace AnimeHubClient.Services
{
    public interface IWatchlistService
    {
        Task<bool> AddToWatchlistAsync(int animeId);
        Task<bool> RemoveFromWatchlistAsync(int animeId);
        Task<WatchStatus?> GetWatchStatusAsync(int animeId);
        Task<bool> UpdateWatchStatusAsync(int animeId, WatchStatus newStatus);
        Task<List<UserAnimeReadDto>> GetMyWatchListAsync();
    }
}
