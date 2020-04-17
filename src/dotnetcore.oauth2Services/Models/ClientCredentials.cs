using dotnetcore.urlshortener.contracts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace dotnetcore.oauth2Services.Models
{
    public partial class ClientCredentials
    {
        [JsonProperty("authority")]
        public string Authority { get; set; }
        [JsonProperty("tenants")]
        public List<Tenant> Tenants { get; set; }
    }
}
