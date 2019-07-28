using System;
using Newtonsoft.Json;

namespace Rofl.Executables.Models
{
    public class LeagueExecutable
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("target-path")]
        public string TargetPath { get; set; }

        [JsonProperty("start-folder")]
        public string StartFolder { get; set; }

        [JsonProperty("patch-version")]
        public string PatchVersion { get; set; }

        [JsonProperty("allow-updates")]
        public bool AllowUpdates { get; set; }

        [JsonProperty("default")]
        public bool IsDefault { get; set; }

        [JsonProperty("date-modified")]
        public DateTime ModifiedDate { get; set; }

        public override string ToString()
        {
            return $"{Name}\tUpdates:{AllowUpdates}\n{TargetPath}";
        }
    }
}
