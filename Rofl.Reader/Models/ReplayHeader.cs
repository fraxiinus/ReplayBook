namespace Rofl.Reader.Models
{ 
    public class ReplayHeader
    {
        /// <summary>
        /// Used as the ID for the database
        /// </summary>
        public ulong Id { get; set; }

        public LengthFields LengthFields { get; set; }
        public MatchMetadata MatchMetadata { get; set; }
        public PayloadFields PayloadFields { get; set; }
        public InferredData InferredData { get; set; }
        public string RawJsonData { get; set; }
    }
}
