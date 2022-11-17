using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;

namespace CatalogService.Auth.Data
{
    public static class Config
    {
        public static List<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = "catalog",
                AllowedGrantTypes = new List<string> { GrantType.ClientCredentials, GrantType.AuthorizationCode },
                RedirectUris = { "https://localhost:5002/home" },
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "catalog",  StandardScopes.OpenId, "role", StandardScopes.Profile },
                
                AllowedCorsOrigins = new List<string>
                {
                    "https://localhost:5001",
                },
                AccessTokenLifetime = 86400
            }
        };

        public static List<ApiResource> ApiResources = new List<ApiResource>
        {
            new ApiResource
            {
                Name = "catalog",
                DisplayName = "Catalog API",
                Scopes = new List<string>
                {
                    StandardScopes.OpenId,
                    "role"
                },
                UserClaims = { JwtClaimTypes.Role }
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("role", "User role(s)", new List<string> { "role" })
            };
    }
}
