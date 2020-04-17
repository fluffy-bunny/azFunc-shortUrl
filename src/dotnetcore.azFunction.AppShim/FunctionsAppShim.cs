using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnetcore.azFunction.AppShim
{
    public class FunctionsAppShim<TStartup> : IFunctionsAppShim
        where TStartup : class
    {
        public LoadConfigurationsDelegate LoadConfigurationsDelegate { get; set; }
        ITestServerHttpClient _testServerHttpClient;
        ITestServerHttpClient FetchTestServerHttpClient(ExecutionContext context, ILogger logger)
        {
            if (_testServerHttpClient == null)
            {
                var hostBuilder = new HostBuilder()
                 .ConfigureWebHost(webHost =>
                 {
                     //webHost.UseContentRoot()
                     // Add TestServer
                     webHost.UseTestServer();
                     webHost.UseStartup<TStartup>();
                     webHost.ConfigureAppConfiguration((hostingContext, config) =>
                     {
                         var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                         config.SetBasePath(context.FunctionAppDirectory);
                         LoadConfigurationsDelegate(config, environmentName);
                         config.AddEnvironmentVariables();
                         config.AddUserSecrets<TStartup>();
                     });
                 })
                 .ConfigureServices((context, services) =>
                 {

                 })
                 .ConfigureLogging((context, loggingBuilder) =>
                 {
                     loggingBuilder.ClearProviders();
                     loggingBuilder.AddProvider(new HostLoggerProvider("me-tenant", logger));
                 });
                // Build and start the IHost
                var host = hostBuilder.StartAsync().GetAwaiter().GetResult();
                _testServerHttpClient = new TestServerHttpClient
                {
                    HttpClient = host.GetTestClient()
                };
            }
            return _testServerHttpClient;
        }
        public async Task<HttpResponseMessage> Run(ExecutionContext context, HttpRequest request, ILogger logger)
        {
            var testServerHttpClient = FetchTestServerHttpClient(context, logger);

            var httpRequestMessageFeature = new HttpRequestMessageFeature(request);
            var httpRequestMessage = httpRequestMessageFeature.HttpRequestMessage;

         //  var path = new PathString(request.Path.Value.ToLower());

            var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host)
            {
                Path = request.Path,
                Query = request.QueryString.Value
            };
            if (request.Host.Port != null)
            {
                uriBuilder.Port = (int)request.Host.Port;
            }

            httpRequestMessage.RequestUri = uriBuilder.Uri;
            httpRequestMessage.Headers.Remove("Host");
            var responseMessage = await testServerHttpClient.HttpClient.SendAsync(httpRequestMessage);
            return responseMessage;
        }
    }
}
