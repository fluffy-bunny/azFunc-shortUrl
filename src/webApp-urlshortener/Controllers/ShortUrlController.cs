using System;
using System.Threading.Tasks;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using webApp_urlshortener.Extensions;
using System.Linq;
using System.Net;

namespace webApp_urlshortener.Controllers
{
    public class ShortUrlRequest
    {
        public int? ttl { get; set; }
        public string LongUrl { get; set; }
        public string ExpiredKey { get; set; }
    }

    [ApiController]
    [Route("api/short-url-service")]
    [Authorize]
    public class ShortUrlController : ControllerBase
    {
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ILogger<ShortUrlController> _logger;

        public ShortUrlController(
            IUrlShortenerService urlShortenerService,
            ILogger<ShortUrlController> logger)
        {
            _urlShortenerService = urlShortenerService;
            _logger = logger;
        }
        string FetchTenantFromUser()
        {
            var tenant = (from c in User.Claims
                          where c.Type == "url-shortener-tenant"
                          select c).FirstOrDefault();
            if (tenant == null || string.IsNullOrWhiteSpace(tenant.Value))
            {
                throw new Exception($"url-shortener-tenant claim is invalid");
            }
            return tenant.Value;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRecord(string key)
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty(nameof(key), key);
                var tenant = FetchTenantFromUser();
                await _urlShortenerService.RemoveShortUrlAsync(key, tenant);

                var jsonResult = new JsonResult("deleted")
                {
                    StatusCode = StatusCodes.Status200OK
                };
                return jsonResult;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return new BadRequestObjectResult("Invalid Request");
        }

        [HttpGet]
        [Route("{key}")]
        public async Task<IActionResult> RetrieveRecord(string key)
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty(nameof(key), key);
                var tenant = FetchTenantFromUser();
                var record = await _urlShortenerService.GetShortUrlAsync(key, tenant);
                if (record == null)
                {
                    return new NotFoundObjectResult(key);
                }
                var jsonResult = new JsonResult(record)
                {
                    StatusCode = StatusCodes.Status200OK
                };
                return jsonResult;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return new BadRequestObjectResult("Invalid Request");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecord([FromBody]ShortUrlRequest shortUrlRequest)
        {
            try
            {
                Guard.ArgumentNotNullOrEmpty(nameof(shortUrlRequest.LongUrl), shortUrlRequest.LongUrl);
                Guard.ArgumentNotNullOrEmpty(nameof(shortUrlRequest.ExpiredKey), shortUrlRequest.ExpiredKey);
                Guard.ArgumentNotNull(nameof(shortUrlRequest.ttl), shortUrlRequest.ttl);

                var tenant = FetchTenantFromUser();

                bool isUri = Uri.IsWellFormedUriString(shortUrlRequest.LongUrl, UriKind.Absolute);
                if (!isUri)
                {
                    throw new Exception($"Failed IsWellFormedUriString:{shortUrlRequest.LongUrl}");
                }
                var shortUrl = new ShortUrl
                {
                    LongUrl = shortUrlRequest.LongUrl,
                    Tenant = tenant,
                    Expiration = DateTime.UtcNow.AddSeconds((double)shortUrlRequest.ttl)
                };
                HttpStatusCode code = HttpStatusCode.BadRequest;
                (code,shortUrl) = await _urlShortenerService.UpsertShortUrlAsync(shortUrlRequest.ExpiredKey,
                    shortUrl);

                var jsonResult = new JsonResult(shortUrl)
                {
                    StatusCode = (int)code
                };
                return jsonResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return new BadRequestObjectResult("Invalid Request");

        }
    }
}
