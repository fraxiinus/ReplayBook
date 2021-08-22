using Newtonsoft.Json;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rofl.UI.Main.Utilities
{
    /// <summary>
    /// Data structure to contain rune data. Seperate from rune stats.
    /// </summary>
    public class Rune
    {
        public Rune(string id, string name, string key, string icon, List<string> descriptions)
        {
            Id = id;
            Name = name;
            Key = key;
            Icon = icon;
            EndOfGameStatDescs = descriptions;
        }

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
        private static List<Rune> RuneData;

        /// <summary>
        /// Loads runes from data json file. Must be called before using any other RuneHelper functions.
        /// </summary>
        /// <param name="language"></param>
        public static void LoadRunes(Language language)
        {
            string targetFile = LanguageHelper.GetRiotRegionCode(language) + ".data.json";
            string runeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "runes", targetFile);

            using (StreamReader r = new StreamReader(runeFile))
            {
                string json = r.ReadToEnd();
                RuneData = JsonConvert.DeserializeObject<List<Rune>>(json);
            }
        }

        /// <summary>
        /// Given rune id, returns Rune object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Rune GetRune(string id)
        {
            if (id is null || id.Length != 4)
            {
                return new Rune("-1", "N/A", "UnknownRune", "", new List<string>());
            }

            if (RuneData is null) { throw new Exception("run LoadRunes() method first"); }

            string prefix = id.Substring(0, 2);

            Rune temp = RuneData.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            return temp ?? new Rune("-1", "N/A", "UnknownRune", "", new List<string>());
        }

        /// <summary>
        /// Gets all runes
        /// </summary>
        /// <returns></returns>
        public static (string key, string target)[] GetAllRunes()
        {
            return RuneData is null
                ? throw new Exception("run LoadRunes() method first")
                : RuneData.Select(x => (key: x.Key, target: x.Icon)).ToArray();
        }

        /// <summary>
        /// Fills rune stats into descriptions
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="value0"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static string FillInDescriptions(string desc, string value0, string value1, string value2)
        {
            if (string.IsNullOrEmpty(desc)) { throw new ArgumentNullException(nameof(desc)); }

            desc = desc.Replace("@eogvar1@", value0);
            desc = desc.Replace("@eogvar2@", value1);
            desc = desc.Replace("@eogvar3@", value2);

            return desc;
        }
    }
}
