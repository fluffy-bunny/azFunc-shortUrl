using Newtonsoft.Json;

namespace webApp_urlshortener.Models
{
    public partial class OperationalStore
    {
        [JsonProperty("database")]
        public string Database { get; set; }

        [JsonProperty("collection")]
        public string Collection { get; set; }
    }
}
