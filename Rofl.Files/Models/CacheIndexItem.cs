using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Files.Models
{
    public class CacheIndexItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("replay_location")]
        public string Location { get; set; }

        [JsonProperty("cache_timestamp")]
        public DateTimeOffset CacheDateTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            CacheIndexItem compItem = (CacheIndexItem) obj;

            return this.Id == compItem.Id &&
                   this.Location == compItem.Location;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Location.GetHashCode();
        }
    }
}
