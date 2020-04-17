using System.Net.Http;

namespace dotnetcore.azFunction.AppShim
{
    public interface ITestServerHttpClient
    {
        HttpClient HttpClient { get; }
    }
}
