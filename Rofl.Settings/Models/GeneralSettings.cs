using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.Settings.Models
{
    public class GeneralSettings
    {
        public GeneralSettings()
        {
            KnownPlayers = new List<string>();
        }

        [JsonProperty("known_players")]
        public List<string> KnownPlayers { get; private set; }
    }
}
