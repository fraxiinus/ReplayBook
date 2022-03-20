using Fraxiinus.ReplayBook.Configuration.Models;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class Bundle
    {
        [JsonPropertyName("patch")]
        public string Patch { get; set; } = string.Empty;

        [JsonPropertyName("download_date")]
        public DateTimeOffset DownloadDate { get; set; }

        [JsonPropertyName("included_languages")]
        public List<Language> IncludedLanguages { get; set; } = new List<Language>();

        [JsonPropertyName("champion_image_files")]
        public List<string> ChampionImageFiles { get; set; } = new List<string>();

        [JsonPropertyName("champion_data_files")]
        public List<string> ChampionDataFiles { get; set; } = new List<string>();

        [JsonPropertyName("item_image_files")]
        public List<string> ItemImageFiles { get; set; } = new List<string>();

        [JsonPropertyName("item_data_files")]
        public List<string> ItemDataFiles { get; set; } = new List<string>();

        [JsonPropertyName("rune_data_files")]
        public List<string> RuneDataFiles { get; set; } = new List<string>();
    }
}
