using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Files.Models
{
    public class CacheIndex
    {
        [JsonProperty("version")]
        public string IndexVersion { get; set; }

        [JsonProperty("items")]
        public List<CacheIndexItem> Items { get; set; }
    }
}
