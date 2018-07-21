using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Win32;

namespace ROFLPlayer.Lib
{
    public class FileManager
    {
        public static string ddragonurl = @"http://ddragon.leagueoflegends.com/cdn/";
        public static string ddragonver = "8.13.1";

        /// <summary>
        /// Creates shortcuts, modifies shortcuts if exists
        /// </summary>
        /// <param name="shortcutpath"></param>
        /// <param name="execpath"></param>
        /// <param name="replaypath"></param>
        /// <returns></returns>
        public static IWshShortcut CreateShortcut(string shortcutpath, string execpath, string replaypath)
        {
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutpath);

            shortcut.Description = "ROFL Player replay shortcut";
            shortcut.TargetPath = execpath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(shortcutpath);
            shortcut.Arguments = "\"" + replaypath + "\"";
            shortcut.Save();

            return shortcut;
        }

        /// <summary>
        /// Searches the Windows Registry for where League of Legends is installed. Path is returned if found.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FindLeagueInstallPath(out string path)
        {
            using(RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node"))
            {
                var riotSubkeyName = (from subkeyName in key.GetSubKeyNames()
                                      where subkeyName == "Riot Games, Inc"
                                      select subkeyName).FirstOrDefault();

                if (!string.IsNullOrEmpty(riotSubkeyName))
                {
                    using(RegistryKey subkey = key.OpenSubKey($@"{riotSubkeyName}\League of Legends"))
                    {
                        path = subkey.GetValue("Location").ToString();
                    }
                    RoflSettings.Default.StartFolder = path;
                    RoflSettings.Default.Save();
                    return true;
                }
            }

            path = string.Empty;
            return false;
        }

        /// <summary>
        /// Set data endpoint version to replay version
        /// </summary>
        /// <param name="gameversion"></param>
        public static async void SetDataDragonVersion(string gameversion)
        {
            var inputversion = gameversion.Substring(0, gameversion.IndexOf('.', gameversion.IndexOf('.') + 1));
            var versions = await GetVersions();
            
            if(versions == null)
            {
                throw new NullReferenceException("Data Dragon Versions request returned null");
            }

            ddragonver = (from version in versions
                          where version.ToString().StartsWith(inputversion)
                          select version).First().ToString();
        }

        /// <summary>
        /// Download item image if it does not exist in cache
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static async Task<string> GetItemImage(int itemId)
        {
            if(itemId == 0)
            {
                return null;
            }

            var cachepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "items");

            if (!Directory.Exists(cachepath))
            {
                Directory.CreateDirectory(cachepath);
            }

            var imgfilepath =
                (from file in Directory.GetFiles(cachepath)
                 where Path.GetFileNameWithoutExtension(file) == itemId.ToString()
                 select file).FirstOrDefault();

            if (imgfilepath != null)
            {
                return imgfilepath;
            }

            var savepath = cachepath + $"\\{itemId.ToString()}.png";
            var dltask = await DownloadItemIcon(itemId, savepath);

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
        public static async Task<string> GetMinimapImage(Maps mapid)
        {
            var cachepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "maps");

            if (!Directory.Exists(cachepath))
            {
                Directory.CreateDirectory(cachepath);
            }

            var imgfilepath =
                (from file in Directory.GetFiles(cachepath)
                 where Path.GetFileNameWithoutExtension(file) == mapid.ToString("D")
                 select file).FirstOrDefault();

            if (imgfilepath != null)
            {
                return imgfilepath;
            }

            var savepath = cachepath + $"\\{mapid.ToString("D")}.png";
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
        public static async Task<string> GetChampionIconImage(string champname)
        {
            if (string.IsNullOrEmpty(champname)) { return null; }

            var cachepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "champs");

            if(!Directory.Exists(cachepath))
            {
                Directory.CreateDirectory(cachepath);
            }

            var imgfilepath = 
                (from file in Directory.GetFiles(cachepath)
                where Path.GetFileNameWithoutExtension(file).ToUpper() == champname.ToUpper()
                select file).FirstOrDefault();

            if(imgfilepath != null)
            {
                return imgfilepath;
            }

            var savepath = cachepath + $"\\{champname}.png";
            var dltask = await DownloadChampionIcon(champname, savepath);

            if(dltask)
            {
                return savepath;
            }
            else
            {
                return null;
            }
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
                catch (WebException ex)
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
                catch (WebException ex)
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
                catch (WebException ex)
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
                catch (WebException ex)
                {
                    return false;
                }
            }
        }
    }
}
