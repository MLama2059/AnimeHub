using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHub.Shared.Models.Dtos.Genre;
using AnimeHub.Shared.Models.Dtos.Studio;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnimeHubClient.Services
{
    public class AnimeService : IAnimeService
    {
        private readonly HttpClient _httpClient;
        private readonly string API_BASE_URL = "api/anime";

        public AnimeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // LOOKUP LOADER
        public async Task<List<AnimeListReadDto>?> GetTopRatedAnimesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AnimeListReadDto>>($"{API_BASE_URL}/top");
        }

        // CRUD OPERATIONS (Used by LoadAnime and UpsertAnime)
        public async Task<AnimeReadDto?> GetAnimeByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<AnimeReadDto>($"{API_BASE_URL}/{id}");
        }

        public async Task<bool> CreateAnimeAsync(AnimeCreateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(API_BASE_URL, dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAnimeAsync(AnimeUpdateDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"{API_BASE_URL}/{dto.Id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAnimeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{API_BASE_URL}/{id}");
            return response.IsSuccessStatusCode;
        }

        // IMAGE UPLOAD
        public async Task<string?> UploadImageAsync(IBrowserFile file, string? oldImageUrl)
        {
            var maxFileSize = 5 * 1024 * 1024; // 5MB limit

            if (file.Size > maxFileSize)
            {
                return null;
            }

            try
            {
                var content = new MultipartFormDataContent();

                // Read the file stream into the content
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);

                // Build the query parameter for the old image
                string oldImageUrlParam = string.IsNullOrEmpty(oldImageUrl) ? "" : $"&oldImageUrl={Uri.EscapeDataString(oldImageUrl)}";

                var response = await _httpClient.PostAsync($"{API_BASE_URL}/upload-image?{oldImageUrlParam}", content);

                if (response.IsSuccessStatusCode)
                {
                    var path = await response.Content.ReadAsStringAsync();

                    // Clean up and return the path

                    return path.Trim('"').Replace("\\", "/");
                    //var relativePath = await response.Content.ReadAsStringAsync();
                    //relativePath = relativePath.Trim('"').Replace("\\", "/");

                    //// Return absolute URL
                    //return new Uri(_httpClient.BaseAddress!, relativePath).ToString();
                }
                else
                {
                    // Handle API failure
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        // TRAILER UPLOADS
        public async Task<string?> UploadTrailerPosterAsync(IBrowserFile file)
        {
            return await UploadFileAsync(file, $"{API_BASE_URL}/upload-trailer-poster");
        }

        public async Task<string?> UploadTrailerAsync(IBrowserFile file)
        {
            return await UploadFileAsync(file, $"{API_BASE_URL}/upload-trailer");
        }

        // PRIVATE HELPER METHOD
        private async Task<string?> UploadFileAsync(IBrowserFile file, string endpoint)
        {
            var maxFileSize = 100 * 1024 * 1024;

            if (file.Size > maxFileSize)
            {
                return null;
            }

            try
            {
                var content = new MultipartFormDataContent();

                // Read the file stream into the content, limiting the stream length
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                // The API controller expects the file under the name "file"
                content.Add(fileContent, "file", file.Name);

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var path = await response.Content.ReadAsStringAsync();

                    // Clean up and return the path

                    return path.Trim('"').Replace("\\", "/");
                    //var relativePath = await response.Content.ReadAsStringAsync();
                    //relativePath = relativePath.Trim('"').Replace("\\", "/");

                    //// Prepend the API Base Address
                    //var absoluteUrl = new Uri(_httpClient.BaseAddress!, relativePath).ToString();

                    //return absoluteUrl;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(List<AnimeListReadDto>? Items, PagedListMetadata? Metadata)> GetPagedAnimeListAsync(Dictionary<string, string?> queryParams, CancellationToken cancellationToken)
        {
            // Convert queryParams dictionary into a URL query string
            var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken);

            var response = await _httpClient.GetAsync($"{API_BASE_URL}?{queryString}", cancellationToken);
            response.EnsureSuccessStatusCode();

            PagedListMetadata? metadata = null;
            if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
            {
                var metadataJson = headerValues.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(metadataJson))
                {
                    metadata = JsonSerializer.Deserialize<PagedListMetadata>(metadataJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            var items = await response.Content.ReadFromJsonAsync<List<AnimeListReadDto>>(cancellationToken);

            return (items, metadata);
        }

        public async Task<(List<AnimeListReadDto>? Items, PagedListMetadata? Metadata)> GetAnimeCatalogAsync(int pageNumber, int pageSize, string orderBy, string? filterQuery, string? filterOn = "Title")
        {
            var queryParams = new Dictionary<string, string?>
            {
                {"pageNumber", pageNumber.ToString()},
                {"pageSize", pageSize.ToString()},
                {"orderBy", orderBy}
            };

            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                queryParams.Add("filterOn", filterOn);
                queryParams.Add("filterQuery", filterQuery);
            }

            // Convert queryParams dictionary into a URL query string
            var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();

            // NOTE: We do not pass CancellationToken here as the UI is handling the debounce/cancellation logic
            var response = await _httpClient.GetAsync($"{API_BASE_URL}?{queryString}");
            response.EnsureSuccessStatusCode();

            PagedListMetadata? metadata = null;
            if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
            {
                var metadataJson = headerValues.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(metadataJson))
                {
                    metadata = JsonSerializer.Deserialize<PagedListMetadata>(metadataJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            var items = await response.Content.ReadFromJsonAsync<List<AnimeListReadDto>>();

            return (items, metadata);
        }
    }
}
