using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.UserAnime;
using AnimeHub.Shared.Models.Enums;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnimeHubClient.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly HttpClient _httpClient;
        private readonly string API_BASE_URL = "api/watchlist";

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

        //public async Task<List<UserAnimeReadDto>> GetMyWatchListAsync()
        //{
        //    var response = await _httpClient.GetAsync("api/watchlist");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadFromJsonAsync<List<UserAnimeReadDto>>() ?? new List<UserAnimeReadDto>();
        //    }

        //    return new List<UserAnimeReadDto>();
        //}

        public async Task<(List<UserAnimeReadDto>? Items, PagedListMetadata? Metadata)> GetPagedWatchlistAsync(Dictionary<string, string?> queryParams, CancellationToken cancellationToken)
        {
            // 1. Convert queryParams dictionary into a URL query string
            // Using QueryHelpers.AddQueryString is generally safer/cleaner than FormUrlEncodedContent for GET requests
            var uri = QueryHelpers.AddQueryString(API_BASE_URL, queryParams!);

            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            PagedListMetadata? metadata = null;

            // 2. Read the X-Pagination Header
            if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
            {
                var metadataJson = headerValues.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(metadataJson))
                {
                    // PagedListMetadata must be a shared DTO
                    metadata = JsonSerializer.Deserialize<PagedListMetadata>(metadataJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            // 3. Read the content (the list of items)
            var items = await response.Content.ReadFromJsonAsync<List<UserAnimeReadDto>>(cancellationToken);

            return (items, metadata);
        }
    }
}
