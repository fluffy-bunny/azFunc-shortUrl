using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnetcore.keyvault.fetch
{
    public abstract class KeyVaultFetchStore<T>
        where T : class
    {
        private KeyVaultClientStoreOptions<T> _options;
        private ILogger _logger;
        private T _value;
        private DateTime _lastRead;

        public KeyVaultFetchStore(
            IOptions<KeyVaultClientStoreOptions<T>> options,
            ILogger logger)
        {
            _options = options.Value;
            _logger = logger;
        }
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        protected virtual T DeserializeValue(string raw)
        {
            T value;
            if (!raw.StartsWith('{'))
            {
                var jsonDecoded = Base64Decode(raw);
                value = JsonConvert.DeserializeObject<T>(jsonDecoded);
            }
            else
            {
                value = JsonConvert.DeserializeObject<T>(raw);
            }
            return value;
        }
        protected abstract void OnRefresh();

        async Task<T> HardFetchAsync()
        {
            try
            {
                if (_options.Value != null)
                {
                    return _options.Value;
                }
                /* The next four lines of code show you how to use AppAuthentication library to fetch secrets from your key vault */
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                                        new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var secret = await keyVaultClient.GetSecretAsync(_options.KeyVaultSecretUrl);
                var value = DeserializeValue(secret.Value);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
        async Task FetchAsync()
        {
            if (_value == null)
            {
                _value = await HardFetchAsync();
                _lastRead = DateTime.UtcNow;
                OnRefresh();
            }
            else
            {
                System.TimeSpan diff = DateTime.UtcNow.Subtract(_lastRead);
                if (diff.TotalSeconds > _options.ExpirationSeconds)
                {
                    _value = await HardFetchAsync();
                    _lastRead = DateTime.UtcNow;
                    OnRefresh();
                }
            }
        }
        public async Task<T> GetValueAsync()
        {
            await FetchAsync();
            return _value;
        }
    }
}
