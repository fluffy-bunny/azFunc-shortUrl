using System.Text.RegularExpressions;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.generator.Extensions;

namespace dotnetcore.urlshortener.InMemoryStore
{
    public class InMemoryUrlShortenerOperationalStore :
        InMemoryUrlShortenerOperationalStoreBase, IUrlShortenerOperationalStore
    {
        public InMemoryUrlShortenerOperationalStore(IUrlShortenerAlgorithm urlShortenerAlgorithm) : 
            base(urlShortenerAlgorithm)
        {
        }
    }
}