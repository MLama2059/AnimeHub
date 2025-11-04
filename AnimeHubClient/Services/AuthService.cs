using AnimeHub.Shared.Models.Dtos.User;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AnimeHubClient.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        }

        public async Task<bool> RegisterAsync(RegistrationRequestDto registerDto)
        {
            var content = JsonSerializer.Serialize(registerDto);
            var bodyContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/User/Registration", bodyContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<AuthenticationResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            var content = JsonSerializer.Serialize(loginDto);
            var bodyContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/User/Login", bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = new AuthenticationResponseDto();
            
            if (!response.IsSuccessStatusCode)
            {
                // API returned 401 Unauthorized, so token is null.
                result.IsAuthSuccessful = false;
                result.ErrorMessage = "Invalid Login Attempt or Server Error.";
                return result;
            }

            // API returns { "token": "..." }. We need to extract it.
            var loginResult = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var token = loginResult!["token"];

            // 1. Store the token securely in local storage
            await _localStorage.SetItemAsync("authToken", token);

            // 2. Notify the framework about the successful login
            _authStateProvider.NotifyUserAuthentication(token);

            // 3. Set the Authorization header for all subsequent API requests
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            result.IsAuthSuccessful = true;
            result.Token = token;
            // Role and other details will be extracted by CustomAuthStateProvider later.

            return result;
        }

        public async Task Logout()
        {
            // 1. Remove the token from local storage
            await _localStorage.RemoveItemAsync("authToken");

            // 2. Clear the default HttpClient authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // 3. Notify the framework about the logout
            _authStateProvider.NotifyUserLogout();
        }

    }
}
