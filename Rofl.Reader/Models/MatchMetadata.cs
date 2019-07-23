using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Rofl.Reader.Models
{
    public class MatchMetadata
    {
        public ulong GameDuration;
        public string GameVersion;
        public uint LastGameChunkID;
        public uint LastKeyframeID;

        [JsonIgnore]
        public Dictionary<string, string>[] BluePlayers;

        [JsonIgnore]
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
