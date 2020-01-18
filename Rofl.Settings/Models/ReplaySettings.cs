using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
