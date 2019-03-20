using Newtonsoft.Json.Linq;

namespace Rofl.Parser
{
    /// <summary>
    /// Length information about the replay
    /// </summary>
    public struct ReplayLengthFields
    {
        public ushort HeaderLength;
        public uint FileLength;
        public uint MetadataOffset;
        public uint MetadataLength;
        public uint PayloadHeaderOffset;
        public uint PayloadHeaderLength;
        public uint PayloadOffset;
    }

    /// <summary>
    /// Information about the match
    /// </summary>
    public struct ReplayMatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;
        public JArray Players;
    }

    /// <summary>
    /// Information about the replay
    /// </summary>
    public struct ReplayPayloadHeader
    {
        public ulong MatchId;
        public uint MatchLength;
        public uint KeyframeAmount;
        public uint ChunkAmount;
        public uint EndChunkID;
        public uint StartChunkID;
        public uint KeyframeInterval;
        public ushort EncryptionKeyLength;
        public string EncryptionKey; // base64
    }
    
    public struct ReplayChunkHeader
    {
        public uint ChunkId;
        public byte ChunkType;
        public uint ChunkLength;
        public uint NextChunkId;
        public uint Offset;
    }

    public struct ReplayKeyframeHeader
    {
        public uint KeyframeId;
        public byte KeyframeType;
        public uint KeyframeLength;
        public uint NextKeyframeId;
        public uint Offset;
    }

    public class ReplayHeader
    {
        public ReplayLengthFields LengthFields;
        public ReplayMatchMetadata MatchMetadata;
        public ReplayPayloadHeader MatchHeader;
    }
}
