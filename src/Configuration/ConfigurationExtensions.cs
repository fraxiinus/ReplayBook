using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Converts ObservableConfiguration to ConfigurationFile for writing to disk
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ConfigurationFile ToConfigurationFile(this ObservableConfiguration config)
        {
            return new ConfigurationFile
            {
                ReplaySettings = new ReplaySettings
                {
                    FolderList = config.ReplayFolders.ToArray()
                },
                RequestSettings = new RequestSettings
                {
                    DataDragonBaseUrl = config.DataDragonBaseUrl,
                    CommunityDragonBaseUrl = config.CommunityDragonBaseUrl,
                    UseCurrentLanguageAsLocale = config.UseCurrentLanguageAsLocale,
                    StaticDataDownloadLanguage = config.StaticDataDownloadLanguage,
                },
                GeneralSettings = new GeneralSettings
                {
                    KnownPlayers = config.PlayerMarkers.ToArray(),
                    MarkerStyle = config.MarkerStyle,
                    FileAction = config.FileAction,
                    PlayConfirm = config.PlayConfirm,
                    RenameFile = config.RenameFile,
                    ItemsPerPage = config.ItemsPerPage,
                    AutoUpdateCheck = config.AutoUpdateCheck,
                    Language = config.Language,
                    ReplayCategories = config.ReplayCategories.ToArray(),
                },
                AppearanceSettings = new AppearanceSettings
                {
                    ThemeMode = config.ThemeMode,
                    AccentColor = config.AccentColor
                },
                Stash = config.Stash
            };
        }

        /// <summary>
        /// Saves configuration file to appsettings.json, overwritting previous data.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task SaveConfigurationFile(this ConfigurationFile config)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            // creates or overwrites old file
            using FileStream stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, config, options);
            await stream.DisposeAsync();
        }

        public static string[] RemoveInvalidReplayPaths(this ObservableConfiguration config, RiZhi log)
        {
            var missingList = new List<string>();
            foreach (var path in config.ReplayFolders)
            {
                if (!Directory.Exists(path))
                {
                    log.Warning($"Replay folder {path} no longer exists, deleting...");
                    missingList.Add(path);
                }
            }

            foreach (var path in missingList)
            {
                config.ReplayFolders.Remove(path);
            }

            return missingList.ToArray();
        }
    }
}