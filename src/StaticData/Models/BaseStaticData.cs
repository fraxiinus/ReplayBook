using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public abstract class BaseStaticData
    {
        public BaseStaticData(string id)
        {
            Id = id;
            DisplayName = "N/A";
            ImageProperties = null;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("imageProperties")]
        public ImageProperties? ImageProperties { get; set; }
    }

    public class ChampionData : BaseStaticData
    {
        public ChampionData(string id) : base(id) { }
    }

    public class ItemData : BaseStaticData
    {
        public ItemData(string id) : base(id) { }
    }

    public class RuneData : BaseStaticData
    {
        public RuneData(string id) : base(id)
        {
            Key = null;
            IconUrl = null;
            EndOfGameStatDescs = new List<string>();
        }

        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("iconUrl")]
        public string? IconUrl { get; set; }

        [JsonPropertyName("endOfGameStatDescs")]
        public IEnumerable<string> EndOfGameStatDescs { get; set; }
    }
}
