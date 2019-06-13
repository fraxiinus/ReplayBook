using Rofl.Parser.Models;

namespace Rofl.Parser
{
    /*
    public struct PayloadChunkFields
    {
        public uint ChunkId;
        public byte ChunkType;
        public uint ChunkLength;
        public uint NextChunkId;
        public uint Offset;
    }

    public struct PayloadKeyframeFields
    {
        public uint KeyframeId;
        public byte KeyframeType;
        public uint KeyframeLength;
        public uint NextKeyframeId;
        public uint Offset;
    }
    */

    public class ReplayHeader
    {
        public LengthFields LengthFields;
        public MatchMetadata MatchMetadata;
        public PayloadFields MatchHeader;
    }
}
