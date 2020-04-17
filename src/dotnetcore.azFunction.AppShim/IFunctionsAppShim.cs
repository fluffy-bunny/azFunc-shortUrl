using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnetcore.azFunction.AppShim
{
    public delegate void LoadConfigurationsDelegate(IConfigurationBuilder config, string environmentName);
    public interface IFunctionsAppShim
    {
        Task<HttpResponseMessage> Run(
            Microsoft.Azure.WebJobs.ExecutionContext context,
            HttpRequest request,
            ILogger log);
    }
}
