using IdentityModel.Client;

namespace WebClient.Services
{
    public class TokenService : ITokenService
    {
        private DiscoveryDocumentResponse DiscDocument { get; set; }

        public TokenService()
        {
            using var client = new HttpClient();
            DiscDocument = client.GetDiscoveryDocumentAsync("https://localhost:44322/.well-known/openid-configuration").Result;
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            using var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = DiscDocument.TokenEndpoint,
                ClientId = "cwm.client",
                Scope = scope,
                ClientSecret = "secret"
            });

            if (tokenResponse.IsError)
            {
                throw new Exception("Token Error");
            }

            return tokenResponse;
        }
    }
}
