using dotnetcore.Simple.CosmosDB.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetcore.urlshortener.contracts.Models
{
    public static class ShortUrlExtensions
    {
        public static ShortUrl ToShortUrl(this ShortUrlCosmosDocumentBase document)
        {
            if (document == null)
            {
                return null;
            }
            var shortUrl = new ShortUrl
            {
                Expiration = document.Expiration,
                Id = document.Id,
                Tenant = document.Tenant,
                LongUrl = document.LongUrl
            };
            return shortUrl;
        }
        public static ShortUrlCosmosDocumentBase ToShortUrlCosmosDocument(this ShortUrl shortUrl)
        {
            int? ttl = null;
            if (shortUrl.Expiration != null)
            {
                var diffInSeconds = (int)(((DateTime)shortUrl.Expiration - DateTime.UtcNow).TotalSeconds);
                if (diffInSeconds <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(shortUrl.Expiration));
                }
                ttl = diffInSeconds;
            }

            return new ShortUrlCosmosDocumentBase
            {
                Id = shortUrl.Id,
                LongUrl = shortUrl.LongUrl,
                Tenant = shortUrl.Tenant,
                Expiration = shortUrl.Expiration,
                ttl = ttl
            };
        }
    }

    public class ShortUrlCosmosDocumentBase: BaseItem
    {
        public ShortUrlCosmosDocumentBase() { }
        public ShortUrlCosmosDocumentBase(ShortUrlCosmosDocumentBase other)
        {
            Id = other.Id;
            LongUrl = other.LongUrl;
            Expiration = other.Expiration;
            Tenant = other.Tenant;
            ttl = other.ttl;
        }

       

        [JsonProperty(PropertyName = "tenant")]
        public string Tenant { get; set; }

        [JsonProperty(PropertyName = "longUrl")]
        public string LongUrl { get; set; }

        [JsonProperty(PropertyName = "expiration")]
        public DateTime? Expiration { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; }
    }
    public class ShortUrlCosmosDocument : ShortUrlCosmosDocumentBase
    {
        public ShortUrlCosmosDocument() { }
        public ShortUrlCosmosDocument(ShortUrlCosmosDocumentBase other) : base(other) { }
    }
    public class ExpiredShortUrlCosmosDocument : ShortUrlCosmosDocumentBase
    {
        public ExpiredShortUrlCosmosDocument() { }
        public ExpiredShortUrlCosmosDocument(ShortUrlCosmosDocumentBase other) : base(other) { }
    }

}

