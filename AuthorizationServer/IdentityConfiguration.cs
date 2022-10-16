using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace AuthorizationServer;

public class IdentityConfiguration
{
    public static List<TestUser> TestUsers =>
        new()
        {
            new TestUser
            {
                SubjectId = "774a0068e9c04e97ba6a96f85f61c05c",
                Username = "long",
                Password = "123",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Long"),
                    new Claim(JwtClaimTypes.GivenName, "Long"),
                    new Claim(JwtClaimTypes.FamilyName, "Pham Bao"),
                    new Claim(JwtClaimTypes.WebSite, "https://google.com")
                }
            }
        };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("myApi.read"),
            new ApiScope("myApi.write")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
            new ApiResource("myApi")
            {
                Scopes = new List<string> { "myApi.read", "myApi.write" },
                ApiSecrets = new List<Secret> { new("supersecret".Sha256()) }
            }
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client
            {
                ClientId = "cwm.client",
                ClientName = "Client Credentials Client",
                AccessTokenType = AccessTokenType.Reference,
                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "myApi.read" },
                RedirectUris = {"https://localhost:44336/callback"},
                RequirePkce = false
            }
        };
}