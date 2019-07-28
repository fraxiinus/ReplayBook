using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rofl.Executables.Models
{
    public class InfoFile
    {
        [JsonProperty("executables")]
        public List<LeagueExecutable> Executables { get; set; }

        [JsonProperty("default-executable")]
        public LeagueExecutable DefaultExecutable { get; set; }
    }
}
