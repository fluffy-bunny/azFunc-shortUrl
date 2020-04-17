using CosmosDB.Simple.Store.Configuration;
using CosmosDB.Simple.Store.Extensions;
using dotnetcore.urlshortener.contracts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using TestServerFixture;
using UrlShortenerApp;


namespace XUnitTest_Shortener
{
    public class MyTestServerFixture : TestServerFixture<Startup>
    {
        protected override string RelativePathToHostProject => @"../../../../UrlShortenerApp";
        protected override void ConfigureAppConfiguration(WebHostBuilderContext hostingContext,
            IConfigurationBuilder config)
        {
            var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
            Program.LoadConfigurations(config, environmentName);
            //   config.AddJsonFile($"appsettings.TestServer.json", optional: false);
        }
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

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
        }
    }
}
