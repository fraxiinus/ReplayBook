using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rofl.Settings.Models
{
    public class GeneralSettings
    {
        public GeneralSettings()
        {
            KnownPlayers = new List<PlayerMarker>();
        }

        [JsonProperty("known_players")]
        public List<PlayerMarker> KnownPlayers { get; private set; }

        [JsonProperty("play_confirm")]
        public bool PlayConfirmation { get; set; }

        [JsonProperty("match_history_url")]
        public string MatchHistoryBaseUrl { get; set; }

        [JsonProperty("items_per_page")]
        public int ItemsPerPage { get; set; }

    }
}
