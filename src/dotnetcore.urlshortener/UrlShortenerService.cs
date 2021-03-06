﻿using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using dotnetcore.urlshortener.contracts;
using dotnetcore.urlshortener.Utils;
using Microsoft.Extensions.Logging;

namespace dotnetcore.urlshortener
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private EventSource<ShortenerEventArgs> _eventSource;
        private ILogger<UrlShortenerService> _logger;
        private IUrlShortenerOperationalStore _urlShortenerOperationalStore;
        private IExpiredUrlShortenerOperationalStore _expiredUrlShortenerOperationalStore;

        void IUrlShortenerEventSource<ShortenerEventArgs>.AddListenter(
            EventHandler<ShortenerEventArgs> handler)
        {
            _eventSource.AddListenter(handler);
        }

        void IUrlShortenerEventSource<ShortenerEventArgs>.RemoveListenter(
            EventHandler<ShortenerEventArgs> handler)
        {
            _eventSource.RemoveListenter(handler);
        }


        public UrlShortenerService(
            IUrlShortenerOperationalStore urlShortenerOperationalStore,
            IExpiredUrlShortenerOperationalStore expiredUrlShortenerOperationalStore,
            ILogger<UrlShortenerService> logger)
        {
            _urlShortenerOperationalStore = urlShortenerOperationalStore;
            _expiredUrlShortenerOperationalStore = expiredUrlShortenerOperationalStore;
            _eventSource = new EventSource<ShortenerEventArgs>();
            _logger = logger;
        }
        public async Task<(HttpStatusCode, ShortUrl)> UpsertShortUrlAsync(string expiredKey, ShortUrl shortUrl)
        {
            try
            {
                Guard.ArgumentNotNull(nameof(shortUrl), shortUrl);
                Guard.ArgumentNotNull(nameof(expiredKey), expiredKey);

                var (code, record) = await _urlShortenerOperationalStore.UpsertShortUrlAsync(shortUrl);
                if (!code.IsSuccess())
                {
                    return (code, record);
                }
                record.Id = $"{expiredKey}.{record.Id}";

                _eventSource.FireEvent(new ShortenerEventArgs()
                {
                    ShortUrl = record,
                    EventType = ShortenerEventType.Upsert,
                    UtcDateTime = DateTime.UtcNow
                });
                return (code, record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ShortUrl> GetShortUrlAsync(string id)
        {
            try
            {
                Guard.ArguementEvaluate(nameof(id),
                    (() =>
                    {
                        if (id.Length <= 4)
                        {
                            return (false, "The value must be greater than 4 in length");
                        }

                        return (true, null);
                    }));
                var keys = id.Split('.');

                var expiredKey = keys[0];
                var key = keys[1];
                var record = await _urlShortenerOperationalStore.GetShortUrlAsync(key);
                if (record != null)
                {
                    _eventSource.FireEvent(new ShortenerEventArgs()
                    {
                        ShortUrl = record,
                        EventType = ShortenerEventType.Get,
                        UtcDateTime = DateTime.UtcNow
                    });
                    return record;
                }

                record = await _expiredUrlShortenerOperationalStore.GetShortUrlAsync(expiredKey);

                _eventSource.FireEvent(new ShortenerEventArgs()
                {
                    ShortUrl = record,
                    EventType = ShortenerEventType.Expired,
                    UtcDateTime = DateTime.UtcNow
                });

                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task RemoveShortUrlAsync(string id)
        {
            try
            {

                Guard.ArguementEvaluate(nameof(id),
                    (() =>
                    {
                        if (id.Length <= 4)
                        {
                            return (false, "The value must be greater than 4 in length");
                        }

                        return (true, null);
                    }));
                var keys = id.Split('.');

                var expiredKey = keys[0];
                var key = keys[1];

                var original = await _urlShortenerOperationalStore.GetShortUrlAsync(key);
                if (original != null)
                {
                    _eventSource.FireEvent(new ShortenerEventArgs()
                    {
                        ShortUrl = original,
                        EventType = ShortenerEventType.Remove,
                        UtcDateTime = DateTime.UtcNow
                    });
                    await _urlShortenerOperationalStore.RemoveShortUrlAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<ShortUrl> GetShortUrlAsync(string id, string tenant)
        {
            try
            {
                Guard.ArguementEvaluate(nameof(id),
                   (() =>
                   {
                       if (id.Length <= 4)
                       {
                           return (false, "The value must be greater than 4 in length");
                       }

                       return (true, null);
                   }));
                Guard.ArguementEvaluate(nameof(tenant),
                 (() =>
                 {
                     if (id.Length <= 4)
                     {
                         return (false, "The value must be greater than 4 in length");
                     }

                     return (true, null);
                 }));
                var keys = id.Split('.');

                var expiredKey = keys[0];
                var key = keys[1];

                var record = await _urlShortenerOperationalStore.GetShortUrlAsync(key, tenant);
                if (record != null)
                {
                    _eventSource.FireEvent(new ShortenerEventArgs()
                    {
                        ShortUrl = record,
                        EventType = ShortenerEventType.Get,
                        UtcDateTime = DateTime.UtcNow
                    });
                    return record;
                }

                record = await _expiredUrlShortenerOperationalStore.GetShortUrlAsync(expiredKey, tenant);

                _eventSource.FireEvent(new ShortenerEventArgs()
                {
                    ShortUrl = record,
                    EventType = ShortenerEventType.Expired,
                    UtcDateTime = DateTime.UtcNow
                });

                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task RemoveShortUrlAsync(string id, string tenant)
        {
            try
            {
                Guard.ArguementEvaluate(nameof(id),
                    (() =>
                    {
                        if (id.Length <= 4)
                        {
                            return (false, "The value must be greater than 4 in length");
                        }

                        return (true, null);
                    }));
                Guard.ArguementEvaluate(nameof(tenant),
                   (() =>
                   {
                       if (id.Length <= 4)
                       {
                           return (false, "The value must be greater than 4 in length");
                       }

                       return (true, null);
                   }));
                var keys = id.Split('.');

                var expiredKey = keys[0];
                var key = keys[1];

                var original = await _urlShortenerOperationalStore.GetShortUrlAsync(key, tenant);
                if (original != null)
                {
                    _eventSource.FireEvent(new ShortenerEventArgs()
                    {
                        ShortUrl = original,
                        EventType = ShortenerEventType.Remove,
                        UtcDateTime = DateTime.UtcNow
                    });
                    await _urlShortenerOperationalStore.RemoveShortUrlAsync(key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
