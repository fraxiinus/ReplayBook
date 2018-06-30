using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Rofl.Parser
{
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

    public struct ReplayMatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;
        public JArray Players;
    }

    public struct ReplayPayloadHeader
    {
        public ulong MatchID;
        public uint MatchLength;
        public uint KeyframeAmount;
        public uint ChunkAmount;
        public uint EndChunkID;
        public uint StartChunkID;
        public uint KeyframeInterval;
        public ushort EncryptionKeyLength;
        public string EncryptionKey; // base64
    }

    public class ReplayHeader
    {
        public ReplayLengthFields LengthFields;
        public ReplayMatchMetadata MatchMetadata;
        public ReplayPayloadHeader MatchHeader;
    }
}
