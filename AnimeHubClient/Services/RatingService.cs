using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.Rating;
using Microsoft.EntityFrameworkCore.Storage.Json;
using MudBlazor;
using System.Net.Http.Json;

namespace AnimeHubClient.Services
{
    public class RatingService : IRatingService
    {
        private readonly HttpClient _httpClient;
        private readonly ISnackbar _snackbar;

        public RatingService(HttpClient httpClient, ISnackbar snackbar)
        {
            _httpClient = httpClient;
            _snackbar = snackbar;
        }

        public async Task<List<RatingReadDto>> GetRatingsForAnimeAsync(int animeId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<RatingReadDto>>($"api/Rating/{animeId}")
                    ?? new List<RatingReadDto>();
            }
            catch (Exception)
            {
                // Fail silently or log, return empty list to not break UI
                return new List<RatingReadDto>();
            }
        }

        public async Task<RatingReadDto?> GetUserRatingAsync(int animeId)
        {
            try
            {
                // Protected endpoint. HttpClient handles the Bearer token automatically because of AuthService/CustomAuthStateProvider logic!
                var response = await _httpClient.GetAsync($"api/Rating/user/{animeId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return null; // User hasn't rated yet
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RatingReadDto?>();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<RatingReadDto?> UpsertRatingAsync(RatingCreateDto ratingDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Rating", ratingDto);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RatingReadDto>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _snackbar.Add($"Error submitting rating: {error}", Severity.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Connection error: {ex.Message}", Severity.Error);
                return null;
            }
        }
    }
}
