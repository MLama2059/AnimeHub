using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHub.Shared.Models.Dtos.Genre;
using AnimeHub.Shared.Models.Dtos.Studio;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnimeHubClient.Services
{
    public class AnimeService : IAnimeService
    {
        private readonly HttpClient _httpClient;
        private readonly string API_BASE_URL = "api/anime";
        private const int MAX_IMAGE_SIZE_MB = 5;
        private const int MAX_VIDEO_SIZE_MB = 100;

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
            return await UploadFileInternalAsync(
                file,
                $"{API_BASE_URL}/upload-image",
                oldImageUrl,
                "oldImageUrl",
                MAX_IMAGE_SIZE_MB
            );
        }

        // PRIVATE UNIFIED HELPER METHOD
        private async Task<string?> UploadFileInternalAsync(
            IBrowserFile file,
            string endpoint,
            string? oldUrl,
            string oldUrlParamName,
            int maxFileSizeMb)
        {
            var maxFileSize = maxFileSizeMb * 1024 * 1024;

            if (file.Size > maxFileSize)
            {
                // CRITICAL FIX: If file fails validation (e.g., too big), return the original URL
                // to prevent accidentally deleting the existing file path during an update.
                return oldUrl;
            }

            try
            {
                var content = new MultipartFormDataContent();

                // Read the file stream into the content, limiting the stream length to the max file size
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);

                // Build the query parameter for the old file
                string oldUrlQuery = string.IsNullOrEmpty(oldUrl)
                    ? string.Empty
                    : $"{oldUrlParamName}={Uri.EscapeDataString(oldUrl)}";

                // Construct the full URI, handling the query parameter
                string requestUri = $"{endpoint}";
                if (!string.IsNullOrEmpty(oldUrlQuery))
                {
                    requestUri += $"?{oldUrlQuery}";
                }

                var response = await _httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var path = await response.Content.ReadAsStringAsync();
                    // Clean up and return the path
                    return path.Trim('"').Replace("\\", "/");
                }
                else
                {
                    // If API call fails, also return the old URL to preserve existing state
                    return oldUrl;
                }
            }
            catch (Exception)
            {
                // Log exception in a production app
                // If an exception occurs, return the old URL to preserve existing state
                return oldUrl;
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

        public async Task<string?> UploadTrailerPosterAsync(IBrowserFile file, string? oldPosterUrl)
        {
            return await UploadFileInternalAsync(
                file,
                $"{API_BASE_URL}/upload-trailer-poster",
                oldPosterUrl,
                "oldPosterUrl",
                MAX_IMAGE_SIZE_MB
            );
        }

        public async Task<string?> UploadTrailerAsync(IBrowserFile file, string? oldTrailerUrl)
        {
            return await UploadFileInternalAsync(
                file,
                $"{API_BASE_URL}/upload-trailer",
                oldTrailerUrl,
                "oldTrailerUrl",
                MAX_VIDEO_SIZE_MB
            );
        }

        public async Task<List<AnimeListReadDto>> GetRecommendationsAsync(int animeId, int count = 6)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<AnimeListReadDto>>($"{API_BASE_URL}/{animeId}/recommendations?count={count}");
                return result ?? new List<AnimeListReadDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching recommendations: {ex.Message}");
                return new List<AnimeListReadDto>();
            }
        }
    }
}
