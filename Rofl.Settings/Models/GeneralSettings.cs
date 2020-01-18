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
            KnownPlayers = new List<PlayerMarker>();
        }

        [JsonProperty("known_players")]
        public List<PlayerMarker> KnownPlayers { get; private set; }
    }
}
