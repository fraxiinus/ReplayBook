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
        public uint MatchHeaderOffset;
        public uint MatchHeaderLength;
        public uint MatchOffset;
    }

    public struct ReplayMatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;
        public JArray Players;
    }

    public struct ReplayMatchHeader
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
        public ReplayMatchHeader MatchHeader;
    }
}
