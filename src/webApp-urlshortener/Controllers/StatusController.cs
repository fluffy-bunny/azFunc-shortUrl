using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace webApp_urlshortener.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var iteracion = 1;
            _logger.LogDebug($"Debug {iteracion}");
            _logger.LogInformation($"Information {iteracion}");
            _logger.LogWarning($"Warning {iteracion}");
            _logger.LogError($"Error {iteracion}");
            _logger.LogCritical($"Critical {iteracion}");

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new List<string> { "ok" };
        }
    }
}
