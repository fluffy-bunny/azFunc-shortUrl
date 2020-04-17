using System.Collections.Generic;
using dotnetcore.urlshortener.contracts;

namespace dotnetcore.urlshortener.InMemoryStore
{
    public class InMemoryUrlShortenerConfigurationModel
    {
        public List<ExpirationRedirectRecord> Records { get; set; }
        public string DefaultExpiredRedirectKey { get; set; }
    }
}