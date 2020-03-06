using System;
using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Rofl.Logger;
using Rofl.Executables.Models;
using System.Collections.Generic;

namespace Rofl.Executables.Utilities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public static class ExeTools
    {
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
                Locale = LeagueLocale.EnglishUS,
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

        public static string GetLocaleCode(LeagueLocale name)
        {
            string code = String.Empty;
            switch (name)
            {
                case LeagueLocale.Czech:
                    code = "cs_CZ";
                    break;
                case LeagueLocale.German:
                    code = "de_DE";
                    break;
                case LeagueLocale.Greek:
                    code = "el_GR";
                    break;
                case LeagueLocale.EnglishAU:
                    code = "en_AU";
                    break;
                case LeagueLocale.EnglishGB:
                    code = "en_GB";
                    break;
                case LeagueLocale.EnglishUS:
                    code = "en_US";
                    break;
                case LeagueLocale.SpanishES:
                    code = "es_ES";
                    break;
                case LeagueLocale.SpanishMX:
                    code = "es_MX";
                    break;
                case LeagueLocale.French:
                    code = "fr_FR";
                    break;
                case LeagueLocale.Hungarian:
                    code = "hu_HU";
                    break;
                case LeagueLocale.Italian:
                    code = "it_IT";
                    break;
                case LeagueLocale.Japanese:
                    code = "ja_JP";
                    break;
                case LeagueLocale.Korean:
                    code = "ko_KR";
                    break;
                case LeagueLocale.Polish:
                    code = "pl_PL";
                    break;
                case LeagueLocale.Portuguese:
                    code = "pt_BR";
                    break;
                case LeagueLocale.Romanian:
                    code = "ro_RO";
                    break;
                case LeagueLocale.Russian:
                    code = "ru_RU";
                    break;
                case LeagueLocale.Turkish:
                    code = "tr_TR";
                    break;
                default:
                    code = "en_US";
                    break;
            }
            return code;
        }

        public static string GetLocaleCode(string name)
        {
            if (Enum.TryParse<LeagueLocale>(name, out LeagueLocale result))
            {
                return GetLocaleCode(result);
            }
            else
            {
                return GetLocaleCode(LeagueLocale.EnglishUS);
            }
        }
    }
}
