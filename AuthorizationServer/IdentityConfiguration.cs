using System.Security.Claims;
using IdentityModel;
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
                SubjectId = "1144",
                Username = "mukesh",
                Password = "mukesh",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Mukesh Murugan"),
                    new Claim(JwtClaimTypes.GivenName, "Mukesh"),
                    new Claim(JwtClaimTypes.FamilyName, "Murugan"),
                    new Claim(JwtClaimTypes.WebSite, "http://codewithmukesh.com")
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