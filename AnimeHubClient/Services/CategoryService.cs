using AnimeHub.Shared.Models.Dtos.Category;
using System.Net.Http.Json;

namespace AnimeHubClient.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string API_BASE_URL = "api/category";

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CategoryReadDto?> GetCategoryByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CategoryReadDto>($"{API_BASE_URL}/{id}");
        }

        public async Task<bool> CreateCategoryAsync(CategoryCreateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(API_BASE_URL, dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryUpdateDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{API_BASE_URL}/{dto.Id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{API_BASE_URL}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
