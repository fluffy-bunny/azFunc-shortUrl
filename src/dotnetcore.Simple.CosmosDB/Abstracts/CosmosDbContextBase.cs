using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CosmosDB.Simple.Store.Configuration;
using CosmosDB.Simple.Store.Extensions;
using dotnetcore.urlshortener.Utils;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace CosmosDB.Simple.Store.Abstracts
{
    /// <summary>
    ///     Base class for Context that uses CosmosDb.
    /// </summary>
    public abstract class CosmosDbContextBase<T> : IDisposable
        where T : class
    {
        /// <summary>
        ///     CosmosDb Configuration Data
        /// </summary>
        protected readonly CosmosDbConfiguration<T> Configuration;

        /// <summary>
        ///     Logger
        /// </summary>
        protected readonly ILogger Logger;
        public CosmosClient CosmosClient { get; }
        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="connectionPolicy"></param>
        /// <param name="logger"></param>
        protected CosmosDbContextBase(
            IOptions<CosmosDbConfiguration<T>> settings,
            ConnectionPolicy connectionPolicy = null,
            ILogger logger = null)
        {
            Guard.ForNullOrDefault(settings.Value, nameof(settings));
            Guard.ForNullOrDefault(settings.Value.EndPointUrl, nameof(settings.Value.EndPointUrl));
            Guard.ForNullOrDefault(settings.Value.PrimaryKey, nameof(settings.Value.PrimaryKey));
            Guard.ForNullOrDefault(settings.Value.DatabaseName, nameof(settings.Value.EndPointUrl));
            Logger = logger;
            Configuration = settings.Value;

            var serviceEndPoint = new Uri(settings.Value.EndPointUrl);

            CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(serviceEndPoint.AbsoluteUri, settings.Value.PrimaryKey);
            CosmosClient = configurationBuilder.Build();
            DocumentClient = new DocumentClient(serviceEndPoint, settings.Value.PrimaryKey,
                connectionPolicy ?? ConnectionPolicy.Default);

            EnsureDatabaseCreated(Configuration.DatabaseName).Wait();
        }



        /// <summary>
        ///     CosmosDb Document Client.
        /// </summary>
        public DocumentClient DocumentClient { get; }

        /// <summary>
        ///     Instance of CosmosDb Database.
        /// </summary>
        protected Microsoft.Azure.Documents.Database Database { get; private set; }
        protected Microsoft.Azure.Cosmos.Database DatabaseV3 { get; private set; }
        /// <summary>
        ///     URL for CosmosDb Instance.
        /// </summary>
        protected Uri DatabaseUri { get; private set; }

        /// <summary>
        ///     Dispose of object resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets the Reserve Units (RU) per second for a given collection from configuration or the default value (1000).
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        protected int GetRUsFor(CollectionName collection)
        {
            return Configuration.Collection?.ReserveUnits
                   ?? dotnetcore.urlshortener.Utils.Constants.DefaultReserveUnits;
        }

        private async Task EnsureDatabaseCreated(string databaseName)
        {
            var dbNameToUse = Configuration.DatabaseName.GetValueOrDefault(databaseName);
            // Create new database
            DatabaseV3 = await CosmosClient.CreateDatabaseIfNotExistsAsync(dbNameToUse);

            DatabaseUri = UriFactory.CreateDatabaseUri(dbNameToUse);
            Logger?.LogDebug($"Database URI: {DatabaseUri}");
            Database = new Microsoft.Azure.Documents.Database { Id = databaseName };
            Logger?.LogDebug($"Database: {Database}");

            Logger.LogDebug($"Ensuring `{Database.Id}` exists...");
            var result = DocumentClient.CreateDatabaseIfNotExistsAsync(Database).Result;
            Logger.LogDebug($"{Database.Id} Creation Results: {result.StatusCode}");
            if (result.StatusCode.EqualsOne(HttpStatusCode.Created, HttpStatusCode.OK))
                Database = result.Resource;

        }

        /// <summary>
        ///     Dispose of object resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO : Dispose of any resources
            }
        }

        /// <summary>
        ///     Deconstructor
        /// </summary>
        ~CosmosDbContextBase()
        {
            Dispose(false);
        }
    }
}
