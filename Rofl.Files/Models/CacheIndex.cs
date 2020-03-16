using Newtonsoft.Json;
using System.Collections.Generic;

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
