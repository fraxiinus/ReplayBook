using System.IO;
using IWshRuntimeLibrary;

namespace ROFLPlayer.Utilities
{
    public class Shortcuts
    {
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
    }
}
