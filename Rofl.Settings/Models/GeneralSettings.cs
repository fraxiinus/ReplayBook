using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Rofl.Settings.Models
{
    public enum MarkerStyle 
    {
        /// <summary>
        /// Show player marker as a colored border around the icon
        /// </summary>
        Border = 0,

        /// <summary>
        /// Show player marker as a square in the top right of the icon
        /// </summary>
        Square = 1
    }

    public enum FileAction 
    {
        /// <summary>
        /// Play replays when opened in Explorer
        /// </summary>
        Play = 0,

        /// <summary>
        /// Open replays in ReplayBook when opened in Explorer
        /// </summary>
        Open = 1
    }

    public enum RenameAction 
    { 
        /// <summary>
        /// Rename replays in the filesystem
        /// </summary>
        File = 0, 

        /// <summary>
        /// Rename replays in the database, files untouched
        /// </summary>
        Database = 1
    }

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
        [JsonConverter(typeof(StringEnumConverter))]
        public FileAction FileAction { get; set; }

        [JsonProperty("play_confirm")]
        public bool PlayConfirmation { get; set; }

        [JsonProperty("rename_action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RenameAction RenameAction { get; set; }

        [JsonProperty("match_history_url")]
        public string MatchHistoryBaseUrl { get; set; }

        [JsonProperty("items_per_page")]
        public int ItemsPerPage { get; set; }

        [JsonProperty("auto_update_check")]
        public bool AutoUpdateCheck { get; set; }

    }
}
