using dotnetcore.keyvault.fetch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using webApp_urlshortener.Models.jwt_validation;

namespace webApp_urlshortener
{
    public class JwtValidationKeyVaultFetchStore : KeyVaultFetchStore<JwtValidation>
    {
        public JwtValidationKeyVaultFetchStore(
                   KeyVaultFetchStoreOptions<JwtValidation> options, ILogger logger) :
                   base(options, logger)
        {
        }

        public JwtValidationKeyVaultFetchStore(
            IOptions<KeyVaultFetchStoreOptions<JwtValidation>> options, ILogger logger) :
            base(options, logger)
        {
        }
        public async Task<JwtValidation> GetConfigurationAsync()
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
