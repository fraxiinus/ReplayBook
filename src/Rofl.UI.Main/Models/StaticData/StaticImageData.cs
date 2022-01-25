using System.Text.Json.Serialization;

namespace Rofl.UI.Main.Models.StaticData
{
    public class StaticImageData
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("w")]
        public int Width { get; set; }

        [JsonPropertyName("h")]
        public int Height { get; set; }
    }
}
