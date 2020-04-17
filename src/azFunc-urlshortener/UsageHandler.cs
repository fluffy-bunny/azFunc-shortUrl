// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using dotnetcore.azFunction.AppShim;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.EventGrid.Models;

namespace azFunc_urlshortener
{
    public class UsageHandler
    {
        private IFunctionsAppShim _functionsAppShim;
        public UsageHandler(IFunctionsAppShim functionsAppShim)
        {
            _functionsAppShim = functionsAppShim;
        }
        [FunctionName("UsageHandler")]
        public static void Run(
          [EventGridTrigger]EventGridEvent eventGridEvent,
          ILogger log)
        {
            //  eventGridEvent.
            log.LogInformation(eventGridEvent.Data.ToString());
        }
    }
}
