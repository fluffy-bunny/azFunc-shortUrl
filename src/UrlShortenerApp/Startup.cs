using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDB.Simple.Store.Configuration;
using CosmosDB.Simple.Store.Extensions;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using dotnetcore.urlshortener.CosmosDBStore.Extensions;
using dotnetcore.urlshortener.Extensions;
using dotnetcore.urlshortener.generator.Extensions;
using dotnetcore.urlshortener.InMemoryStore;
using dotnetcore.urlshortener.InMemoryStore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UrlShortenerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddUrlShortenerService();
            services.AddGuidUrlShortenerAlgorithm();
            
           // services.AddInMemoryUrlShortenerOperationalStore();
            services.AddCosmosDBUrlShortenerOperationalStore();
            if (_env.IsDevelopment())
            {
                services.AddSimpleItemStore<ShortUrlCosmosDocument>(options =>
                {
                    options.EndPointUrl = "https://localhost:8081";
                    options.PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                    options.DatabaseName = "shorturl";
                    options.Collection = new Collection()
                    {
                        CollectionName = "shorturl",
                        ReserveUnits = 400
                    };

                });
                services.AddSimpleItemStore<ExpiredShortUrlCosmosDocument>(options =>
                {
                    options.EndPointUrl = "https://localhost:8081";
                    options.PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                    options.DatabaseName = "shorturl";
                    options.Collection = new Collection()
                    {
                        CollectionName = "expired-shorturl",
                        ReserveUnits = 400
                    };

                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IExpiredUrlShortenerOperationalStore expiredUrlShortenerOperationalStore)
        {

            var section = Configuration.GetSection("InMemoryUrlShortenerExpiryOperationalStore");
            var model = new InMemoryUrlShortenerConfigurationModel();

            section.Bind(model);
            
            foreach (var record in model.Records)
            {
                var su = expiredUrlShortenerOperationalStore.UpsertShortUrlAsync(new ShortUrl
                {
                    Id = record.ExpiredRedirectKey,
                    LongUrl = record.ExpiredRedirectUrl,
                    Expiration = DateTime.UtcNow.AddYears(10)
                }).GetAwaiter().GetResult();

            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
