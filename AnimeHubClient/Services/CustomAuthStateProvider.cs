using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Buffers.Text;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Channels;
using static MudBlazor.CategoryTypes;
using System.Linq;

namespace AnimeHubClient.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly Timer? _tokenExpiryTimer;

        // Define an anonymous/unauthenticated user identity
        private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
            // Start timer to check every 60 seconds
            _tokenExpiryTimer = new Timer(async _ => await ValidateTokenAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = await GetValidatedIdentityAsync();
            if (identity.IsAuthenticated)
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        private async Task ValidateTokenAsync()
        {
            var identity = await GetValidatedIdentityAsync();

            // If the shared logic says we are "unauthenticated" but we haven't notified the UI yet
            if (!identity.IsAuthenticated)
            {
                var tokenInStorage = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(tokenInStorage))
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                    NotifyUserLogout();
                }
            }
        }

        private async Task<ClaimsIdentity> GetValidatedIdentityAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token)) return new ClaimsIdentity();

            try
            {
                var identity = CreateClaimsIdentityFromToken(token);
                var expClaim = identity.Claims.FirstOrDefault(c => c.Type == "exp");

                if (expClaim != null)
                {
                    var expiryTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                    if (expiryTime.UtcDateTime <= DateTime.UtcNow)
                    {
                        return new ClaimsIdentity(); // Expired
                    }
                }
                return identity; // Valid
            }
            catch
            {
                return new ClaimsIdentity(); // Invalid/Corrupt token
            }
        }

        public void Dispose() => _tokenExpiryTimer?.Dispose();

        // Called by AuthService upon successful login
        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(CreateClaimsIdentityFromToken(token));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            // Notifies the CascadingAuthenticationState(in App.razor) that the state has changed
            NotifyAuthenticationStateChanged(authState);
        }

        // Called by AuthService upon logout
        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(new AuthenticationState(Anonymous));

            // Clears the user's identity
            NotifyAuthenticationStateChanged(authState);
        }

        // Helper method to parse the JWT and build the ClaimsIdentity
        private static ClaimsIdentity CreateClaimsIdentityFromToken(string token)
        {
            // JWT parsing requires extracting the payload section (second part of token)
            var claims = ParseClaimsFromJwt(token);

            // The identity is built using the claims retrieved from the token
            // The authentication type is set to "JWT"
            return new ClaimsIdentity(claims, "jwt");
        }

        // Method based on standard JWT structure to extract claims
        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            // Pad the base64 string if necessary
            var jsonBytes = ParseBase64WithoutPadding(payload);

            // Deserialize the JSON payload
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            // API uses standard claim types, so we map them here:
            keyValuePairs!.TryGetValue(ClaimTypes.Role, out object? role);

            // Add all parsed claims
            claims.AddRange(keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));

            // The Role claim may use a shorter key ("role") in the token, 
            // but we ensure it's mapped to the correct ClaimTypes.Role if needed.
            if (role is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()!));
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string payload)
        {
            // Pad base64 string with '=' until it's a multiple of 4
            switch (payload.Length % 4)
            {
                case 2:
                    payload += "==";
                    break;

                case 3:
                    payload += "=";
                    break;
            }
            return Convert.FromBase64String(payload);
        }
    }
}
