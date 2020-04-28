using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Middleware.Hooks;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using CorrelationId;
using CorrelationId.Abstractions;

namespace ShorturlRequestUserTracker
{
    public class ScopedRequestHook : IScopedRequestHook
    {
        private MiddlewareOptions _options;
        private ICorrelationContextAccessor _correlationContextAccessor;
        private ILogger<ScopedRequestHook> _logger;
        private Claim _claim;

        public ScopedRequestHook(
            IOptionsMonitor<MiddlewareOptions>  optionMonitorAccessor,
            ICorrelationContextAccessor correlationContextAccessor,
            ILogger<ScopedRequestHook> logger)
        {
            _options = optionMonitorAccessor.CurrentValue;
            _correlationContextAccessor = correlationContextAccessor;
            _logger = logger;
        }

        public Task OnPreEndpointRequestAsync(HttpContext httpContext, out bool proceed)
        {
          //  _logger.LogDebug($"user:{httpContext.User.}")
            proceed = true;
            _claim = (from item in httpContext.User.Claims
                        where item.Type == "client_id"
                        select item).FirstOrDefault();
            _logger.LogInformation($"CorrelationId={_correlationContextAccessor.CorrelationContext.CorrelationId},OnPreEndpointRequestAsync: client_id={_claim.Value}");

            return Task.CompletedTask;
        }

        public Task OnPostEndpointRequestAsync(HttpContext httpContext)
        {
            _logger.LogInformation($"CorrelationId={_correlationContextAccessor.CorrelationContext.CorrelationId},OnPreEndpointRequestAsync: client_id={_claim.Value}");
            return Task.CompletedTask;
        }
    }
}
