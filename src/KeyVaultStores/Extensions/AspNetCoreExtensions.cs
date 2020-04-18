﻿using System;
using System.Collections.Generic;
using System.Text;
using dotnetcore.keyvault.fetch;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using Microsoft.Extensions.DependencyInjection;

namespace KeyVaultStores.Extensions
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddKeyVaultTenantStore(
          this IServiceCollection services,
          Action<KeyVaultFetchStoreOptions<TenantConfiguration>> setupAction)
        {
            services.Configure(setupAction);
            services.AddSingleton<ITenantStore, TenantStore>(); // We must explicitly register Foo
            return services;
        }
    }
}
