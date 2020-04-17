using System.Net;
using System.Threading.Tasks;

namespace dotnetcore.urlshortener.contracts
{
    public interface IUrlShortenerOperationalStoreBase
    {
        Task<(HttpStatusCode, ShortUrl)> UpsertShortUrlAsync(ShortUrl shortUrl);
        Task<ShortUrl> GetShortUrlAsync(string id, string tenant);
        Task RemoveShortUrlAsync(string id, string tenant);
        Task<ShortUrl> GetShortUrlAsync(string id);
        Task RemoveShortUrlAsync(string id);
    }
}