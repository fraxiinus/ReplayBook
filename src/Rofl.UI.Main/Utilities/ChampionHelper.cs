using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rofl.UI.Main.Utilities
{
    /// <summary>
    /// Item static data used to enhance data from replays
    /// </summary>
    public class ChampionData
    {
        public ChampionData(string id)
        {
            Id = id;
            Name = null;
            ImageData = null;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public ChampionImageData ImageData { get; set; }
    }

    public class ChampionImageData
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }
    }

    public class ChampionHelper
    {
        private static int AtlasCount;

        private static Dictionary<string, ChampionData> ChampionDictionary;

        private static Dictionary<string, BitmapFrame> ChampionAtlases;

        /// <summary>
        /// Loads champions from data json file. Must be called before using any other ChampionHelper functions.
        /// </summary>
        /// <param name="language"></param>
        public static void LoadChampions(Language language)
        {
            // instantiate dictionary
            ChampionDictionary = new Dictionary<string, ChampionData>();
            ChampionAtlases = new Dictionary<string, BitmapFrame>();

            string targetFile = LanguageHelper.GetRiotRegionCode(language) + ".data.json";
            string champFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "champions", targetFile);

            using var r = new StreamReader(champFile);
            string json = r.ReadToEnd();
            var champJson = JToken.Parse(json);

            // save atlas count, for finding images
            AtlasCount = (int)champJson["atlasCount"];

            LoadChampionAtlases();

            // load all items to the dictionary
            foreach (var champData in champJson["data"])
            {
                ChampionDictionary.Add((string)champData["id"], champData.ToObject<ChampionData>());
            }
        }

        /// <summary>
        /// Loads champion images
        /// </summary>
        private static void LoadChampionAtlases()
        {
            if (ChampionAtlases is null) { throw new Exception("run LoadChampions() method first"); }

            for (int i = 0; i < AtlasCount; i++)
            {
                var atlasName = $"champion{i}.png";
                string atlasPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "champions", atlasName);
                ChampionAtlases.Add(atlasName, ResourceTools.GetImageSourceFromPath(atlasPath));
            }
        }

        /// <summary>
        /// Given champion id, returns champion object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ChampionData GetChampionData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ChampionData("-1");
            }

            if (ChampionDictionary is null) { throw new Exception("run LoadChampions() method first"); }

            if (ChampionDictionary.TryGetValue(id, out var champion))
            {
                return champion;
            }
            else
            {
                return new ChampionData(id);
            }
        }

        public static ImageBrush GetChampionImage(string id)
        {
            if (ChampionAtlases is null) { throw new Exception("run LoadChampions() method first"); }

            var champion = GetChampionData(id);
            var champAtlas = ChampionAtlases[champion.ImageData.Source];

            // multiply by 96 (magic number?) and divide by dpi to convert pixels to px
            var viewBox = new Rect(champion.ImageData.X * 96 / champAtlas.DpiX,
                                   champion.ImageData.Y * 96 / champAtlas.DpiY,
                                   champion.ImageData.Width * 96 / champAtlas.DpiX,
                                   champion.ImageData.Height * 96 / champAtlas.DpiY);

            return new ImageBrush
            {
                ImageSource = champAtlas,
                Viewbox = viewBox,
                ViewboxUnits = BrushMappingMode.Absolute,
                RelativeTransform = new ScaleTransform(1.2, 1.2, 0.5, 0.5) // zoom in a little bit
            };
        }
    }
}
