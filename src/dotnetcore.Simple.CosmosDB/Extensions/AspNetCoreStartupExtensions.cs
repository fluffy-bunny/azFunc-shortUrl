using System;
using System.Text;
using CosmosDB.Simple.Store.Abstracts;
using CosmosDB.Simple.Store.Configuration;
using CosmosDB.Simple.Store.DbContext;
using CosmosDB.Simple.Store.Interfaces;
using dotnetcore.Simple.CosmosDB.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CosmosDB.Simple.Store.Extensions
{
    public static class AspNetCoreStartupExtensions
    {
        public static IServiceCollection AddSimpleItemStore<T>(
           this IServiceCollection services,
           Action<CosmosDbConfiguration<T>> setupAction)
            where T : BaseItem
        {
            services.Configure(setupAction);
            services.AddTransient<ISimpleItemDbContext<T>, DocumentDBRepository<T>>();
            return services;
        }
    }
}
