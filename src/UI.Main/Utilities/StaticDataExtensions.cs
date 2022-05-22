using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
{
    public static class StaticDataExtensions
    {
        public static async Task<ChampionData> GetChampionDataForCurrentLanguage(this StaticDataManager staticData, string championId, string patchVersion)
        {
            return await staticData.GetChampionData(championId, LanguageHelper.CurrentLanguage, patchVersion);
        }

        public static async Task<ItemData> GetItemDataForCurrentLanguage(this StaticDataManager staticData, string itemId, string patchVersion)
        {
            return await staticData.GetItemData(itemId, LanguageHelper.CurrentLanguage, patchVersion);
        }

        public static async Task<RuneData> GetRuneDataForCurrentLanguage(this StaticDataManager staticData, string runeId, string patchVersion)
        {
            return await staticData.GetRuneData(runeId, LanguageHelper.CurrentLanguage, patchVersion);
        }

        public static ImageBrush CreateItemBrush(this ItemData itemData, BitmapFrame itemAtlas)
        {
            // multiply by 96 (magic number?) and divide by dpi to convert pixels to px
            var viewBox = new Rect(itemData.ImageProperties.X * 96 / itemAtlas.DpiX,
                                   itemData.ImageProperties.Y * 96 / itemAtlas.DpiY,
                                   itemData.ImageProperties.Width * 96 / itemAtlas.DpiX,
                                   itemData.ImageProperties.Height * 96 / itemAtlas.DpiY);

            return new ImageBrush
            {
                ImageSource = itemAtlas,
                Viewbox = viewBox,
                ViewboxUnits = BrushMappingMode.Absolute
            };
        }

        public static ImageBrush CreateChampionBrush(this ChampionData champData, BitmapFrame championAtlas)
        {
            // multiply by 96 (magic number?) and divide by dpi to convert pixels to px
            var viewBox = new Rect(champData.ImageProperties.X * 96 / championAtlas.DpiX,
                                   champData.ImageProperties.Y * 96 / championAtlas.DpiY,
                                   champData.ImageProperties.Width * 96 / championAtlas.DpiX,
                                   champData.ImageProperties.Height * 96 / championAtlas.DpiY);

            return new ImageBrush
            {
                ImageSource = championAtlas,
                Viewbox = viewBox,
                ViewboxUnits = BrushMappingMode.Absolute,
                RelativeTransform = new ScaleTransform(1.2, 1.2, 0.5, 0.5) // zoom in a little bit
            };
        }

        public static IEnumerable<string> FillRuneDescriptions(this RuneData runeData, string firstValue, string secondValue, string thirdValue)
        {
            var results = new List<string>();
            foreach (var description in runeData.EndOfGameStatDescs)
            {
                if (string.IsNullOrEmpty(description)) { throw new ArgumentNullException(nameof(description)); }

                var result = description;

                result = result.Replace("@eogvar1@", firstValue);
                result = result.Replace("@eogvar2@", secondValue);
                result = result.Replace("@eogvar3@", thirdValue);

                results.Add(result);
            }

            return results;
        }

        ///// <summary>
        ///// Given champion id, returns champion object
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ChampionStaticData GetChampion(string id)
        //{
        //    // return -1 champion if given id is weird
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return new ChampionStaticData("-1");
        //    }

        //    if (_championData.TryGetValue(id.ToUpper(CultureInfo.InvariantCulture), out var champion))
        //    {
        //        // return found champion by id
        //        return champion;
        //    }
        //    else
        //    {
        //        // return unknown champion by id
        //        return new ChampionStaticData(id);
        //    }
        //}

        ///// <summary>
        ///// Given champion id, returns image (viewbox cropped)
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ImageBrush GetChampionImage(string id)
        //{
        //    var champion = GetChampion(id);
        //    var champAtlas = _championImages[champion.ImageData.Source];

        //    // multiply by 96 (magic number?) and divide by dpi to convert pixels to px
        //    var viewBox = new Rect(champion.ImageData.X * 96 / champAtlas.DpiX,
        //                           champion.ImageData.Y * 96 / champAtlas.DpiY,
        //                           champion.ImageData.Width * 96 / champAtlas.DpiX,
        //                           champion.ImageData.Height * 96 / champAtlas.DpiY);

        //    return new ImageBrush
        //    {
        //        ImageSource = champAtlas,
        //        Viewbox = viewBox,
        //        ViewboxUnits = BrushMappingMode.Absolute,
        //        RelativeTransform = new ScaleTransform(1.2, 1.2, 0.5, 0.5) // zoom in a little bit
        //    };
        //}

        ///// <summary>
        ///// Given item id, returns item object
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ItemStaticData GetItem(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return new ItemStaticData("-1");
        //    }

        //    if (_itemData.TryGetValue(id, out var item))
        //    {
        //        return item;
        //    }
        //    else
        //    {
        //        return new ItemStaticData(id);
        //    }
        //}

        ///// <summary>
        ///// Given item id, returns image (viewbox cropped)
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ImageBrush GetItemImage(string id)
        //{
        //    var item = GetItem(id);
        //    var itemAtlas = _itemImages[item.ImageData.Source];

        //    // multiply by 96 (magic number?) and divide by dpi to convert pixels to px
        //    var viewBox = new Rect(item.ImageData.X * 96 / itemAtlas.DpiX,
        //                           item.ImageData.Y * 96 / itemAtlas.DpiY,
        //                           item.ImageData.Width * 96 / itemAtlas.DpiX,
        //                           item.ImageData.Height * 96 / itemAtlas.DpiY);

        //    return new ImageBrush
        //    {
        //        ImageSource = itemAtlas,
        //        Viewbox = viewBox,
        //        ViewboxUnits = BrushMappingMode.Absolute
        //    };
        //}

        ///// <summary>
        ///// Given rune id, returns Rune object
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public RuneStaticData GetRune(string id)
        //{
        //    // rune ids are always 4 long
        //    if (id is null || id.Length != 4)
        //    {
        //        return new RuneStaticData("-1");
        //    }

        //    if (_runeData.TryGetValue(id, out var rune))
        //    {
        //        return rune;
        //    }
        //    else
        //    {
        //        return new RuneStaticData(id);
        //    }
        //}

        ///// <summary>
        ///// Returns keys and target urls for all runes
        ///// </summary>
        ///// <returns></returns>
        //public (string key, string target)[] GetAllRunes()
        //{
        //    return _runeData.Select(x => (key: x.Value.Key, target: x.Value.Icon)).ToArray();
        //}

        ///// <summary>
        ///// Fills rune stats into descriptions
        ///// </summary>
        ///// <param name="desc"></param>
        ///// <param name="value0"></param>
        ///// <param name="value1"></param>
        ///// <param name="value2"></param>
        ///// <returns></returns>
        //public static string FillInDescriptions(string desc, string value0, string value1, string value2)
        //{
        //    if (string.IsNullOrEmpty(desc)) { throw new ArgumentNullException(nameof(desc)); }

        //    desc = desc.Replace("@eogvar1@", value0);
        //    desc = desc.Replace("@eogvar2@", value1);
        //    desc = desc.Replace("@eogvar3@", value2);

        //    return desc;
        //}
        //#endregion

        //#region Internal functions
        //private async Task LoadFileData<T>() where T : StaticDataBaseClass
        //{
        //    string fileName = _riotLanguageCode + ".data.json";
        //    string targetFile = Path.Combine(_currentDirectory, "data");

        //    if (typeof(T) == typeof(ChampionStaticData))
        //    {
        //        targetFile = Path.Combine(targetFile, "champions", fileName);
        //    }
        //    else if (typeof(T) == typeof(ItemStaticData))
        //    {
        //        targetFile = Path.Combine(targetFile, "items", fileName);
        //    }
        //    else if (typeof(T) == typeof(RuneStaticData))
        //    {
        //        targetFile = Path.Combine(targetFile, "runes", fileName);
        //    }

        //    using FileStream stream = File.OpenRead(targetFile);
        //    using JsonDocument jsonDocument = await JsonDocument.ParseAsync(stream);

        //    // load all items to the dictionary
        //    foreach (JsonElement data in jsonDocument.RootElement.GetProperty("data").EnumerateArray())
        //    {
        //        if (typeof(T) == typeof(ChampionStaticData))
        //        {
        //            // keep key uppercase
        //            _championData.Add(data.GetProperty("id").GetString().ToUpper(CultureInfo.InvariantCulture), data.Deserialize<ChampionStaticData>());
        //        }
        //        else if (typeof(T) == typeof(ItemStaticData))
        //        {
        //            // item keys are integers
        //            _itemData.Add(data.GetProperty("id").GetString(), data.Deserialize<ItemStaticData>());
        //        }
        //        else if (typeof(T) == typeof(RuneStaticData))
        //        {
        //            // item keys are integers
        //            _runeData.Add(data.GetProperty("id").GetString(), data.Deserialize<RuneStaticData>());
        //        }
        //    }

        //    // load image thumbnails to a dictionary
        //    if (typeof(T) != typeof(RuneStaticData))
        //    {
        //        int atlasCount = jsonDocument.RootElement.GetProperty("atlasCount").GetInt32();
        //        LoadImageData<T>(atlasCount);
        //    }
        //}

        //private void LoadImageData<T>(int atlasCount) where T: StaticDataBaseClass
        //{
        //    string atlasKey = "champion";
        //    string targetFile = Path.Combine(_currentDirectory, "data");

        //    if (typeof(T) == typeof(ChampionStaticData))
        //    {
        //        atlasKey = "champion";
        //        targetFile = Path.Combine(targetFile, "champions");
        //    }
        //    else if (typeof(T) == typeof(ItemStaticData))
        //    {
        //        atlasKey = "item";
        //        targetFile = Path.Combine(targetFile, "items");
        //    }
        //    else if (typeof(T) == typeof(RuneStaticData))
        //    {
        //        // rune images are loaded the old fashioned way (Rofl.Requests)
        //        // since no sprite sheet exists
        //        return;
        //    }


        //    for (int i = 0; i < atlasCount; i++)
        //    {
        //        var atlasName = $"{atlasKey}{i}.png";
        //        string atlasPath = Path.Combine(targetFile, atlasName);

        //        if (typeof(T) == typeof(ChampionStaticData))
        //        {
        //            _championImages.Add(atlasName, ResourceTools.GetImageSourceFromPath(atlasPath));
        //        }
        //        else if (typeof(T) == typeof(ItemStaticData))
        //        {
        //            _itemImages.Add(atlasName, ResourceTools.GetImageSourceFromPath(atlasPath));
        //        }
        //    }
        //}
        //#endregion
    }
}
