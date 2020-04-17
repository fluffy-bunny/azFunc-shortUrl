using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using dotnetcore.urlshortener.contracts;
using Microsoft.Extensions.Configuration;

namespace dotnetcore.urlshortener.InMemoryStore
{
    public class InMemoryUrlShortenerExpiryOperationalStore : IExpiredUrlShortenerOperationalStore
    {
        private Dictionary<string, ExpirationRedirectRecord> _database;
        private string DefaultExpiredRedirectKey { get; set; }
        public InMemoryUrlShortenerExpiryOperationalStore(IConfiguration configuration)
        {
            _database = new Dictionary<string, ExpirationRedirectRecord>();
            var section = configuration.GetSection("InMemoryUrlShortenerExpiryOperationalStore");
            var model = new InMemoryUrlShortenerConfigurationModel();

            section.Bind(model);

            foreach (var record in model.Records)
            {
                _database[record.ExpiredRedirectKey] = record;
            }

            DefaultExpiredRedirectKey = model.DefaultExpiredRedirectKey;
        }
        public async Task<ExpirationRedirectRecord> GetExpirationRedirectRecordAsync(string key)
        {
            if (_database.ContainsKey(key))
            {
                return _database[key];
            }

            if (_database.ContainsKey(DefaultExpiredRedirectKey))
            {
                return _database[DefaultExpiredRedirectKey];
            }
            return null;
        }

        public Task<(HttpStatusCode, ShortUrl)> UpsertShortUrlAsync(ShortUrl shortUrl)
        {
            throw new NotImplementedException();
        }

        public Task<ShortUrl> GetShortUrlAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveShortUrlAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ShortUrl> GetShortUrlAsync(string id, string tenant)
        {
            throw new NotImplementedException();
        }

        public Task RemoveShortUrlAsync(string id, string tenant)
        {
            throw new NotImplementedException();
        }
    }
}