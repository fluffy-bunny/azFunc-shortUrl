// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Net.Http;

namespace azFunc_urlshortener
{
    public class Function1
    {
        private IHttpClientFactory _httpClientFactory;

        public Function1(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [FunctionName("Function1")]
        public static void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            [DurableClient] IDurableClient client,
            ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
        }
    }
}
