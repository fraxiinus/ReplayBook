using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rofl.Settings.Models
{
    public class ReplaySettings
    {
        public ReplaySettings()
        {
            SourceFolders = new List<string>();
        }

        [JsonProperty("folder_list")]
        public List<string> SourceFolders { get; private set; }
    }
}
