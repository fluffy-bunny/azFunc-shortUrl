using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnetcore.keyvault.fetch
{
    public abstract class KeyVaultFetchStore<T>
       where T : class
    {

        private KeyVaultFetchStoreOptions<T> _options;
        private ILogger _logger;
        private T _value;
        private DateTime _lastRead;
        protected T Value
        {
            get { return _value; }
        }
        public KeyVaultFetchStore(
            IOptions<KeyVaultFetchStoreOptions<T>> options,
            ILogger logger) : this(options.Value, logger)
        {
        }
        public KeyVaultFetchStore(
            KeyVaultFetchStoreOptions<T> options,
            ILogger logger)
        {
            _options = options;
            _logger = logger;
        }
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        protected string Decode(string raw)
        {
            if (!raw.StartsWith('{'))
            {
                var jsonDecoded = Base64Decode(raw);
                return jsonDecoded;
            }
            return raw;
        }
        protected virtual T DeserializeValue(string raw)
        {
            T value;
            var decoded = Decode(raw);
            value = JsonConvert.DeserializeObject<T>(decoded);
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

                var rawValue = secret.Value;
                if (!string.IsNullOrWhiteSpace(_options.Schema))
                {
                    rawValue = Decode(rawValue);
                    // load schema
                    JSchema schema = JSchema.Parse(_options.Schema);
                    JToken json = JToken.Parse(rawValue);

                    // validate json
                    IList<ValidationError> errors;
                    bool valid = json.IsValid(schema, out errors);

                    if (!valid)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var error in errors)
                        {
                            stringBuilder.Append($"{error.Message}\n");
                        }
                        throw new Exception(stringBuilder.ToString());

                    }
                }


                var value = DeserializeValue(rawValue);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
        async Task<(bool fresh, T value)> FetchAsync()
        {
            bool fresh = false;
            if (_value == null)
            {
                _value = await HardFetchAsync();
                fresh = true;
                _lastRead = DateTime.UtcNow;
                OnRefresh();
            }
            else
            {
                System.TimeSpan diff = DateTime.UtcNow.Subtract(_lastRead);
                if (diff.TotalSeconds > _options.ExpirationSeconds)
                {
                    _value = await HardFetchAsync();
                    fresh = true;
                    _lastRead = DateTime.UtcNow;
                    OnRefresh();
                }
            }
            return (fresh, _value);

        }
        public async Task<(bool fresh, T value)> GetValueAsync()
        {
            return await FetchAsync();
        }
    }
}
