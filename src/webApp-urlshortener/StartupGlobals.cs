using Microsoft.Extensions.Logging;

namespace webApp_urlshortener
{
    public static class StartupGlobals
    {
        public static ILogger ExternalShimLogger { get; set; }
    }
}
