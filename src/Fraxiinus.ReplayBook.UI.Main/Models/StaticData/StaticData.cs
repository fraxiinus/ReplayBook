using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.UI.Main.Models.StaticData
{
    public abstract class StaticData
    {
        public StaticData(string id)
        {
            Id = id;
            DisplayName = "N/A";
            ImageData = null;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("image")]
        public StaticImageData ImageData { get; set; }
    }
}
