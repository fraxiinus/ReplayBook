using LiteDB;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Rofl.Reader.Models
{
    public class MatchMetadata
    {
        /// <summary>
        /// Used as the ID for the database
        /// </summary>
        public ulong Id { get; set; }

        public ulong GameDuration { get; set; }
        public string GameVersion { get; set; }
        public uint LastGameChunkID { get; set; }
        public uint LastKeyframeID { get; set; }

        [JsonIgnore]
        public Dictionary<string, string>[] BluePlayers { get; set; }

        [JsonIgnore]
        public Dictionary<string, string>[] RedPlayers { get; set; }

        [BsonIgnore]
        public Dictionary<string, string>[] AllPlayers
        {
            get
            {
                return BluePlayers.Union(RedPlayers).ToArray();
            }
        }
    }
}
