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
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using System.Collections.Generic;

namespace ShorturlRequestUserTracker
{
    public class ScopedRequestHook : IScopedRequestHook
    {
        private MiddlewareOptions _options;
        private ICorrelationContextAccessor _correlationContextAccessor;
        private ILogger<ScopedRequestHook> _logger;
        private Claim _claim;
        private TelemetryClient _telemetryClient { get; set; }
        public ScopedRequestHook(
            IOptionsMonitor<MiddlewareOptions>  optionMonitorAccessor,
            ICorrelationContextAccessor correlationContextAccessor,
            TelemetryClient telemetryClient,
            ILogger<ScopedRequestHook> logger)
        {
            _telemetryClient = telemetryClient;
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

            _logger.LogInformation($"CorrelationId={_correlationContextAccessor.CorrelationContext.CorrelationId},OnPreEndpointRequestAsync: client_id={(_claim == null ? "none":_claim.Value)}");


            return Task.CompletedTask;
        }

        public Task OnPostEndpointRequestAsync(HttpContext httpContext)
        {
            _logger.LogInformation($"CorrelationId={_correlationContextAccessor.CorrelationContext.CorrelationId},OnPostEndpointRequestAsync: client_id={(_claim == null ? "none" : _claim.Value)}");
            if(_claim != null)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("client_id", _claim.Value);
                _telemetryClient.TrackEvent("clientRequest",data);
                _telemetryClient.Flush();
            }

            return Task.CompletedTask;
        }
    }
}
