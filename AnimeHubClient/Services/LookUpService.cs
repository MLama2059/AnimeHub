using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHub.Shared.Models.Dtos.Genre;
using AnimeHub.Shared.Models.Dtos.Studio;
using System.Net.Http;
using System.Net.Http.Json;

namespace AnimeHubClient.Services
{
    public class LookUpService : ILookUpService
    {
        private readonly HttpClient _httpClient;

        public LookUpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryReadDto>?> GetCategoriesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CategoryReadDto>>("api/category");
        }

        public async Task<List<GenreReadDto>?> GetGenresAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<GenreReadDto>>("api/genre");
        }

        public async Task<List<StudioReadDto>?> GetStudiosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<StudioReadDto>>("api/studio");
        }
    }
}
