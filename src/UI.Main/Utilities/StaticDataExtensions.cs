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
        /// <summary>
        /// Runes need a special helper function to get RuneData without knowing the current language.
        /// </summary>
        /// <param name="staticData"></param>
        /// <param name="runeId"></param>
        /// <param name="patchVersion"></param>
        /// <returns></returns>
        public static async Task<RuneProperties> GetRuneDataForCurrentLanguage(this StaticDataManager staticData, string runeId, string patchVersion)
        {
            var properties = await staticData.GetProperties<RuneProperties>(runeId, patchVersion, LanguageHelper.CurrentLanguage);
            
            if (properties == default)
            {
                // return blank properties
                return new RuneProperties(runeId)
                {
                    DisplayName = runeId,
                    EndOfGameStatDescs = new List<string>(),
                    IconUrl = null,
                    ImageProperties = null,
                    Key = null
                };
            }
            else
            {
                return properties;
            }
        }

        /// <summary>
        /// Creates ImageBrush object used for display in UI.
        /// Runes do not use atlases so cannot be used in this function.
        /// </summary>
        /// <param name="staticData"></param>
        /// <param name="atlas"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ImageBrush CreateBrush(this IStaticProperties staticData, BitmapFrame atlas)
        {
            if (staticData is RuneProperties)
            {
                throw new Exception("runes cannot be used with this function");
            }

            // multiply by 96 (magic number?) and divide by dpi to convert pixels to px
            var viewBox = new Rect(staticData.ImageProperties.X * 96 / atlas.DpiX,
                                   staticData.ImageProperties.Y * 96 / atlas.DpiY,
                                   staticData.ImageProperties.Width * 96 / atlas.DpiX,
                                   staticData.ImageProperties.Height * 96 / atlas.DpiY);

            var newBrush = new ImageBrush
            {
                ImageSource = atlas,
                Viewbox = viewBox,
                ViewboxUnits = BrushMappingMode.Absolute
            };

            // zoom in for champion data to remove black border
            if (staticData is ChampionProperties)
            {
                newBrush.RelativeTransform = new ScaleTransform(1.2, 1.2, 0.5, 0.5);
            }

            return newBrush;
        }

        /// <summary>
        /// Replaces variable placeholder text in rune descriptions with actual values.
        /// </summary>
        /// <param name="runeData"></param>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <param name="thirdValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<string> ReplaceRuneDescriptions(this RuneProperties runeData, string firstValue, string secondValue, string thirdValue)
        {
            var results = new List<string>();
            foreach (var description in runeData.EndOfGameStatDescs)
            {
                if (string.IsNullOrEmpty(description)) { throw new ArgumentNullException(nameof(runeData)); }

                var result = description;

                result = result.Replace("@eogvar1@", firstValue);
                result = result.Replace("@eogvar2@", secondValue);
                result = result.Replace("@eogvar3@", thirdValue);
                result = result.Replace("<br>", "\n");
                result = result.Replace(@"\n", "\n");

                results.Add(result);
            }

            return results;
        }
    }
}
