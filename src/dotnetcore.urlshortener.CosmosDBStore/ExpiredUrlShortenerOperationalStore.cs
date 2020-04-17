using CosmosDB.Simple.Store.Interfaces;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.contracts.Models;

namespace dotnetcore.urlshortener.CosmosDBStore
{
    public class ExpiredUrlShortenerOperationalStore :
        UrlShortenerOperationalStoreAbstract<ExpiredShortUrlCosmosDocument>, IExpiredUrlShortenerOperationalStore
    {
        public ExpiredUrlShortenerOperationalStore(
            IUrlShortenerAlgorithm urlShortenerAlgorithm,
            ISimpleItemDbContext<ExpiredShortUrlCosmosDocument> simpleItemDbContext) :
            base(urlShortenerAlgorithm, simpleItemDbContext)
        {
        }

        public override ExpiredShortUrlCosmosDocument CreateDocument(ShortUrlCosmosDocumentBase shortUrlCosmosDocumentBase)
        {
            var document = new ExpiredShortUrlCosmosDocument(shortUrlCosmosDocumentBase);
            return document;
        }
    }
}