using dotnetcore.urlshortener.contracts;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace webApp_urlshortener.Controllers
{


    [Route("api/Redirector")]
    public class RedirectorController : ControllerBase
    {
        private IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;
        private ITenantStore _tenantStore;
        private ILogger<RedirectorController> _logger;

        public RedirectorController(
             IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ITenantStore tenantStore,
            ILogger<RedirectorController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _tenantStore = tenantStore;
            _logger = logger;
        }
        public async Task<IActionResult> Get(string tenant, string key)
        {
            var tenantServices = await _tenantStore.GetTenantAsync(tenant);
            var tokenResponse = await tenantServices.GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient("short-url");
            client.BaseAddress = new Uri("https://apim-organics.azure-api.net/azFunc-urlshortener/short-url-service");
            client.SetBearerToken(tokenResponse.AccessToken);

            return new JsonResult(tokenResponse.AccessToken);
        }
    }
}
