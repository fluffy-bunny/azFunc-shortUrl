using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Middleware.Hooks
{
    public interface IScopedRequestHook
    {
        Task OnPreEndpointRequestAsync(HttpContext httpContext, out bool proceed);
        Task OnPostEndpointRequestAsync(HttpContext httpContext);
    }
}
