using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetcore.Simple.CosmosDB.Models
{
    public class BaseItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
