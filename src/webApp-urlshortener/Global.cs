using Microsoft.Extensions.Logging;
using webApp_urlshortener.HostContext;

namespace webApp_urlshortener
{
    public class Global
    {
        public static IHostContext HostContext { get; set; }
        public static ILoggerProvider LoggerProvider { get; set; }
    }
}
