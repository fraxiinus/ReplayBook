using Newtonsoft.Json;

namespace Rofl.Reader.Models.Internal.ROFL
{
    /// <summary>
    /// Low level model of ROFL file, split into different sections
    /// </summary>
    public class ROFLHeader : ReplayHeader
    {
        public LengthFields LengthFields { get; set; }
        public MatchMetadata MatchMetadata { get; set; }
        public PayloadFields PayloadFields { get; set; }

        [JsonIgnore]
        public string RawJsonString { get; set; }
    }
}
