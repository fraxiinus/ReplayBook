using Rofl.Executables.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Rofl.Executables.Utilities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public static class LeagueExecutableExtensions
    {
        public static Process PlayReplay(this LeagueExecutable executable, string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Replay \"{path}\" does not exist");
            }

            // This will throw an exception if exe has issues
            executable.Validate();

            // Create the launch arguments, each argument is put in quotes
            // <replay file path> GameBaseDir=... <other arguments>
            string launchArgs = $"\"{path}\" " + executable.LaunchArguments + $" \"-Locale={ExeTools.GetLocaleCode(executable.Locale)}\"";

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = executable.TargetPath,
                Arguments = launchArgs,

                // The game client uses the working directory to find the data files
                WorkingDirectory = Path.GetDirectoryName(executable.TargetPath)
            };

            Process game = Process.Start(processStartInfo);
            game.EnableRaisingEvents = true;
            return game;
        }

        public static void Validate(this LeagueExecutable executable)
        {
            // Name must not already exist
            // Start folder must exist
            // Target file must exist and be contained in start folder
            // patch version must pass versionsubstring
            // allowupdates...
            // isdefault...
            // ModifiedDate must not be null

            // Check all properties if they are null
            if (executable == null ||
                String.IsNullOrEmpty(executable.Name) ||
                String.IsNullOrEmpty(executable.TargetPath) ||
                String.IsNullOrEmpty(executable.StartFolder) ||
                String.IsNullOrEmpty(executable.PatchNumber) ||
                executable.ModifiedDate == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            // Check if start folder exists
            if (!Directory.Exists(executable.StartFolder))
            {
                // _log.Warning(_myName, $"Start folder {executable.StartFolder} does not exist");
                throw new DirectoryNotFoundException($"Start folder {executable.StartFolder} does not exist");
            }

            // Check if target path begins with stater path
            if (!executable.TargetPath.StartsWith(executable.StartFolder, StringComparison.OrdinalIgnoreCase))
            {
                // _log.Warning(_myName, $"Target file {executable.TargetPath} cannot be found from start folder {executable.StartFolder}");
                throw new FileNotFoundException($"Target file {executable.TargetPath} cannot be found from start folder {executable.StartFolder}");
            }

            // Check if target path exists
            if (!File.Exists(executable.TargetPath))
            {
                // _log.Warning(_myName, $"Target file {executable.TargetPath} not found");
                throw new FileNotFoundException($"Target file {executable.TargetPath} not found");
            }

            // Check if patch version is properly formatted
            if (executable.PatchNumber.VersionSubstring() == null)
            {
                // _log.Warning(_myName, $"Version string {executable.PatchNumber} not proper format");
                throw new ArgumentException($"Version string {executable.PatchNumber} not proper format");
            }
        }
    }
}
