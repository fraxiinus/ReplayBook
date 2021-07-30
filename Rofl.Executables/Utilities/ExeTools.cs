using System;
using System.IO;
using System.Diagnostics;
using Rofl.Executables.Models;
using System.Linq;

namespace Rofl.Executables.Utilities
{
    public static class ExeTools
    {
        public static bool CheckExecutableFile(string filePath)
        {
            return !string.IsNullOrEmpty(filePath)
                && filePath.Contains("League of Legends.exe")
                && File.Exists(filePath)
                && FileVersionInfo.GetVersionInfo(filePath).FileDescription
                        .Equals(@"League of Legends (TM) Client", StringComparison.OrdinalIgnoreCase);
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
            newExe.LaunchArguments = $"\"-GameBaseDir={newExe.StartFolder}\"" +
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

            return GetLocaleEnum(code);
        }

        private static LeagueLocale GetLocaleEnum(string name)
        {
            switch (name)
            {
                case "cs_CZ":
                    return LeagueLocale.Czech;
                case "de_DE":
                    return LeagueLocale.German;
                case "el_GR":
                    return LeagueLocale.Greek;
                case "en_AU":
                    return LeagueLocale.EnglishAU;
                case "en_GB":
                    return LeagueLocale.EnglishGB;
                case "en_US":
                    return LeagueLocale.EnglishUS;
                case "es_ES":
                    return LeagueLocale.SpanishES;
                case "es_MX":
                    return LeagueLocale.SpanishMX;
                case "fr_FR":
                    return LeagueLocale.French;
                case "hu_HU":
                    return LeagueLocale.Hungarian;
                case "it_IT":
                    return LeagueLocale.Italian;
                case "ja_JP":
                    return LeagueLocale.Japanese;
                case "ko_KR":
                    return LeagueLocale.Korean;
                case "pl_PL":
                    return LeagueLocale.Polish;
                case "bt_BR":
                    return LeagueLocale.Portuguese;
                case "ro_RO":
                    return LeagueLocale.Romanian;
                case "ru_RU":
                    return LeagueLocale.Russian;
                case "tr_TR":
                    return LeagueLocale.Turkish;
                case "zh_TW":
                    return LeagueLocale.ChineseTW;
                case "zh_CN":
                    return LeagueLocale.ChineseCN;
                case "en_PH":
                    return LeagueLocale.EnglishPH;
                case "en_SG":
                    return LeagueLocale.EnglishSG;
                case "es_AR":
                    return LeagueLocale.SpanishAR;
                case "id_ID":
                    return LeagueLocale.Indonesian;
                case "th_TH":
                    return LeagueLocale.Thai;
                case "vn_VN":
                    return LeagueLocale.Vietnamese;
                case "zh_MY":
                    return LeagueLocale.ChineseMY;
                default:
                    throw new ArgumentException($"locale not found {name}");
            }
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
                case LeagueLocale.ChineseTW:
                    code = "zh_TW";
                    break;
                case LeagueLocale.ChineseCN:
                    code = "zh_CN";
                    break;
                case LeagueLocale.EnglishPH:
                    code = "en_PH";
                    break;
                case LeagueLocale.EnglishSG:
                    code = "en_SG";
                    break;
                case LeagueLocale.SpanishAR:
                    code = "es_AR";
                    break;
                case LeagueLocale.Indonesian:
                    code = "id_ID";
                    break;
                case LeagueLocale.Thai:
                    code = "th_TH";
                    break;
                case LeagueLocale.Vietnamese:
                    code = "vn_VN";
                    break;
                case LeagueLocale.ChineseMY:
                    code = "zh_MY";
                    break;
                case LeagueLocale.Custom:
                    code = "Custom";
                    break;
                default:
                    code = "en_US";
                    break;
            }
            return code;
        }

        public static string GetLocaleCode(string name)
        {
            return Enum.TryParse(name, out LeagueLocale result) ? GetLocaleCode(result) : GetLocaleCode(LeagueLocale.EnglishUS);
        }
    }
}
