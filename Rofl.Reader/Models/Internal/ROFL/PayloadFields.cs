namespace Rofl.Reader.Models.Internal.ROFL
{
    /// <summary>
    /// Low level model of payload header in ROFL file
    /// </summary>
    public class PayloadFields
    {
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
