
using System.Diagnostics;
using IWshRuntimeLibrary;

public struct ReplayRunResult
{
    public bool Success;
    public string Message;
}

namespace ROFLPlayer.Lib
{
    public class ReplayManager
    {

        public ReplayRunResult StartReplay(string replayPath)
        {
            ReplayRunResult returnValue = new ReplayRunResult { Success = false, Message = "" };

            // Get the path of the file executable
            var gamePath = CheckGameDirValid();

            if (string.IsNullOrEmpty(gamePath))
            {
                returnValue.Message = "Failed to find League of Legends executable path.";
                return returnValue;
            }

            // Create a shortcut in the league directory
            var shortcutFile = CreateAlias(gamePath, replayPath);

            if(shortcutFile == null)
            {
                returnValue.Message = "Failed to create replay shortcut.";
                return returnValue;
            }

            // Run the replay
            var process = RunReplay(replayPath + @"\roflplayer_link.lnk");

            if(process == null)
            {
                returnValue.Message = "Failed to start League of Legends.";
                return returnValue;
            }

            // Clean up when replay is done

            if(!CleanUp(shortcutFile))
            {
                returnValue.Message = "Failed to delete created replay shortcut";
                return returnValue;
            }


            returnValue.Success = true;
            return returnValue;
        }

        private string CheckGameDirValid()
        {
            // Check if LOL executable directory is found
            // Needs to be able to find latest directory after patch

            return null;
        }

        private IWshShortcut CreateAlias(string execPath, string replayPath)
        {

            // Create a new shortcut that launches league and includes replay path

            return null;
        }

        private Process RunReplay(string shortcutPath)
        {
            return null;
        }

        private bool CleanUp(IWshShortcut shortcut)
        {

            return false;
        }
    }
}
