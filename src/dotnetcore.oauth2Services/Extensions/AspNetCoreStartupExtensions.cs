using dotnetcore.oauth2Services.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace dotnetcore.oauth2Services.Extensions
{
    public static class AspNetCoreStartupExtensions
    {
        public static IServiceCollection AddClientCredentialsManager(
           this IServiceCollection services,
           Action<ClientCredentials> setupAction)
        {
            services.Configure(setupAction);
            services.AddSingleton<IClientCredentialsManager, ClientCredentialsManager>();
            return services;
        }
    }
}
