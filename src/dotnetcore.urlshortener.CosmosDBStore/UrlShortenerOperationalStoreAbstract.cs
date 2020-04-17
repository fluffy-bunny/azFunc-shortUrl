using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CosmosDB.Simple.Store.Interfaces;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using dotnetcore.urlshortener.Utils;
using Microsoft.Azure.Cosmos;

namespace dotnetcore.urlshortener.CosmosDBStore
{
    public abstract class UrlShortenerOperationalStoreAbstract<T> : IUrlShortenerOperationalStoreBase
        where T : ShortUrlCosmosDocumentBase
    {
        private IUrlShortenerAlgorithm _urlShortenerAlgorithm;
        private ISimpleItemDbContext<T> _simpleItemDbContext;

        protected UrlShortenerOperationalStoreAbstract(
            IUrlShortenerAlgorithm urlShortenerAlgorithm,
            ISimpleItemDbContext<T> simpleItemDbContext)
        {
            _urlShortenerAlgorithm = urlShortenerAlgorithm;
            _simpleItemDbContext = simpleItemDbContext;

        }
        public async Task<(HttpStatusCode, ShortUrl)> UpsertShortUrlAsync(ShortUrl shortUrl)
        {
            Guard.ArgumentNotNull(nameof(shortUrl), shortUrl);
            Guard.ArgumentNotNullOrEmpty(nameof(shortUrl.LongUrl), shortUrl.LongUrl);
            Guard.ArgumentNotNull(nameof(shortUrl.Expiration), shortUrl.Expiration);
            if (string.IsNullOrWhiteSpace(shortUrl.Id))
            {
                shortUrl.Id = _urlShortenerAlgorithm.GenerateUniqueId();
            }

            var document = CreateDocument(shortUrl.ToShortUrlCosmosDocument());

            // var document = new ShortUrlCosmosDocument(shortUrl.ToShortUrlCosmosDocument());
            var response = await _simpleItemDbContext.UpsertItemAsync(document);
            return (response.StatusCode,shortUrl);
        }

        public abstract T CreateDocument(ShortUrlCosmosDocumentBase shortUrlCosmosDocumentBase);

        public async Task<ShortUrl> GetShortUrlAsync(string id)
        {

            Guard.ArgumentNotNullOrEmpty(nameof(id), id);
            var item = await _simpleItemDbContext.GetItemAsync(id);
            return item.ToShortUrl();
        }

        public async Task RemoveShortUrlAsync(string id)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(id), id);
            await _simpleItemDbContext.DeleteItemAsync(id);
        }

        async Task<TDoc> GetOneAsync<TDoc>(QueryDefinition query, PartitionKey partitionKey)
        {
            List<TDoc> results = new List<TDoc>();
            FeedIterator<TDoc> resultSetIterator = _simpleItemDbContext.Container.GetItemQueryIterator<TDoc>(query,
                requestOptions: new QueryRequestOptions() { PartitionKey = partitionKey });
            while (resultSetIterator.HasMoreResults)
            {
                Microsoft.Azure.Cosmos.FeedResponse<TDoc> response = await resultSetIterator.ReadNextAsync();
                results.AddRange(response);
                if (response.Diagnostics != null)
                {
                    Console.WriteLine($"\nQueryWithSqlParameters Diagnostics: {response.Diagnostics.ToString()}");
                }
                break;
            }
            var item = (TDoc)results.FirstOrDefault();
            return item;
        }
        public async Task<ShortUrl> GetShortUrlAsync(string id, string tenant)
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM ShortUrls f WHERE f.id = @id AND f.tenant = @tenant")
               .WithParameter("@id", id)
               .WithParameter("@tenant", tenant);
            var doc = await GetOneAsync<ShortUrlCosmosDocumentBase>(query, new PartitionKey(id));
            return doc.ToShortUrl();
        }

        public async Task RemoveShortUrlAsync(string id, string tenant)
        {
            var shortUrl = await GetShortUrlAsync(id);
            if (shortUrl != null && shortUrl.Tenant == tenant)
            {
                await RemoveShortUrlAsync(id);
            }
        }
    }
}