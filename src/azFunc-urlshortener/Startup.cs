using dotnetcore.azFunction.AppShim;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(azFunc_urlshortener.Startup))]
namespace azFunc_urlshortener
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            var functionsAppShim = new FunctionsAppShim<webApp_urlshortener.Startup>
            {
                LoadConfigurationsDelegate = webApp_urlshortener.Program.LoadConfigurations
            };
            builder.Services.AddSingleton<IFunctionsAppShim>(functionsAppShim);
        }
    }
}