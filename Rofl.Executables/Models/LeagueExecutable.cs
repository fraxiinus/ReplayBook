using System;
using Newtonsoft.Json;

namespace Rofl.Executables.Models
{
    public class LeagueExecutable
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("targetPath")]
        public string TargetPath { get; set; }

        [JsonProperty("startFolder")]
        public string StartFolder { get; set; }

        [JsonProperty("patchNumber")]
        public string PatchNumber { get; set; }

        [JsonProperty("launchArgs")]
        public string LaunchArguments { get; set; }

        [JsonProperty("dateModified")]
        public DateTime ModifiedDate { get; set; }

    }
}
