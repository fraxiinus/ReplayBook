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

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("endOfGameStatDescs")]
        public List<string> EndOfGameStatDescs { get; private set; }
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
                Key = "UnknownRune",
                Id = id,
                Icon = ""
            };
        }

        public static (string key, string target)[] GetAllRunes()
        {
            if (RuneData is null) { throw new Exception("run LoadRunes() method first"); }

            return RuneData.Select(x => (key: x.Key, target: x.Icon)).ToArray();
        }

        public static string FillInDescriptions(string desc, string value0, string value1, string value2)
        {
            if (string.IsNullOrEmpty(desc)) { throw new ArgumentNullException(nameof(desc)); }

            desc = desc.Replace("@eogvar1@", value0);
            desc = desc.Replace("@eogvar2@", value1);
            desc = desc.Replace("@eogvar3@", value2);

            return desc;
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
