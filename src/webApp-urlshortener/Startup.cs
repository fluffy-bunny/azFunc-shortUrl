using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CosmosDB.Simple.Store.Configuration;
using CosmosDB.Simple.Store.Extensions;
using dotnetcore.keyvault.fetch;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using dotnetcore.urlshortener.CosmosDBStore.Extensions;
using dotnetcore.urlshortener.Extensions;
using dotnetcore.urlshortener.generator.Extensions;
using dotnetcore.urlshortener.InMemoryStore;
using IdentityModel.Client;
using KeyVaultStores.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using webApp_urlshortener.Controllers;
using webApp_urlshortener.Models;
using webApp_urlshortener.Models.jwt_validation;

namespace webApp_urlshortener
{
    public partial class KeyVaultConfiguration
    {
        [JsonProperty("expirationSeconds")]
        public int ExpirationSeconds { get; set; }

        [JsonProperty("keyVaultName")]
        public string KeyVaultName { get; set; }

        [JsonProperty("secretName")]
        public string SecretName { get; set; }
    }

    public class Startup
    {

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment _hostingEnvironment;
        private ILogger _logger;
        private Exception _exConfigureServices;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _logger = new LoggerBuffered(LogLevel.Debug);
            _logger.LogInformation($"Create Startup {hostingEnvironment.ApplicationName} - {hostingEnvironment.EnvironmentName}");

            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;

        }
        string SafeFetchSettings(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                value = Configuration[key];
            }
            return value;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                _logger.LogInformation("ConfigureServices");
                var nameKeyVault = "kv-shorturl2";
                var snCosmosConfigTemplate = "cosmosConfigTemplateProduction";
                var snCosmosPrimaryKey = "cosmosPrimaryKeyProduction";

//                var snCosmosConfigTemplate = "cosmosConfigTemplateEmulator";
//                var snCosmosPrimaryKey = "cosmosPrimaryKeyEmulator";

                var cosmosPrimaryKeyVaultFetchStore = new SimpleStringKeyVaultFetchStore(
                                   new KeyVaultFetchStoreOptions<string>()
                                   {
                                       ExpirationSeconds = 3600,
                                       KeyVaultName = nameKeyVault,
                                       SecretName = snCosmosPrimaryKey
                                   }, _logger);
                var primaryKey = cosmosPrimaryKeyVaultFetchStore.GetStringValueAsync().GetAwaiter().GetResult();

                var cosmosKeyVaultOptions = new KeyVaultFetchStoreOptions<CosmosConfiguration>()
                {
                    ExpirationSeconds = 3600,
                    KeyVaultName = nameKeyVault,
                    SecretName = snCosmosConfigTemplate
                };
                var cosmosConfigurationKeyVaultFetchStore = new CosmosConfigurationKeyVaultFetchStore(cosmosKeyVaultOptions, _logger);
                var cosmosConfiguration = cosmosConfigurationKeyVaultFetchStore.GetConfigurationAsync().GetAwaiter().GetResult();

                cosmosConfiguration.PrimaryKey = cosmosConfiguration.PrimaryKey.Replace("{{primaryKey}}", primaryKey);
                cosmosConfiguration.PrimaryConnectionString = cosmosConfiguration.PrimaryConnectionString.Replace("{{primaryKey}}", primaryKey);

                var jwtValidateSettingsKeyVaultOptions = new KeyVaultFetchStoreOptions<JwtValidation>()
                {
                    ExpirationSeconds = 3600,
                    KeyVaultName = nameKeyVault,
                    SecretName = "jwtValidateSettings"
                };
                var JwtValidationKeyVaultFetchStore = new JwtValidationKeyVaultFetchStore(jwtValidateSettingsKeyVaultOptions, _logger);
                var jwtValidation = JwtValidationKeyVaultFetchStore.GetConfigurationAsync().GetAwaiter().GetResult();


                _logger.LogInformation($"primaryKey:{!string.IsNullOrEmpty(primaryKey)}");
                _logger.LogInformation($"cosmosEndpointUri:{cosmosConfiguration.Uri}");

                services.AddHttpClient();
                services.AddControllers();

                _logger.LogInformation($"jwtValidateSettings:{jwtValidation != null} - JsonConvert.DeserializeObject");
                var tok = jwtValidation.ToTokenValidationParameters();
                services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = jwtValidation.Authority;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = tok;
                    });

                services.AddUrlShortenerService();
                services.AddGuidUrlShortenerAlgorithm();

                // services.AddInMemoryUrlShortenerOperationalStore();
                services.AddCosmosDBUrlShortenerOperationalStore();

                TenantConfiguration tenantConfiguration = null;
                try
                {
                    _logger.LogInformation($"SafeFetchSettings(\"azFuncShorturlClientCredentials\")");
                    var creds = SafeFetchSettings("azFuncShorturlClientCredentials");
                    _logger.LogInformation($"azFuncShorturlClientCredentials:{!string.IsNullOrEmpty(creds)} - base64");
                    creds = Base64Decode(creds);
                    _logger.LogInformation($"azFuncShorturlClientCredentials:{!string.IsNullOrEmpty(creds)} - decoded");
                    tenantConfiguration = JsonConvert.DeserializeObject<TenantConfiguration>(creds);
                    _logger.LogInformation($"tenantConfiguration ok");
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"tenantConfiguration not ok, setting to null");
                    tenantConfiguration = null;
                }

                services.AddKeyVaultTenantStore(options =>
                {
                    options.ExpirationSeconds = 3600;
                    options.KeyVaultName = nameKeyVault;
                    options.SecretName = "azFuncShorturlClientCredentials";
                    options.Value = tenantConfiguration; // ok if null.  If it is not null we don't go to key vault at all
                });

                services.AddSimpleItemStore<ShortUrlCosmosDocument>(options =>
                {
                    options.EndPointUrl = cosmosConfiguration.Uri;
                    options.PrimaryKey = primaryKey;
                    options.DatabaseName = "shorturl";
                    options.Collection = new Collection()
                    {
                        CollectionName = "shorturl",
                        ReserveUnits = 400
                    };

                });
                services.AddSimpleItemStore<ExpiredShortUrlCosmosDocument>(options =>
                {
                    options.EndPointUrl = cosmosConfiguration.Uri;
                    options.PrimaryKey = primaryKey;
                    options.DatabaseName = "shorturl";
                    options.Collection = new Collection()
                    {
                        CollectionName = "expired-shorturl",
                        ReserveUnits = 400
                    };

                });
            }
            catch(Exception ex)
            {
                _exConfigureServices = ex;
                // defer throw, because we need to log in the Configure() function.
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IServiceProvider serviceProvider,
            ILogger<Startup> logger)
        {
            try
            {
                (_logger as LoggerBuffered).CopyToLogger(logger);
                _logger = logger;
                if (_exConfigureServices != null)
                {
                    // defered throw.
                    throw _exConfigureServices;
                }
                _logger.LogInformation("Configure");

                var section = Configuration.GetSection("InMemoryUrlShortenerExpiryOperationalStore");
                var model = new InMemoryUrlShortenerConfigurationModel();

                section.Bind(model);
                var expiredUrlShortenerOperationalStore = serviceProvider.GetRequiredService<IExpiredUrlShortenerOperationalStore>();


                foreach (var record in model.Records)
                {
                    var su = expiredUrlShortenerOperationalStore.UpsertShortUrlAsync(new ShortUrl
                    {
                        Id = record.ExpiredRedirectKey,
                        Tenant = record.Tenant,
                        LongUrl = record.ExpiredRedirectUrl,
                        Expiration = DateTime.UtcNow.AddYears(10)
                    }).GetAwaiter().GetResult();

                }

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }


                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
    }
}
