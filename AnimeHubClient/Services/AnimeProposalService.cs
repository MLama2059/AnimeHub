using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.AnimeProposal;
using System.Net.Http.Json;

namespace AnimeHubClient.Services
{
    public class AnimeProposalService : IAnimeProposalService
    {
        private readonly HttpClient _httpClient;
        private readonly string API_BASE_URL = "api/animeproposal";

        public AnimeProposalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> UploadTempFileAsync(MultipartFormDataContent content, string type)
        {
            var response = await _httpClient.PostAsync($"{API_BASE_URL}/upload-temp?type={type}", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        public async Task<AnimeProposal?> CreateProposalAsync(AnimeProposalCreateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AnimeProposal>();
            }

            return null;
        }

        public async Task<IEnumerable<AnimeProposal>> GetAllProposalsAsync()
        {
            var response = await _httpClient.GetAsync($"{API_BASE_URL}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<AnimeProposal>>() ?? Enumerable.Empty<AnimeProposal>();
            }

            return Enumerable.Empty<AnimeProposal>();
        }

        public async Task<AnimeProposal?> GetProposalByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{API_BASE_URL}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AnimeProposal>();
            }

            return null;
        }

        public async Task<bool> ApproveProposalAsync(int proposalId)
        {
            var response = await _httpClient.PostAsync($"{API_BASE_URL}/{proposalId}/approve", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RejectProposalAsync(int proposalId, string feedback)
        {
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/{proposalId}/reject", feedback);
            return response.IsSuccessStatusCode;
        }
    }
}
