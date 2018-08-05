using System;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ROFLPlayer.Utilities
{
    public class ReplayManager
    {
        
        public static Task<bool> StartReplay(string replayPath)
        {
            var replayname = Path.GetFileNameWithoutExtension(replayPath);

            // Get the path of the file executable
            var gamePath = CheckGameDirValid();

            if (string.IsNullOrEmpty(gamePath))
            {
                throw new FileNotFoundException("Failed to find League of Legends executable path.");
            }

            // Create a shortcut in the league directory
            var shortcutFile = CreateAlias(gamePath, replayPath);

            if(shortcutFile == null)
            {
                throw new IOException("Failed to create replay shortcut.");
            }

            // Run the replay
            var gamefolder = Path.GetDirectoryName(gamePath);
            var shortcutPath = Path.Combine(gamefolder, Path.GetFileNameWithoutExtension(replayPath) + ".lnk");
            Process process;
            try
            {
                process = RunReplay(shortcutPath);
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to start League of Legends.", ex);
            }

            process.WaitForExit();

            // Clean up when replay is done

            if (!CleanUp(shortcutPath))
            {
                throw new IOException("Failed to delete created replay shortcut.");
            }

            return Task.FromResult<bool>(true);
        }

        private static string CheckGameDirValid()
        {
            // Check if LOL executable directory is found
            if(GameLocator.CheckLeagueExecutable())
            {
                return RoflSettings.Default.LoLExecLocation;
            }
            else
            {
                if (!string.IsNullOrEmpty(RoflSettings.Default.StartFolder))
                {
                    var execPath = GameLocator.FindLeagueExecutable(RoflSettings.Default.StartFolder);
                    RoflSettings.Default.LoLExecLocation = execPath;
                    RoflSettings.Default.Save();
                    return execPath;
                }
                else
                {
                    if (GameLocator.FindLeagueInstallPath(out string path))
                    {
                        RoflSettings.Default.StartFolder = path;
                        var execPath = GameLocator.FindLeagueExecutable(path);
                        RoflSettings.Default.LoLExecLocation = execPath;
                        RoflSettings.Default.Save();
                        return execPath;
                    }
                    else
                    {
                        MessageBox.Show("Could not find League of Legends, please open ROFLPlayer and set the path", "Error playing replay", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return string.Empty;
                    }
                }
            }
        }

        private static IWshShortcut CreateAlias(string execPath, string replayPath)
        {
            var dir = Path.GetDirectoryName(execPath);

            var filepath = Path.Combine(dir, Path.GetFileNameWithoutExtension(replayPath) + ".lnk");
            // Create a new shortcut that launches league and includes replay path
            
            return Shortcuts.CreateShortcut(filepath, execPath, replayPath);
        }

        private static Process RunReplay(string shortcutPath)
        {
            // Create a new process and run the shortcut
            Process game = new Process();
            game.StartInfo.FileName = shortcutPath;
            try
            {
                game.Start();
            }
            catch (Exception)
            {
                throw;
            }
            game.WaitForExit();
            return game;
        }

        private static bool CleanUp(string shortcutPath)
        {
            try
            {
                System.IO.File.Delete(shortcutPath);
            }
            catch(Exception)
            {
                return false;
            }
            // Delete the shortcut file
            return true;
        }
    }
}
