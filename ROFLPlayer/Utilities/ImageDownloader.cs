using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using ROFLPlayer.Models;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ROFLPlayer.Utilities
{
    public class ImageDownloader
    {
        private static string ddragonurl = @"http://ddragon.leagueoflegends.com/cdn/";
        private static string ddragonver = "8.13.1";

        /// <summary>
        /// Set data endpoint version to replay version
        /// </summary>
        /// <param name="gameversion"></param>
        public static async void SetDataDragonVersion(string gameversion)
        {
            var inputversion = gameversion.Substring(0, gameversion.IndexOf('.', gameversion.IndexOf('.') + 1));
            var versions = await GetVersions();

            if (versions == null)
            {
                throw new NullReferenceException("Data Dragon Versions request returned null");
            }

            var versionQuery = (from version in versions
                          where version.ToString().StartsWith(inputversion)
                          select version).FirstOrDefault();

            if(versionQuery == null)
            {
                ddragonver = versions.First.ToString();
            } else
            {
                ddragonver = versionQuery.ToString();
            }
        }

        /// <summary>
        /// Download item image if it does not exist in cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<string> GetItemImageAsync(int id)
        {
            if (id == 0) { return "EMPTY"; }

            var cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "items");

            var cacheResult = CheckCache(cachePath, id.ToString());
            if(!string.IsNullOrEmpty(cacheResult))
            {
                return cacheResult;
            }

            var savepath = cachePath + $"\\{id.ToString()}.png";
            var dltask = await DownloadItemIcon(id, savepath);

            if (dltask)
            {
                return savepath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  Download map image if it does not exist in cache
        /// </summary>
        /// <param name="mapid"></param>
        /// <returns></returns>
        public static async Task<string> GetMinimapImageAsync(Maps mapid)
        {
            var cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "maps");

            var cacheResult = CheckCache(cachePath, mapid.ToString("D"));
            if (!string.IsNullOrEmpty(cacheResult))
            {
                return cacheResult;
            }

            var savepath = cachePath + $"\\{mapid.ToString("D")}.png";
            var dltask = await DownloadMapIcon(mapid, savepath);

            if (dltask)
            {
                return savepath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Download champion image if it does not exist in cache
        /// </summary>
        /// <param name="champname"></param>
        /// <returns></returns>
        public static async Task<string> GetChampionIconImageAsync(string champname)
        {
            if (string.IsNullOrEmpty(champname)) { return null; }

            var cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "champs");

            var cacheResult = CheckCache(cachePath, champname.ToUpper());
            if (!string.IsNullOrEmpty(cacheResult))
            {
                return cacheResult;
            }

            var savepath = cachePath + $"\\{champname}.png";
            var dltask = await DownloadChampionIcon(champname, savepath);

            if (dltask)
            {
                return savepath;
            }
            else
            {
                return null;
            }
        }

        private static string CheckCache(string cachePath, string fileName)
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var imgfilepath =
                (from file in Directory.GetFiles(cachePath)
                 where Path.GetFileNameWithoutExtension(file).ToUpper() == fileName
                 select file).FirstOrDefault();

            if (imgfilepath != null)
            {
                return imgfilepath;
            }

            return string.Empty;
        }

        private async static Task<JArray> GetVersions()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    var result = await wc.DownloadStringTaskAsync(@"https://ddragon.leagueoflegends.com/api/versions.json");
                    return JArray.Parse(result);
                }
                catch (WebException)
                {
                    return null;
                }
            }
        }

        private async static Task<bool> DownloadItemIcon(int itemId, string path)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    var url = new Uri(ddragonurl + ddragonver + @"/img/item/" + itemId.ToString() + ".png");
                    await wc.DownloadFileTaskAsync(url, path);
                    return true;
                }
                catch (WebException)
                {
                    return false;
                }
            }
        }

        private async static Task<bool> DownloadMapIcon(Maps mapid, string path)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    await wc.DownloadFileTaskAsync(new Uri(ddragonurl + ddragonver + @"/img/map/map" + mapid.ToString("D") + ".png"), path);
                    return true;
                }
                catch (WebException)
                {
                    return false;
                }
            }
        }

        private async static Task<bool> DownloadChampionIcon(string champname, string path)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    await wc.DownloadFileTaskAsync(new Uri(ddragonurl + ddragonver + @"/img/champion/" + champname + ".png"), path);
                    return true;
                }
                catch (WebException)
                {
                    return false;
                }
            }
        }

    }
}
