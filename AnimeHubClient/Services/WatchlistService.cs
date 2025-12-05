using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.UserAnime;
using AnimeHub.Shared.Models.Enums;
using System.Net.Http.Json;

namespace AnimeHubClient.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly HttpClient _httpClient;

        public WatchlistService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AddToWatchlistAsync(int animeId)
        {
            var response = await _httpClient.PostAsync($"api/watchlist/{animeId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFromWatchlistAsync(int animeId)
        {
            var response = await _httpClient.DeleteAsync($"api/watchlist/{animeId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<WatchStatus?> GetWatchStatusAsync(int animeId)
        {
            var response = await _httpClient.GetAsync($"api/watchlist/status/{animeId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<WatchStatus?>();
            }

            return null;
        }

        public async Task<bool> UpdateWatchStatusAsync(int animeId, WatchStatus newStatus)
        {
            var response = await _httpClient.PutAsync($"api/watchlist/status/{animeId}/{newStatus}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserAnimeReadDto>> GetMyWatchListAsync()
        {
            var response = await _httpClient.GetAsync("api/watchlist");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserAnimeReadDto>>() ?? new List<UserAnimeReadDto>();
            }

            return new List<UserAnimeReadDto>();
        }
    }
}
