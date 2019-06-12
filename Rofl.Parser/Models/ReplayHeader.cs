using System.Collections.Generic;

namespace Rofl.Parser
{
    /// <summary>
    /// Length information about the replay
    /// </summary>
    public struct LengthFields
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
    public struct MatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;
        public Dictionary<string, string> BlueTeam;
        public Dictionary<string, string> RedTeam;
    }

    /// <summary>
    /// Information about the replay
    /// </summary>
    public struct PayloadFields
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

    public class ReplayHeader
    {
        public LengthFields LengthFields;
        public MatchMetadata MatchMetadata;
        public PayloadFields MatchHeader;
    }
}
