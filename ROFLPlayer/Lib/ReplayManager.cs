using System;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ROFLPlayer.Lib
{
    public class ReplayManager
    {
        /*
        public static Task<RunResult<bool>> StartReplay(string replayPath, Button button)
        {
            RunResult<bool> returnValue = new RunResult<bool> { Success = false, Message = "" };
            var replayname = Path.GetFileNameWithoutExtension(replayPath);

            // Get the path of the file executable
            var gamePath = CheckGameDirValid();

            if (string.IsNullOrEmpty(gamePath) || !System.IO.File.Exists(gamePath))
            {
                returnValue.Message = "Failed to find League of Legends executable path.";
                return Task.FromResult<RunResult<bool>>(returnValue);
            }

            // Create a shortcut in the league directory
            var shortcutFile = CreateAlias(gamePath, replayPath);

            if (shortcutFile == null)
            {
                returnValue.Message = "Failed to create replay shortcut.";
                return Task.FromResult<RunResult<bool>>(returnValue);
            }

            // Run the replay
            var gamefolder = Path.GetDirectoryName(gamePath);
            var shortcutPath = Path.Combine(gamefolder, Path.GetFileNameWithoutExtension(replayPath) + ".lnk");
            var process = RunReplay(shortcutPath);

            if (process == null)
            {
                returnValue.Message = "Failed to start League of Legends.";
                return Task.FromResult<RunResult<bool>>(returnValue);
            }

            process.WaitForExit();
            button.Enabled = true;

            // Clean up when replay is done

            if (!CleanUp(shortcutPath))
            {
                returnValue.Message = "Failed to delete created replay shortcut";
                return Task.FromResult<RunResult<bool>>(returnValue);
            }

            returnValue.Success = true;
            return Task.FromResult<RunResult<bool>>(returnValue);
        }
        */
        public static Task<bool> StartReplay(string replayPath)
        {
            //RunResult<bool> returnValue = new RunResult<bool> { Success = false, Message = "" };
            var replayname = Path.GetFileNameWithoutExtension(replayPath);

            // Get the path of the file executable
            var gamePath = CheckGameDirValid();

            if (string.IsNullOrEmpty(gamePath))
            {
                throw new FileNotFoundException("Failed to find League of Legends executable path.");
                //returnValue.Message = "Failed to find League of Legends executable path.";
                //return Task.FromResult<RunResult<bool>>(returnValue);
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
            // TODO: Needs to be able to find latest directory after patch
            if(LeagueManager.CheckLeagueExecutable())
            {
                return RoflSettings.Default.LoLExecLocation;
            }
            return null;
        }

        private static IWshShortcut CreateAlias(string execPath, string replayPath)
        {
            var dir = Path.GetDirectoryName(execPath);

            var filepath = Path.Combine(dir, Path.GetFileNameWithoutExtension(replayPath) + ".lnk");
            // Create a new shortcut that launches league and includes replay path
            
            return FileManager.CreateShortcut(filepath, execPath, replayPath);
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
