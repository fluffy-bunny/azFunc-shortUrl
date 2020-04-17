using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using dotnetcore.azFunction.AppShim;
using webApp_urlshortener;

namespace azFunc_urlshortener
{
    public class Ingress
    {
        private IFunctionsAppShim _functionsAppShim;
        public Ingress(IFunctionsAppShim functionsAppShim)
        {
            _functionsAppShim = functionsAppShim;
        }

        [FunctionName("Ingress")]
        public async Task<HttpResponseMessage> Run(
            Microsoft.Azure.WebJobs.ExecutionContext context,
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", "delete", Route = "{*all}")] HttpRequest request,
            ILogger logger)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");
            StartupGlobals.ExternalShimLogger = logger;
            return await _functionsAppShim.Run(context, request, logger);
        }
    }
}
