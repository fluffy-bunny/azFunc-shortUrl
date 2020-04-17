using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CosmosDB.Simple.Store.Configuration;
using CosmosDB.Simple.Store.Extensions;
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
        private IWebHostEnvironment _env;


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;

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
            services.AddSingleton(provider =>
            {
                var service = provider.GetRequiredService<ILogger<StartupLogger>>();
                return new StartupLogger(service);
            });
            var logger = services.BuildServiceProvider().GetRequiredService<StartupLogger>();
            logger.Log("Startup.ConfigureServices called");
            try
            {
                services.AddHttpClient();

                services.AddControllers();
                var jwt_validate_settings = SafeFetchSettings("jwt-validate-settings");
                logger.Log($"jwt-validate-settings:{!string.IsNullOrEmpty(jwt_validate_settings)} - base64");

                jwt_validate_settings = Base64Decode(jwt_validate_settings);
                logger.Log($"jwt-validate-settings:{!string.IsNullOrEmpty(jwt_validate_settings)} - decoded");

                var authenictation = JsonConvert.DeserializeObject<Authentication>(jwt_validate_settings);
                logger.Log($"jwt-validate-settings:{authenictation != null} - JsonConvert.DeserializeObject");
                var tok = authenictation.ToTokenValidationParameters();
                services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = authenictation.JwtValidation.Authority;
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = tok;
                    });

                services.AddUrlShortenerService();
                services.AddGuidUrlShortenerAlgorithm();

                // services.AddInMemoryUrlShortenerOperationalStore();
                services.AddCosmosDBUrlShortenerOperationalStore();

                // wellknown CosmosDB emulator for local development

                string primaryKey = SafeFetchSettings("azFunc-shorturl-cosmos-primarykey");
                string cosmosEndpointUri = SafeFetchSettings("azFunc-shorturl-cosmos-uri");
                primaryKey = primaryKey ?? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                cosmosEndpointUri = cosmosEndpointUri ?? "https://localhost:8081";

                logger.Log($"primaryKey:{!string.IsNullOrEmpty(primaryKey)}");
                logger.Log($"cosmosEndpointUri:{cosmosEndpointUri}");


                TenantConfiguration tenantConfiguration = null;
                try
                {
                    logger.Log($"SafeFetchSettings(\"azFunc-shorturl-client-credentials\")");
                    var creds = SafeFetchSettings("azFunc-shorturl-client-credentials");
                    logger.Log($"azFunc-shorturl-client-credentials:{!string.IsNullOrEmpty(creds)} - base64");
                    creds = Base64Decode(creds);
                    logger.Log($"azFunc-shorturl-client-credentials:{!string.IsNullOrEmpty(creds)} - decoded");
                    tenantConfiguration = JsonConvert.DeserializeObject<TenantConfiguration>(creds);
                    logger.Log($"tenantConfiguration ok");
                }
                catch (Exception e)
                {
                    logger.Log($"tenantConfiguration not ok, setting to null");
                    tenantConfiguration = null;
                }
                logger.Log($"SafeFetchSettings(\"keyvault-config\")");
                string settingString = SafeFetchSettings("keyvault-config");
                logger.Log($"keyvault-config:{!string.IsNullOrEmpty(settingString)} - base64");
                settingString = Base64Decode(settingString);
                logger.Log($"keyvault-config:{!string.IsNullOrEmpty(settingString)} - decoded");

                var keyVaultConfiguration = JsonConvert.DeserializeObject<KeyVaultConfiguration>(settingString);
                logger.Log($"keyVaultConfiguration ok");
                services.AddKeyValutTenantStore(options =>
                {
                    options.ExpirationSeconds = keyVaultConfiguration.ExpirationSeconds;
                    options.KeyVaultName = keyVaultConfiguration.KeyVaultName;
                    options.SecretName = keyVaultConfiguration.SecretName;
                    options.Value = tenantConfiguration; // ok if null.  If it is not null we don't go to key vault at all
                });

                services.AddSimpleItemStore<ShortUrlCosmosDocument>(options =>
                {
                    options.EndPointUrl = cosmosEndpointUri;
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
                    options.EndPointUrl = cosmosEndpointUri;
                    options.PrimaryKey = primaryKey;
                    options.DatabaseName = "shorturl";
                    options.Collection = new Collection()
                    {
                        CollectionName = "expired-shorturl",
                        ReserveUnits = 400
                    };

                });
            }
            catch (Exception ex)
            {
                logger.Log(ex.Message);
                throw;
            }


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(StartupLogger logger, IApplicationBuilder app, IWebHostEnvironment env,
            IExpiredUrlShortenerOperationalStore expiredUrlShortenerOperationalStore)
        {
            try
            {
                logger.Log("Startup Configure...");
                var section = Configuration.GetSection("InMemoryUrlShortenerExpiryOperationalStore");
                var model = new InMemoryUrlShortenerConfigurationModel();

                section.Bind(model);

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
                logger.Log($"Startup Configure {ex.Message}");
            }
        }
    }
}
