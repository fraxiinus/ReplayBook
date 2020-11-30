using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Rofl.Settings.Models
{
    public enum MarkerStyle { Border = 0, Square = 1 }

    public class GeneralSettings
    {
        public GeneralSettings()
        {
            KnownPlayers = new List<PlayerMarker>();
        }

        [JsonProperty("known_players")]
        public List<PlayerMarker> KnownPlayers { get; private set; }

        [JsonProperty("marker_style")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarkerStyle PlayerMarkerStyle { get; set; }

        [JsonProperty("file_action")]
        public int FileAction { get; set; }

        [JsonProperty("play_confirm")]
        public bool PlayConfirmation { get; set; }

        [JsonProperty("match_history_url")]
        public string MatchHistoryBaseUrl { get; set; }

        [JsonProperty("items_per_page")]
        public int ItemsPerPage { get; set; }

    }
}
