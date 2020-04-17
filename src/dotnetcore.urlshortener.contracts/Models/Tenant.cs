using Newtonsoft.Json;
using System.Collections.Generic;

namespace dotnetcore.urlshortener.contracts.Models
{
    /*
     {
	"authority": "https://apim-organics.azure-api.net/oauth2",
	"tenants": [{
		"credentials": {
			"client_id": "b2b-client",
			"client_secret": "secret"
		},
		"name": "marketing",
		"options": {
			"max_ttl": 2592000
		}
	}]
}
{"authority":"https:\/\/apim-organics.azure-api.net\/oauth2","tenants":[{"credentials":{"client_id":"b2b-client","client_secret":"secret"},"name":"marketing","options":{"max_ttl":2592000}}]}


     */

    public partial class TenantConfiguration
    {
        [JsonProperty("authority")]
        public string Authority { get; set; }
        [JsonProperty("tenants")]
        public List<Tenant> Tenants { get; set; }
    }

    public partial class Tenant
    {
        [JsonProperty("credentials")]
        public Credentials Credentials { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }
    }

    public partial class Credentials
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }

    public partial class Options
    {
        [JsonProperty("max_ttl")]
        public long MaxTtl { get; set; }
    }
    public partial class ShortUrlConfiguration
    {
        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("tenants")]
        public List<Tenant> Tenants { get; set; }
    }


}

