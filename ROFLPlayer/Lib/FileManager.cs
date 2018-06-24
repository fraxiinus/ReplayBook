using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using System.Net;
using System.IO;
using System.Threading;

namespace ROFLPlayer.Lib
{
    public class FileManager
    {

        // Creates shortcuts, modifies shortcuts if exists
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

        // If the image is not downloaded already, then download it, otherwise return already downloaded image
        public static async Task<string> GetChampionIconImage(string champname)
        {
            Thread.Sleep(10000);
            if (string.IsNullOrEmpty(champname)) { return null; }

            var cachepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

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
            var dltask = await Task.Run<bool>(() => DownloadChampionIcon(champname, savepath));

            if(dltask)
            {
                return savepath;
            }
            else
            {
                return null;
            }

        }

        private static bool DownloadChampionIcon(string champname, string path)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadFile(new Uri(@"http://ddragon.leagueoflegends.com/cdn/8.12.1/img/champion/" + champname + ".png"), path);
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
