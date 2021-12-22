using Newtonsoft.Json;
using System.Collections.Generic;


namespace Rofl.Settings.Models
{
    public class AppearanceSettings
    {
        public AppearanceSettings()
        {

        }

        
        [JsonProperty("theme_mode")]

        // 0 = Use Default, 1 = Dark, 2 = Light
        public int ThemeMode { get; set; }

        // Null = using Windows, Hex otherwise
        [JsonProperty("accent_color")]
        public string AccentColor { get; set; }
    }
}
