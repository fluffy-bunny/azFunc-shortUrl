using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using IdentityModel.Client;
 using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace KeyVaultStores
{
    public class TenantServices : ITenantServices
    {
        private readonly object _accessLock = new object();
        class TokenRecord
        {
            public TokenResponse TokenResponse { get; set; }
            public Credentials ClientCredential { get; set; }
            public DateTime Expiration { get; set; }
        }
        private  IHttpClientFactory _httpClientFactory;
        private Tenant _tenant;
        private IDiscoveryCache _discoveryCache;

        TokenRecord CurrentTokenRecord { get; set; }

        public TenantServices(
           IHttpClientFactory httpClientFactory,
           Tenant tenant,
           IDiscoveryCache discoveryCache)
        {
            _httpClientFactory = httpClientFactory;
            _tenant = tenant;
            _discoveryCache = discoveryCache;
        }
        public async Task<TokenResponse> GetAccessTokenAsync()
        {
            lock (_accessLock)
            {
                var nowUtc = DateTime.UtcNow;
             
                if (CurrentTokenRecord != null)
                {
                    var remaining = (CurrentTokenRecord.Expiration - nowUtc).TotalSeconds;
                    if (remaining <= 0)
                    {
                        CurrentTokenRecord = null;
                    }
                    else
                    {
                        return CurrentTokenRecord.TokenResponse;
                    }
                }

            }
            return await RefreshAccessTokenAsync();
        }

        public async Task<TokenResponse> RefreshAccessTokenAsync()
        {
            var document = await _discoveryCache.GetAsync();

            var tokenClient = _httpClientFactory.CreateClient("tokenClient");

            var tokenResonse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = document.TokenEndpoint,
                ClientId = _tenant.Credentials.ClientId,
                ClientSecret = _tenant.Credentials.ClientSecret,
            });
            lock (_accessLock)
            {
                CurrentTokenRecord = new TokenRecord
                {
                    TokenResponse = tokenResonse,
                    ClientCredential = _tenant.Credentials,
                    Expiration = DateTime.UtcNow.AddSeconds(tokenResonse.ExpiresIn - 300)  // give a 5 minute skew.
                };
                return tokenResonse;
            }
        }
    }
}
