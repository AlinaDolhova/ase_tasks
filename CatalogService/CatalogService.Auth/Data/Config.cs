using System.Collections.Generic;
using System.Collections.ObjectModel;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;

namespace CatalogService.Auth.Data
{
    public static class Config
    {
        public static readonly ReadOnlyCollection<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = "catalog",
                AllowedGrantTypes = new List<string> { GrantType.ResourceOwnerPassword, GrantType.AuthorizationCode  },
                RedirectUris = {"https://oauth.pstmn.io/v1/callback", "https://localhost:5003/swagger/oauth2-redirect.html", "https://localhost:5003/signin-oidc" },
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "catalog",  StandardScopes.OpenId, "role", StandardScopes.Profile },
                AllowAccessTokensViaBrowser = true,
                AllowPlainTextPkce = true,
                RequireConsent = false,                
                AllowedCorsOrigins = new List<string>
                {
                    "http://localhost:6405","http://localhost:17614", "https://localhost:5003"
                },
                AccessTokenLifetime = 86400,               
            }
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<ApiScope> ApiScopes = new List<ApiScope>()
        {
            new ApiScope {Name = "catalog", Enabled = true, UserClaims =
            {   JwtClaimTypes.Name,
                JwtClaimTypes.Email,
                JwtClaimTypes.Subject,
                JwtClaimTypes.Role,
                JwtClaimTypes.Address,
                JwtClaimTypes.Confirmation,
                JwtClaimTypes.EmailVerified,
                JwtClaimTypes.Id,
                JwtClaimTypes.Profile
            } }
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<ApiResource> ApiResources = new List<ApiResource>
        {
            new ApiResource
            {
                Name = "catalog",
                DisplayName = "Catalog API",
                Scopes = new List<string>
                {
                    StandardScopes.OpenId,
                    "role", 
                    "catalog"
                },
                UserClaims = { JwtClaimTypes.Role }
            }
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<IdentityResource> IdentityResources =
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("role", "User role(s)", new List<string> { "role" })
            }.AsReadOnly();
    }
}
