using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;

namespace webApp_urlshortener.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string ToFullHostName(this HttpRequest req)
        {
            var scheme = req.IsHttps ? "https" : "http";
            return $"{scheme}://{req.Host.Value}";
        }
        public static NameValueCollection ToNameValueCollectionHeader(this HttpRequest req)
        {
            var result = new NameValueCollection();
            foreach (var item in req.Headers)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
        public static NameValueCollection ToNameValueCollectionRequest(this HttpRequest req)
        {
            if (req.Method == "GET")
            {
                return req.Query.ToNameValueCollection();
            }
            if (req.Method == "POST")
            {
                return req.Form.ToNameValueCollection();
            }
            return null;
        }
        public static NameValueCollection ToNameValueCollection(this IQueryCollection req)
        {
            var result = new NameValueCollection();
            foreach (var item in req)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
        public static NameValueCollection ToNameValueCollection(this IFormCollection req)
        {
            var result = new NameValueCollection();
            foreach (var item in req)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
    }
}
