using System;

namespace dotnetcore.urlshortener.contracts
{
    public class ShortenerEventArgs : EventArgs
    {
        public ShortUrl ShortUrl { get; set; }
        public ShortenerEventType EventType { get; set; }
        public DateTime UtcDateTime { get; set; }
    }
}