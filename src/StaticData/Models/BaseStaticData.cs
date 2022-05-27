using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public interface IStaticProperties
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public ImageProperties? ImageProperties { get; set; }
    }

    public abstract class BaseStaticProperties : IStaticProperties
    {
        public BaseStaticProperties(string id)
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

    public class ChampionProperties : BaseStaticProperties
    {
        public ChampionProperties(string id) : base(id) { }
    }

    public class ItemProperties : BaseStaticProperties
    {
        public ItemProperties(string id) : base(id) { }
    }

    public class RuneProperties : BaseStaticProperties
    {
        public RuneProperties(string id) : base(id)
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
