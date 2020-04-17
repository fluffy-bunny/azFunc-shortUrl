using dotnetcore.urlshortener.contracts;

namespace dotnetcore.urlshortener.InMemoryStore
{
    public class ExpiredInMemoryUrlShortenerOperationalStore :
       InMemoryUrlShortenerOperationalStoreBase, IExpiredUrlShortenerOperationalStore
    {
        public ExpiredInMemoryUrlShortenerOperationalStore(IUrlShortenerAlgorithm urlShortenerAlgorithm) :
            base(urlShortenerAlgorithm)
        {
        }
    }
}