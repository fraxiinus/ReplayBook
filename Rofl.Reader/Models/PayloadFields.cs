namespace Rofl.Reader.Models
{
    public class PayloadFields
    {
        /// <summary>
        /// Used as the ID for the database
        /// </summary>
        public ulong MatchId { get; set; }

        public uint MatchLength { get; set; }
        public uint KeyframeAmount { get; set; }
        public uint ChunkAmount { get; set; }
        public uint EndChunkID { get; set; }
        public uint StartChunkID { get; set; }
        public uint KeyframeInterval { get; set; }
        public ushort EncryptionKeyLength { get; set; }
        public string EncryptionKey { get; set; } // base64
    }
}
