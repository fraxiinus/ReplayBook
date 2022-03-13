namespace Fraxiinus.ReplayBook.Reader.Models.Internal.ROFL
{
    /// <summary>
    /// Low level model of length header in ROFL file
    /// </summary>
    public class LengthFields
    {
        public ushort HeaderLength { get; set; }
        public uint FileLength { get; set; }
        public uint MetadataOffset { get; set; }
        public uint MetadataLength { get; set; }
        public uint PayloadHeaderOffset { get; set; }
        public uint PayloadHeaderLength { get; set; }
        public uint PayloadOffset { get; set; }
    }
}
