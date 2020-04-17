using dotnetcore.urlshortener.contracts;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetcore.urlshortener.Extensions
{
    public static class AspNetCoreExtensions
    {

        public static IServiceCollection AddUrlShortenerService(this IServiceCollection services)
        {
            services.AddSingleton<UrlShortenerService>(); // We must explicitly register Foo
            services.AddSingleton<IUrlShortenerService>(x => x.GetRequiredService<UrlShortenerService>()); // Forward requests to Foo
            services.AddSingleton<IUrlShortenerEventSource<ShortenerEventArgs>>(x => x.GetRequiredService<UrlShortenerService>()); // Forward requests to Foo
            return services;
        }
        
    }
}
