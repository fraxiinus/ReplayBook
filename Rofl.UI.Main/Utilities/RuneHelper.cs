using Newtonsoft.Json;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rofl.UI.Main.Utilities
{
    public class RuneJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public static class RuneHelper
    {
        private static List<RuneJson> RuneData;

        public static void LoadRunes(Language language)
        {
            string targetFile = GetAppropriateRegionForLanguage(language) + ".data.json";
            string runeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "runes", targetFile);

            using (StreamReader r = new StreamReader(runeFile))
            {
                string json = r.ReadToEnd();
                RuneData = JsonConvert.DeserializeObject<List<RuneJson>>(json);
            }
        }

        public static RuneJson GetRune(string id)
        {
            if (id is null) { throw new ArgumentNullException(nameof(id)); }
            if (id.Length != 4) { throw new ArgumentException("invalid rune id"); }
            if (RuneData is null) { throw new Exception("run LoadRunes() method first"); }

            string prefix = id.Substring(0, 2);

            RuneJson temp = RuneData.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            return temp ?? new RuneJson
            {
                Name = "Unknown Rune",
                Id = id,
                Icon = ""
            };
        }

        private static string GetAppropriateRegionForLanguage(Language language)
        {
            switch (language)
            {
                case Language.En:
                    return "en_US";
                case Language.ZhHans:
                    return "zh_CN";
                case Language.De:
                    return "de_DE";
                default:
                    return "en_US";
            }
        }
    }
}
