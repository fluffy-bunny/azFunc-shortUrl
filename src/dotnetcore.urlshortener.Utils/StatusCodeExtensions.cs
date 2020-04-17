using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace dotnetcore.urlshortener.Utils
{
    public static class StatusCodeExtensions
    {
        private static readonly ConcurrentDictionary<HttpStatusCode, bool> IsSuccessStatusCode = new ConcurrentDictionary<HttpStatusCode, bool>();
        public static bool IsSuccess(this HttpStatusCode statusCode) => IsSuccessStatusCode.GetOrAdd(statusCode, c => new HttpResponseMessage(c).IsSuccessStatusCode);
    }
}
