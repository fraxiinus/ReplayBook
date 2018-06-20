using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using System.IO;

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
    }
}
