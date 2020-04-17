using dotnetcore.Simple.CosmosDB.Models;
using System;

namespace dotnetcore.urlshortener.contracts
{
    public enum LongUrlType
    {
        Original,
        ExpiryRedirect
    }
    public class ShortUrl: BaseItem
    {
        public string LongUrl { get; set; }
 
        public string Tenant { get; set; }
        public DateTime? Expiration { get; set; }

    }
}