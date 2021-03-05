using Newtonsoft.Json;
using System;

namespace Rofl.Settings.Models
{
    
    public class SettingsModel
    {
        public SettingsModel()
        {
            //
        }

        public SettingsModel (ObservableSettings settings)
        {
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

            RequestSettings = new RequestSettings()
            {
                DataDragonBaseUrl = settings.DataDragonBaseUrl,
                ChampionRelativeUrl = settings.ChampionRelativeUrl,
                ItemRelativeUrl = settings.ItemRelativeUrl,
                MapRelativeUrl = settings.MapRelativeUrl,
                UseMostRecent = settings.UseMostRecent
            };
            ReplaySettings = new ReplaySettings();
            ReplaySettings.SourceFolders.AddRange(settings.SourceFolders);

            GeneralSettings = new GeneralSettings();
            GeneralSettings.KnownPlayers.AddRange(settings.KnownPlayers);
            GeneralSettings.PlayerMarkerStyle = settings.PlayerMarkerStyle;
            GeneralSettings.FileAction = settings.FileAction;
            GeneralSettings.PlayConfirmation = settings.PlayConfirmation;
            GeneralSettings.RenameAction = settings.RenameAction;
            GeneralSettings.MatchHistoryBaseUrl = settings.MatchHistoryBaseUrl;
            GeneralSettings.ItemsPerPage = settings.ItemsPerPage;
            GeneralSettings.AutoUpdateCheck = settings.AutoUpdateCheck;

            AppearanceSettings = new AppearanceSettings();
            AppearanceSettings.ThemeMode = settings.ThemeMode;
            AppearanceSettings.AccentColor = settings.AccentColor;
        }

        [JsonProperty("request_settings")]
        public RequestSettings RequestSettings { get; set; }

        [JsonProperty("replay_settings")]
        public ReplaySettings ReplaySettings { get; set; }

        [JsonProperty("general_settings")]
        public GeneralSettings GeneralSettings { get; set; }

        [JsonProperty("appearance_settings")]
        public AppearanceSettings AppearanceSettings { get; set; }
    }
}
