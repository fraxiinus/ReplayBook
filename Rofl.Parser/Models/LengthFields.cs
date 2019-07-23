namespace Rofl.Parsers.Models
{
    public class LengthFields
    {
        public ushort HeaderLength;
        public uint FileLength;
        public uint MetadataOffset;
        public uint MetadataLength;
        public uint PayloadHeaderOffset;
        public uint PayloadHeaderLength;
        public uint PayloadOffset;
    }
}
