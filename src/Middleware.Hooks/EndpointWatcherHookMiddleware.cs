using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Middleware.Hooks
{

    public class EndpointWatcherHookMiddleware
    {
        private readonly RequestDelegate _next;

        public EndpointWatcherHookMiddleware(RequestDelegate next, IScopedRequestHook userHook)
        {
            _next = next;
        }

        // IMyScopedService is injected into Invoke
        public async Task Invoke(HttpContext httpContext, IScopedRequestHook userHook)
        {
            bool proceed = true;
            await userHook.OnPreEndpointRequestAsync(httpContext,out proceed);
            if (!proceed)
            {
                return;
            }
            await _next(httpContext); 
            await userHook.OnPostEndpointRequestAsync(httpContext);
        }
    }
}
