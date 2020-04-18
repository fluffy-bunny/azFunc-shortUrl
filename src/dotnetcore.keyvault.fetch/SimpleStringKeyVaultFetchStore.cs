using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnetcore.keyvault.fetch
{
    public class SimpleStringKeyVaultFetchStore : KeyVaultFetchStore<string>
    {
        public SimpleStringKeyVaultFetchStore(
                  KeyVaultFetchStoreOptions<string> options, ILogger logger) :
                  base(options, logger)
        {
        }

        public SimpleStringKeyVaultFetchStore(
            IOptions<KeyVaultFetchStoreOptions<string>> options, ILogger logger) :
            base(options, logger)
        {
        }
        public async Task<string> GetStringValueAsync()
        {
            await SafeFetchAsync();
            return Value;
        }
        protected override void OnRefresh()
        {

        }
        protected override string DeserializeValue(string raw)
        {
            return raw;
        }
        async Task SafeFetchAsync()
        {
            await GetValueAsync();
        }
    }
}
