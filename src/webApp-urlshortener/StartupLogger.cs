using Microsoft.Extensions.Logging;

namespace webApp_urlshortener
{
    public class StartupLogger
    {
        private readonly ILogger _logger;

        public StartupLogger(ILogger<StartupLogger> logger)
        {
            _logger = StartupGlobals.ExternalShimLogger;
            if (_logger == null)
            {
                _logger = logger;
            }
        }

        public void Log(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
