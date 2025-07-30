using Gateway.Models;

namespace Gateway.Clients
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _http;
        public AuthServiceClient(HttpClient http) => _http = http;

        public async Task<ValidateTokenResponse?> ValidateTokenAsync(string jwt)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "auth/validate-token");
            req.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<ValidateTokenResponse>();
        }
    }
}
