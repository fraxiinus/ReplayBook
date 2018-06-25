using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ROFLPlayer.Lib
{
    public class ReplayHeader
    {
        public ReplayLengthFields LengthFields;
        public ReplayMatchMetadata MatchMetadata;
        public ReplayMatchHeader MatchHeader;
    }

    public struct ReplayMatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;
        public JArray PlayerStatsObject;
    }

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

    public struct RunResult<T>
    {
        public bool Success;
        public string Message;
        public T Result;
    }

    public struct PlayerInfo
    {
        public string Champion;
        public string Name;
        public string Team;
        public string Win;
    }

    public struct FileBaseData
    {
        public long GameLength;
        public string GameVersion;
        public PlayerInfo[] BluePlayers;
        public PlayerInfo[] PurplePlayers;
        public string WonGame;
    }

}
