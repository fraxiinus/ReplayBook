using Newtonsoft.Json;

namespace Rofl.Settings.Models
{
    // Shuttup about my string urls >:(
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]

    public class RequestSettings
    {
        [JsonProperty("datadragon_baseurl")]
        public string DataDragonBaseUrl { get; set; }

        [JsonProperty("champion_relurl")]
        public string ChampionRelativeUrl { get; set; }

        [JsonProperty("map_relurl")]
        public string MapRelativeUrl { get; set; }

        [JsonProperty("item_relurl")]
        public string ItemRelativeUrl { get; set; }

        [JsonProperty("use_most_recent")]
        public bool UseMostRecent { get; set; }
    }
}
