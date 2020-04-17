namespace dotnetcore.urlshortener.contracts
{
    public class ExpirationRedirectRecord
    {
        public string ExpiredRedirectKey { get; set; }
        public string ExpiredRedirectUrl { get; set; }
        public string Tenant { get; set; }
    }
}