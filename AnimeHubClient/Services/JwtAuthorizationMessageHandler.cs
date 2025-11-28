using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace AnimeHubClient.Services
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public JwtAuthorizationMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get the token from local storage
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Attach it to the Authorization header if it exists
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
