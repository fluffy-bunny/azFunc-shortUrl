using System.Text.RegularExpressions;
using CosmosDB.Simple.Store.Interfaces;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;
using dotnetcore.urlshortener.generator.Extensions;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace dotnetcore.urlshortener.CosmosDBStore
{
    public class UrlShortenerOperationalStore :
        UrlShortenerOperationalStoreAbstract<ShortUrlCosmosDocument>, IUrlShortenerOperationalStore
    {
        public UrlShortenerOperationalStore(
            IUrlShortenerAlgorithm urlShortenerAlgorithm,
            ISimpleItemDbContext<ShortUrlCosmosDocument> simpleItemDbContext) :
            base(urlShortenerAlgorithm, simpleItemDbContext)
        {
        }

        public override ShortUrlCosmosDocument CreateDocument(ShortUrlCosmosDocumentBase shortUrlCosmosDocumentBase)
        {
            var document = new ShortUrlCosmosDocument(shortUrlCosmosDocumentBase);
            return document;
        }
    }
}