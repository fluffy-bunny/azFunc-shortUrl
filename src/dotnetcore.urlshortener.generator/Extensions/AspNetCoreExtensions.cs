using dotnetcore.urlshortener.contracts;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetcore.urlshortener.generator.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddGuidUrlShortenerAlgorithm(this IServiceCollection services)
        {
            services.AddSingleton<IUrlShortenerAlgorithm, GuidUrlShortenerAlgorithm>(); // We must explicitly register Foo
            return services;
        }
    }
}
