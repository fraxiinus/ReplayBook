using Fraxiinus.ReplayBook.Configuration.Models;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class Bundle
    {
        [JsonPropertyName("patch")]
        public string Patch { get; set; } = string.Empty;

        [JsonPropertyName("download_date")]
        public DateTimeOffset LastDownloadDate { get; set; }

        [JsonPropertyName("champion_image_files")]
        public List<string> ChampionImageFiles { get; set; } = new List<string>();

        [JsonPropertyName("item_image_files")]
        public List<string> ItemImageFiles { get; set; } = new List<string>();

        [JsonPropertyName("rune_image_files")]
        public Dictionary<string, string> RuneImageFiles { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("champion_data_files")]
        public Dictionary<string, string> ChampionDataFiles { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("item_data_files")]
        public Dictionary<string, string> ItemDataFiles { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("rune_data_files")]
        public Dictionary<string, string> RuneDataFiles { get; set; } = new Dictionary<string, string>();
    }
}
