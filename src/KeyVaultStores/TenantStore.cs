using dotnetcore.keyvault.fetch;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KeyVaultStores
{
    public class TenantStore : KeyVaultFetchStore<TenantConfiguration>, ITenantStore
    {
        private IHttpClientFactory _httpClientFactory;

        public TenantStore(
            IOptions<KeyVaultClientStoreOptions<TenantConfiguration>> options,
            IHttpClientFactory httpClientFactory,
            ILogger<TenantConfiguration> logger) : base(options, logger)
        {

            _httpClientFactory = httpClientFactory;
            TenantCache = new ConcurrentDictionary<string, ITenantServices>();
        }
        ConcurrentDictionary<string, ITenantServices> TenantCache { get; set; }
        private IDiscoveryCache _discoveryCache;
        async Task<IDiscoveryCache> GetDiscoveryCacheAsync()
        {
            if (_discoveryCache == null)
            {
                var value = await GetValueAsync();
                _discoveryCache = new DiscoveryCache(value.Authority, () => _httpClientFactory.CreateClient());
            }
            return _discoveryCache;
        }

        public async Task<ITenantServices> GetTenantAsync(string tenant)
        {
            ITenantServices tenantServices = null;
            if (TenantCache.TryGetValue(tenant, out tenantServices))
            {
                return tenantServices;
            }
            var value = await GetValueAsync();

            var tenantConfig = (from item in value.Tenants
                                where item.Name == tenant
                                select item).FirstOrDefault();
            if (tenantConfig == null)
            {
                return null;
            }
            var discoveryCache = await GetDiscoveryCacheAsync();
            tenantServices = new TenantServices(_httpClientFactory, tenantConfig, discoveryCache);
            TenantCache[tenant] = tenantServices;

            return tenantServices;
        }

        protected override void OnRefresh()
        {
            TenantCache.Clear();
        }
    }
}
