using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.Utils;

namespace dotnetcore.urlshortener.InMemoryStore
{
    public class InMemoryUrlShortenerOperationalStoreBase : IUrlShortenerOperationalStoreBase
    {
        private Dictionary<string, ShortUrl> _database;
        private IUrlShortenerAlgorithm _urlShortenerAlgorithm;

        public InMemoryUrlShortenerOperationalStoreBase(IUrlShortenerAlgorithm urlShortenerAlgorithm)
        {
            _urlShortenerAlgorithm = urlShortenerAlgorithm;
            _database = new Dictionary<string, ShortUrl>();
        }
        public async Task<(HttpStatusCode, ShortUrl)> UpsertShortUrlAsync(ShortUrl shortUrl)
        {
            Guard.ArgumentNotNull(nameof(shortUrl), shortUrl);
            Guard.ArgumentNotNullOrEmpty(nameof(shortUrl.LongUrl), shortUrl.LongUrl);
            Guard.ArgumentNotNull(nameof(shortUrl.Expiration), shortUrl.Expiration);

            shortUrl.Id = _urlShortenerAlgorithm.GenerateUniqueId();
            _database.Add(shortUrl.Id, shortUrl);
            return (HttpStatusCode.OK,shortUrl);
        }

        public async Task<ShortUrl> GetShortUrlAsync(string id)
        {

            Guard.ArgumentNotNullOrEmpty(nameof(id), id);

            if (_database.ContainsKey(id))
            {
                var record = _database[id];
                if (record.Expiration <= DateTime.UtcNow)
                {
                    _database.Remove(id);
                }
                else
                {
                    return record;
                }
            }
            return null;
        }

        public async Task RemoveShortUrlAsync(string id)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(id), id);
            if (_database.ContainsKey(id))
            {
                _database.Remove(id);
            }
        }

        public async Task<ShortUrl> GetShortUrlAsync(string id, string tenant)
        {
            var shortUrl = await GetShortUrlAsync(id);
            if (shortUrl.Tenant == tenant)
            {
                return shortUrl;
            }
            return null;
        }

        public async Task RemoveShortUrlAsync(string id, string tenant)
        {
            var shortUrl = await GetShortUrlAsync(id);
            if (shortUrl.Tenant == tenant)
            {
                await RemoveShortUrlAsync(id);
            }
        }
    }
}