using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CosmosDB.Simple.Store.Abstracts;
using CosmosDB.Simple.Store.Configuration;
using CosmosDB.Simple.Store.Interfaces;
using dotnetcore.Simple.CosmosDB.Models;
using dotnetcore.urlshortener.Utils;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CosmosDB.Simple.Store.DbContext
{

    public class DocumentDBRepository<T> : 
        CosmosDbContextBase<T>, 
        ISimpleItemDbContext<T> 
        where T : BaseItem
    {
        public Uri DocumentCollectionUri { get; private set; }
        private Container _container;
        private ILogger<DocumentDBRepository<T>> _logger;

        public Container Container
        {
            get
            {
                if(_container == null)
                {
                    _container = CosmosClient.GetContainer(Database.Id, Configuration.Collection.CollectionName);
                }
                return _container;
            }
        }
    

        public DocumentDBRepository(
            IOptions<CosmosDbConfiguration<T>> settings,
            ConnectionPolicy connectionPolicy = null,
            ILogger<DocumentDBRepository<T>> logger = null) :
            base(settings, connectionPolicy, logger)
        {
            _logger = logger;
            Guard.ForNullOrDefault(settings.Value, nameof(settings));
            
            SetupAsync().Wait();
        }
        private async Task SetupAsync()
        {
            DocumentCollectionUri =
                UriFactory.CreateDocumentCollectionUri(Database.Id, Configuration.Collection.CollectionName);
            Logger?.LogDebug($"Persisted Grants URI: {DocumentCollectionUri}");
            await CreateContainerIfNotExistsAsync();

        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                var partitionKey = new Microsoft.Azure.Cosmos.PartitionKey(id);

                ItemResponse<T> response = await Container.ReadItemAsync<T>(
                      partitionKey: partitionKey,
                      id: id);
                if (response.StatusCode.IsSuccess())
                {
                    T item = (T)response;
                    // Read the same item but as a stream.
                    using (ResponseMessage responseMessage = await Container.ReadItemStreamAsync(
                        partitionKey: partitionKey,
                        id: id))
                    {
                        // Item stream operations do not throw exceptions for better performance
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            T streamResponse = responseMessage.Content.FromStream<T>();
                            return streamResponse;
                        }
                        else
                        {
                            _logger.LogError($"Read item from stream failed. Status code: {responseMessage.StatusCode} Message: {responseMessage.ErrorMessage}");
                        }
                    }

                }

                return null;
            }
           
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
         
        public async Task<ItemResponse<T>> UpsertItemAsync(T item)
        {
            var response = await Container.UpsertItemAsync(item, new Microsoft.Azure.Cosmos.PartitionKey(item.Id));
            return response;
        }

        public async Task<Document> ReplaceItemAsync(string id, T item)
        {
            return await DocumentClient.ReplaceDocumentAsync(DocumentCollectionUri, item);
        }

        public async Task DeleteItemAsync(string id)
        {
            var item = DocumentClient.CreateDocumentQuery(DocumentCollectionUri)
                    .Where(d => d.Id == id)
                    .AsEnumerable()
                    .FirstOrDefault();

            await DocumentClient.DeleteDocumentAsync(item.SelfLink);
        }

        private async Task CreateContainerIfNotExistsAsync()
        {
            try
            {
                // Create new container
                var containerProperties = new ContainerProperties
                {
                    Id = Configuration.Collection.CollectionName,
                    PartitionKeyPath = "/id",
                    DefaultTimeToLive = -1
                };

                await DatabaseV3.CreateContainerIfNotExistsAsync(containerProperties);

                await DocumentClient.ReadDocumentCollectionAsync(DocumentCollectionUri);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await DocumentClient.CreateDocumentCollectionAsync(
                        DatabaseUri,
                        new DocumentCollection { Id = Configuration.Collection.CollectionName },
                        new Microsoft.Azure.Documents.Client.RequestOptions
                        {
                            OfferThroughput = Configuration.Collection.ReserveUnits,
                        });
                }
                else
                {
                    throw;
                }
            }
        }
       
    }
}
