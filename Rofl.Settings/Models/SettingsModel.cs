using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Settings.Models
{
    public class SettingsModel
    {

        [JsonProperty("request_settings")]
        public RequestSettings RequestSettings { get; set; }

        [JsonProperty("replay_settings")]
        public ReplaySettings ReplaySettings { get; set; }

        [JsonProperty("general_settings")]
        public GeneralSettings GeneralSettings { get; set; }
    }
}
