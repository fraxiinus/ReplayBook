using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class ImageProperties
    {
        [JsonPropertyName("source")]
        public string? Source { get; set; }

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
