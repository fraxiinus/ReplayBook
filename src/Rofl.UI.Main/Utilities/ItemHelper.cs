using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rofl.UI.Main.Utilities
{
    /// <summary>
    /// Item static data used to enhance data from replays
    /// </summary>
    public class ItemData
    {
        public ItemData(string id)
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
        public ItemImageData ImageData { get; set; }
    }

    public class ItemImageData
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

    public class ItemHelper
    {
        private static int AtlasCount;

        private static Dictionary<string, ItemData> ItemDictionary;

        private static Dictionary<string, BitmapFrame> ItemAtlases;

        /// <summary>
        /// Loads items from data json file. Must be called before using any other ItemHelper functions.
        /// </summary>
        /// <param name="language"></param>
        public static void LoadItems(Language language)
        {
            // instantiate dictionary
            ItemDictionary = new Dictionary<string, ItemData>();
            ItemAtlases = new Dictionary<string, BitmapFrame>();

            string targetFile = LanguageHelper.GetRiotRegionCode(language) + ".data.json";
            string itemFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "items", targetFile);

            using var r = new StreamReader(itemFile);
            string json = r.ReadToEnd();
            var itemJson = JToken.Parse(json);

            // save atlas count, for finding images
            AtlasCount = (int)itemJson["atlasCount"];

            LoadImageAtlases();

            // load all items to the dictionary
            foreach (var itemData in itemJson["data"])
            {
                ItemDictionary.Add((string)itemData["id"], itemData.ToObject<ItemData>());
            }
        }

        /// <summary>
        /// Loads image data
        /// </summary>
        private static void LoadImageAtlases()
        {
            for (int i = 0; i < AtlasCount; i++)
            {
                var atlasName = $"item{i}.png";
                string atlasPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "items", atlasName);
                ItemAtlases.Add(atlasName, ResourceTools.GetImageSourceFromPath(atlasPath));
            }
        }

        /// <summary>
        /// Given item id, returns item object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ItemData GetItemData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ItemData("-1");
            }

            if (ItemDictionary is null) { throw new Exception("run LoadRunes() method first"); }

            if (ItemDictionary.TryGetValue(id, out var item))
            {
                return item;
            }
            else
            {
                return new ItemData(id);
            }
        }

        public static ImageBrush GetItemImage(string id)
        {
            var item = GetItemData(id);
            var itemAtlas = ItemAtlases[item.ImageData.Source];

            var viewBox = new Rect(item.ImageData.X * 96 / itemAtlas.DpiX,
                                   item.ImageData.Y * 96 / itemAtlas.DpiY,
                                   item.ImageData.Width * 96 / itemAtlas.DpiX,
                                   item.ImageData.Height * 96 / itemAtlas.DpiY);

            return new ImageBrush
            {
                ImageSource = itemAtlas,
                Viewbox = viewBox,
                ViewboxUnits = BrushMappingMode.Absolute
            };
        }
    }
}
