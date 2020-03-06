using Rofl.Executables.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Rofl.Executables.Utilities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public static class ReplayPlayer
    {
        public static void Play(LeagueExecutable leagueExe, string replayPath)
        {
            if (!File.Exists(replayPath))
            {
                throw new FileNotFoundException($"Replay does not exist");
            }

            // This will throw an exception if exe has issues
            ExeTools.ValidateLeagueExecutable(leagueExe);

            // Create the launch arguments, each argument is put in quotes
            // <replay file path> GameBaseDir=... <other arguments>
            string launchArgs = $"\"{replayPath}\" " + leagueExe.LaunchArguments + $" \"-Locale={ExeTools.GetLocaleCode(leagueExe.Locale)}\"";

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = leagueExe.TargetPath,
                Arguments = launchArgs,

                // The game client uses the working directory to find the data files
                WorkingDirectory = Path.GetDirectoryName(leagueExe.TargetPath)
            };

            Process game = Process.Start(processStartInfo);
            game.WaitForExit();

            return;
        }
    }
}
