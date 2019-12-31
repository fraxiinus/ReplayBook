using System;
using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Rofl.Logger;
using Rofl.Executables.Models;

namespace Rofl.Executables.Utilities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public static class ExeTools
    {
        /* public static string FindLeagueExecutable(string startDir)
        {
            if (string.IsNullOrEmpty(startDir))
            {
                // _log.Warning(_myName, "Input path cannot be empty");
                throw new ArgumentNullException(nameof(startDir));
            }
            if (!Directory.Exists(startDir))
            {
                // _log.Warning(_myName, $"Input path {startDir} does not exist");
                throw new DirectoryNotFoundException($"Input path {startDir} does not exist");
            }

            // If the Game directory exists, we don't have to traverse RADS
            if(Directory.Exists(Path.Combine(startDir, "Game")))
            {
                // _log.Information(_myName, "Found Game folder");
                string exePath = Path.Combine(startDir, "Game", "League of Legends.exe");
                if (File.Exists(exePath) && CheckExecutableFile(exePath))
                {
                    // _log.Information(_myName, $"Found executable: {exePath}");
                    return exePath;
                }
                else
                {
                    // _log.Warning(_myName, "Could not find League of Legends.exe in Game folder");
                    throw new FileNotFoundException($"Could not find League of Legends.exe in Game folder");
                }
            }
            else if (Directory.Exists(Path.Combine(startDir, "RADS")))
            {
                // _log.Information(_myName, "Found RADS folder");
                return FindLeagueExecutablePathInRADS(startDir);
            }
            else
            {
                // _log.Warning(_myName, "Folder does not match known pattern");
                throw new ArgumentException($"Folder does not match known pattern");
            }
        }
        */

        public static bool CheckExecutableFile(string filePath)
        {
            if (String.IsNullOrEmpty(filePath) 
                || !filePath.Contains("League of Legends.exe") 
                || !File.Exists(filePath) 
                || !(FileVersionInfo.GetVersionInfo(filePath).FileDescription)
                        .Equals(@"League of Legends (TM) Client", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public static void ValidateLeagueExecutable(LeagueExecutable executable)
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

        public static string GetLeagueVersion(string filePath)
        {
            if (CheckExecutableFile(filePath))
            {
                return FileVersionInfo.GetVersionInfo(filePath).FileVersion;
            }
            else
            {
                // _log.Warning(_myName, $"Invalid Executable: {filePath}");
                throw new ArgumentException($"Invalid Executable: {filePath}");
            }
        }

        public static DateTime GetLastModifiedDate(string filePath)
        {
            if(CheckExecutableFile(filePath))
            {
                return (new FileInfo(filePath)).LastWriteTime;
            }
            else
            {
                // _log.Warning(_myName, $"Invalid Executable: {filePath}");
                throw new FileNotFoundException($"Invalid Executable: {filePath}");
            }
        }

        public static LeagueExecutable CreateNewLeagueExecutable(string path)
        {
            LeagueExecutable newExe = new LeagueExecutable()
            {
                TargetPath = path,
                StartFolder = Path.GetDirectoryName(path),
                PatchNumber = GetLeagueVersion(path),
                ModifiedDate = GetLastModifiedDate(path)
            };

            newExe.Name = $"Patch {newExe.PatchNumber.VersionSubstring()}";
            newExe.LaunchArguments = $"\"-GameBaseDir={newExe.StartFolder}\"" +
                                        " \"-SkipRads\"" +
                                        " \"-SkipBuild\"" +
                                        " \"-EnableLNP\"" +
                                        " \"-UseNewX3D=1\"" +
                                        " \"-UseNewX3DFramebuffers=1\"";

            return newExe;
        }

        /* private static string FindLeagueExecutablePathInRADS(string startDir)
        {
            // Browse to releases folder
            var browse = Path.Combine(startDir, "RADS", "solutions", "lol_game_client_sln", "releases");
            if (!Directory.Exists(browse))
            {
                //_log.Warning(_myName, $"Expected RADS folders do not exist, check if {browse} exists");
                throw new DirectoryNotFoundException($"Expected RADS folders do not exist, check if {browse} exists");
            }

            // Get most recent release by folder modification date
            DirectoryInfo[] folders = new DirectoryInfo(browse).GetDirectories();

            string releaseFolder = null;
            if (!folders.Any()) // No folders, something's wrong with install
            {
                releaseFolder = String.Empty;
            }
            else if (folders.Length == 1) // Only one release, choose that one
            {
                releaseFolder = folders[0].FullName;
            }
            else // More than one, use the most recent
            {
                releaseFolder = folders.OrderBy(x => x.LastWriteTime).Last().FullName;
            }

            if (String.IsNullOrEmpty(releaseFolder))
            {
                // _log.Warning(_myName, $"No release folder found in RADS");
                throw new DirectoryNotFoundException($"No release folder found in RADS");
            }

            // Browse to executable
            browse = Path.Combine(browse, releaseFolder, "deploy", "League of Legends.exe");

            if (CheckExecutableFile(browse))
            {
                return browse;
            }
            else
            {
                // _log.Warning(_myName, $"Could not find valid EXE in {browse}");
                throw new FileNotFoundException($"Could not find valid EXE in {browse}");
            }
        }*/
    }
}
