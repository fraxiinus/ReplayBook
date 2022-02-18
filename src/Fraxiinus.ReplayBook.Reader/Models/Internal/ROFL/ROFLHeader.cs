using Newtonsoft.Json;

namespace Fraxiinus.ReplayBook.Reader.Models.Internal.ROFL
{
    /// <summary>
    /// Low level model of ROFL file, split into different sections
    /// </summary>
    public class ROFLHeader : IReplayHeader
    {
        public LengthFields LengthFields { get; set; }
        public MatchMetadata MatchMetadata { get; set; }
        public PayloadFields PayloadFields { get; set; }

        [JsonIgnore]
        public string RawJsonString { get; set; }
    }
}
