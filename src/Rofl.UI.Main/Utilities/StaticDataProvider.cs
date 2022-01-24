using Rofl.Configuration.Models;
using Rofl.UI.Main.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rofl.UI.Main.Utilities
{
    public class ChampionStaticData : StaticData
    {
        public ChampionStaticData(string id) : base(id) { }
    }

    public class ItemStaticData : StaticData
    {
        public ItemStaticData(string id) : base(id) { }
    }

    public class RuneStaticData : StaticData
    {
        public RuneStaticData(string id) : base(id) 
        {
            EndOfGameStatDescs = Array.Empty<string>();
        }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("endOfGameStatDescs")]
        public string[] EndOfGameStatDescs { get; set; }
    }

    public class StaticDataProvider
    {
        private string _riotLanguageCode;

        private readonly string _currentDirectory;

        private readonly Dictionary<string, ChampionStaticData> _championData;

        private readonly Dictionary<string, BitmapFrame> _championImages;

        private readonly Dictionary<string, ItemStaticData> _itemData;

        private readonly Dictionary<string, BitmapFrame> _itemImages;

        private readonly Dictionary<string, RuneStaticData> _runeData;

        public StaticDataProvider(Language language)
        {
            _riotLanguageCode = LanguageHelper.GetRiotRegionCode(language);
            _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _championData = new Dictionary<string, ChampionStaticData>();
            _championImages = new Dictionary<string, BitmapFrame>();
            _itemData = new Dictionary<string, ItemStaticData>();
            _itemImages = new Dictionary<string, BitmapFrame>();
            _runeData = new Dictionary<string, RuneStaticData>();
        }

        #region Available functions
        /// <summary>
        /// Load all static data from file
        /// </summary>
        /// <returns></returns>
        public async Task LoadStaticData()
        {
            await LoadFileData<ChampionStaticData>();
            await LoadFileData<ItemStaticData>();
            await LoadFileData<RuneStaticData>();
        }

        /// <summary>
        /// Reload all static data in another language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public async Task Reload(Language language)
        {
            _riotLanguageCode = LanguageHelper.GetRiotRegionCode(language);

            _championData.Clear();
            _championImages.Clear();
            _itemData.Clear();
            _itemImages.Clear();
            _runeData.Clear();

            await LoadStaticData();
        }

        /// <summary>
        /// Given champion id, returns champion object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChampionStaticData GetChampion(string id)
        {
            // return -1 champion if given id is weird
            if (string.IsNullOrEmpty(id))
            {
                return new ChampionStaticData("-1");
            }

            if (_championData.TryGetValue(id.ToUpper(CultureInfo.InvariantCulture), out var champion))
            {
                // return found champion by id
                return champion;
            }
            else
            {
                // return unknown champion by id
                return new ChampionStaticData(id);
            }
        }

        /// <summary>
        /// Given champion id, returns image (viewbox cropped)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ImageBrush GetChampionImage(string id)
        {
            var champion = GetChampion(id);
            var champAtlas = _championImages[champion.ImageData.Source];

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

        /// <summary>
        /// Given item id, returns item object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemStaticData GetItem(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ItemStaticData("-1");
            }

            if (_itemData.TryGetValue(id, out var item))
            {
                return item;
            }
            else
            {
                return new ItemStaticData(id);
            }
        }

        /// <summary>
        /// Given item id, returns image (viewbox cropped)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ImageBrush GetItemImage(string id)
        {
            var item = GetItem(id);
            var itemAtlas = _itemImages[item.ImageData.Source];

            // multiply by 96 (magic number?) and divide by dpi to convert pixels to px
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

        /// <summary>
        /// Given rune id, returns Rune object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RuneStaticData GetRune(string id)
        {
            // rune ids are always 4 long
            if (id is null || id.Length != 4)
            {
                return new RuneStaticData("-1");
            }

            if (_runeData.TryGetValue(id, out var rune))
            {
                return rune;
            }
            else
            {
                return new RuneStaticData(id);
            }
        }

        /// <summary>
        /// Returns keys and target urls for all runes
        /// </summary>
        /// <returns></returns>
        public (string key, string target)[] GetAllRunes()
        {
            return _runeData.Select(x => (key: x.Value.Key, target: x.Value.Icon)).ToArray();
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
        #endregion

        #region Internal functions
        private async Task LoadFileData<T>() where T : StaticData
        {
            string fileName = _riotLanguageCode + ".data.json";
            string targetFile = Path.Combine(_currentDirectory, "data");

            if (typeof(T) == typeof(ChampionStaticData))
            {
                targetFile = Path.Combine(targetFile, "champions", fileName);
            }
            else if (typeof(T) == typeof(ItemStaticData))
            {
                targetFile = Path.Combine(targetFile, "items", fileName);
            }
            else if (typeof(T) == typeof(RuneStaticData))
            {
                targetFile = Path.Combine(targetFile, "runes", fileName);
            }

            using FileStream stream = File.OpenRead(targetFile);
            using JsonDocument jsonDocument = await JsonDocument.ParseAsync(stream);

            // load all items to the dictionary
            foreach (JsonElement data in jsonDocument.RootElement.GetProperty("data").EnumerateArray())
            {
                if (typeof(T) == typeof(ChampionStaticData))
                {
                    // keep key uppercase
                    _championData.Add(data.GetProperty("id").GetString().ToUpper(CultureInfo.InvariantCulture), data.Deserialize<ChampionStaticData>());
                }
                else if (typeof(T) == typeof(ItemStaticData))
                {
                    // item keys are integers
                    _itemData.Add(data.GetProperty("id").GetString(), data.Deserialize<ItemStaticData>());
                }
                else if (typeof(T) == typeof(RuneStaticData))
                {
                    // item keys are integers
                    _runeData.Add(data.GetProperty("id").GetString(), data.Deserialize<RuneStaticData>());
                }
            }

            // load image thumbnails to a dictionary
            if (typeof(T) != typeof(RuneStaticData))
            {
                int atlasCount = jsonDocument.RootElement.GetProperty("atlasCount").GetInt32();
                LoadImageData<T>(atlasCount);
            }
        }

        private void LoadImageData<T>(int atlasCount) where T: StaticData
        {
            string atlasKey = "champion";
            string targetFile = Path.Combine(_currentDirectory, "data");

            if (typeof(T) == typeof(ChampionStaticData))
            {
                atlasKey = "champion";
                targetFile = Path.Combine(targetFile, "champions");
            }
            else if (typeof(T) == typeof(ItemStaticData))
            {
                atlasKey = "item";
                targetFile = Path.Combine(targetFile, "items");
            }
            else if (typeof(T) == typeof(RuneStaticData))
            {
                // rune images are loaded the old fashioned way (Rofl.Requests)
                // since no sprite sheet exists
                return;
            }


            for (int i = 0; i < atlasCount; i++)
            {
                var atlasName = $"{atlasKey}{i}.png";
                string atlasPath = Path.Combine(targetFile, atlasName);

                if (typeof(T) == typeof(ChampionStaticData))
                {
                    _championImages.Add(atlasName, ResourceTools.GetImageSourceFromPath(atlasPath));
                }
                else if (typeof(T) == typeof(ItemStaticData))
                {
                    _itemImages.Add(atlasName, ResourceTools.GetImageSourceFromPath(atlasPath));
                }
            }
        }
        #endregion
    }
}
