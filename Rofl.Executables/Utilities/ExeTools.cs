using System;
using System.IO;
using System.Diagnostics;
using Rofl.Executables.Models;

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
            string code;
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
