namespace Rofl.Reader.Models
{
    public class LengthFields
    {
        /// <summary>
        /// Used as the ID for the database
        /// </summary>
        public ulong Id { get; set; }

        public ushort HeaderLength { get; set; }
        public uint FileLength { get; set; }
        public uint MetadataOffset { get; set; }
        public uint MetadataLength { get; set; }
        public uint PayloadHeaderOffset { get; set; }
        public uint PayloadHeaderLength { get; set; }
        public uint PayloadOffset { get; set; }
    }
}
