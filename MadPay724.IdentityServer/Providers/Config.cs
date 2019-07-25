using System.Collections.Generic;
using IdentityServer4.Models;

namespace MadPay724.IdentityServer.Providers
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("MadPay724Api","Customer Api For MadPay724")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("sevret".Sha256())
                    },
                    AllowedScopes = { "MadPay724Api" }
                }
            };
        }
    }
}
