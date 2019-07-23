namespace Rofl.Reader.Models
{ 
    public class ReplayHeader
    {
        public LengthFields LengthFields;
        public MatchMetadata MatchMetadata;
        public PayloadFields PayloadFields;
        public string RawJsonData;
    }
}
