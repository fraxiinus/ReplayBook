using System.Collections.Generic;
using System.Linq;

namespace Rofl.Parsers.Models
{
    public class MatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;

        public Dictionary<string, string>[] BluePlayers;
        public Dictionary<string, string>[] RedPlayers;

        public Dictionary<string, string>[] AllPlayers
        {
            get
            {
                return BluePlayers.Union(RedPlayers).ToArray();
            }
        }
    }
}
