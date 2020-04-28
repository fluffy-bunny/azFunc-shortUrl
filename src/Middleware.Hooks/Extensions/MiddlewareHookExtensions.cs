using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Middleware.Hooks.Extensions
{
    public static class MiddlewareHookExtensions
    {
        public static IApplicationBuilder UseEndpointWatcherHookMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EndpointWatcherHookMiddleware>();
        }

        public static IServiceCollection AddScopedRequestHook<TImplementation>(this IServiceCollection services)
            where TImplementation : class, IScopedRequestHook
        {
            services.AddScoped<IScopedRequestHook, TImplementation>();
            return services;
        }
    }
}