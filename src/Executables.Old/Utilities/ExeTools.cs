using System;
using System.IO;
using System.Diagnostics;
using Fraxiinus.ReplayBook.Executables.Old.Models;
using System.Linq;
using Fraxiinus.ReplayBook.Configuration.Models;

namespace Fraxiinus.ReplayBook.Executables.Old.Utilities
{
    public static class ExeTools
    {
        public static bool CheckExecutableFile(string filePath)
        {
            return !string.IsNullOrEmpty(filePath)
                && filePath.Contains("League of Legends.exe")
                && File.Exists(filePath)
                && FileVersionInfo.GetVersionInfo(filePath).FileDescription?
                    .Equals(@"League of Legends (TM) Client", StringComparison.OrdinalIgnoreCase) == true;
        }

        public static string GetLeagueVersion(string filePath)
        {
            return CheckExecutableFile(filePath)
                ? FileVersionInfo.GetVersionInfo(filePath).FileVersion
                : throw new ArgumentException($"Invalid Executable: {filePath}");
        }

        public static DateTime GetLastModifiedDate(string filePath)
        {
            return CheckExecutableFile(filePath)
                ? new FileInfo(filePath).LastWriteTime
                : throw new FileNotFoundException($"Invalid Executable: {filePath}");
        }

        /// <summary>
        /// Given a executable path, creates a new LeagueExecutable
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
            // The GameBaseDir value needs to be set to the parent folder so the game can find user config files
            newExe.LaunchArguments = $"\"-GameBaseDir={Directory.GetParent(newExe.StartFolder).FullName}\"" +
                                        " \"-SkipRads\"" +
                                        " \"-SkipBuild\"" +
                                        " \"-EnableLNP\"" +
                                        " \"-UseNewX3D=1\"" +
                                        " \"-UseNewX3DFramebuffers=1\"";
            return newExe;
        }

        /// <summary>
        /// Tries to detect locale
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static LeagueLocale DetectExecutableLocale(string path)
        {
            // Get the base directory
            string baseFolder = Path.GetDirectoryName(path);

            // Navigate to DATA folder
            string dataFolder = Path.Combine(baseFolder, "DATA", "FINAL", "Champions");

            if (!Directory.Exists(dataFolder)) { throw new DirectoryNotFoundException(dataFolder); }

            // Look for a locale file
            string code = Directory.EnumerateFiles(dataFolder, "Nidalee.*.wad.client", SearchOption.AllDirectories)
                .Select(x => Path.GetFileName(x))
                .FirstOrDefault()
                .Substring(8, 5);

            return ConfigurationDefinitions.GetLocaleEnum(code);
        }
    }
}
