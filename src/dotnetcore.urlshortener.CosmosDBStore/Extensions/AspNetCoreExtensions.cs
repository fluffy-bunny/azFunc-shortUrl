using System;
using System.Collections.Generic;
using System.Text;
using dotnetcore.urlshortener.contracts;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetcore.urlshortener.CosmosDBStore.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddCosmosDBUrlShortenerOperationalStore(this IServiceCollection services)
        {
            services.AddSingleton<IUrlShortenerOperationalStore, UrlShortenerOperationalStore>(); // We must explicitly register Foo
            services.AddSingleton<IExpiredUrlShortenerOperationalStore, ExpiredUrlShortenerOperationalStore>(); // We must explicitly register Foo
            return services;
        }
    }
}
