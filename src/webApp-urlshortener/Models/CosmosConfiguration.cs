using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webApp_urlshortener.Models
{

    public partial class CosmosConfiguration
    {
        [JsonProperty("useCosmos")]
        public bool UseCosmos { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }

        [JsonProperty("primaryConnectionString")]
        public string PrimaryConnectionString { get; set; }

        [JsonProperty("OperationalStore")]
        public OperationalStore OperationalStore { get; set; }
    }
}
