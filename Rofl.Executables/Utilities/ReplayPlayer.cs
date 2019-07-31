using Rofl.Executables.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Rofl.Executables.Utilities
{
    public class ReplayPlayer
    {
        private ExeManager _exeManager;

        private readonly string _exceptionOriginName = "Rofl.Player.ReplayPlayer";

        private string BaseDir = "-GameBaseDir=";

        // These launch arguments, including basedir, are taken from what the client does
        public List<string> LaunchArguments { get; set; } = new List<string> { "-SkipRads", "-SkipBuild", "-EnableLNP", "-UseNewX3D=1", "-UseNewX3DFramebuffers=1" };

        public ReplayPlayer(ExeManager exeManager)
        {
            _exeManager = exeManager;
        }

        public void Play(LeagueExecutable leagueExe, string replayPath)
        {
            if (!File.Exists(replayPath))
            {
                throw new FileNotFoundException($"{_exceptionOriginName} - replay does not exist");
            }

            // This will throw an exception if exe has issues
            _exeManager.ValidateExecutable(leagueExe);

            // Create the launch arguments, each argument is put in quotes
            // <replay file path> GameBaseDir=... <other arguments>
            string combinedArgs = "\"" + replayPath + "\" " + "\"" + BaseDir + leagueExe.StartFolder + "\"";
            foreach (string arg in LaunchArguments)
            {
                combinedArgs += $" \"{arg}\"";
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = leagueExe.TargetPath,
                Arguments = combinedArgs,

                // The game client uses the working directory to find the data files
                WorkingDirectory = Path.GetDirectoryName(leagueExe.TargetPath)
            };

            Process game = Process.Start(processStartInfo);
            game.WaitForExit();

            return;
        }
    }
}
