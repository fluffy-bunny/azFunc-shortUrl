using dotnetcore.keyvault.fetch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using webApp_urlshortener.Models;

namespace webApp_urlshortener
{
    public class CosmosConfigurationKeyVaultFetchStore : KeyVaultFetchStore<CosmosConfiguration>
    {
        public CosmosConfigurationKeyVaultFetchStore(
                   KeyVaultFetchStoreOptions<CosmosConfiguration> options, ILogger logger) :
                   base(options, logger)
        {
        }

        public CosmosConfigurationKeyVaultFetchStore(
            IOptions<KeyVaultFetchStoreOptions<CosmosConfiguration>> options, ILogger logger) :
            base(options, logger)
        {
        }
        public async Task<CosmosConfiguration> GetConfigurationAsync()
        {
            await SafeFetchAsync();
            return Value;
        }
        protected override void OnRefresh()
        {

        }
        async Task SafeFetchAsync()
        {
            await GetValueAsync();
        }
    }
}
