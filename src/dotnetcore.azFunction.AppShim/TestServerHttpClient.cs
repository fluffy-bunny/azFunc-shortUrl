using System.Net.Http;

namespace dotnetcore.azFunction.AppShim
{
    class TestServerHttpClient : ITestServerHttpClient
    {
        public HttpClient HttpClient { get; set; }
    }
}
