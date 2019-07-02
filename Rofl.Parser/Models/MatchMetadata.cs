using System.Collections.Generic;

namespace Rofl.Parsers.Models
{
    public class MatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;
        public Dictionary<string, string>[] BlueTeam;
        public Dictionary<string, string>[] RedTeam;
    }
}
