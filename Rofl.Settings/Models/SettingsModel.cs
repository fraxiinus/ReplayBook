using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
                MapRelativeUrl = settings.MapRelativeUrl
            };
            ReplaySettings = new ReplaySettings();
            ReplaySettings.SourceFolders.AddRange(settings.SourceFolders);

            GeneralSettings = new GeneralSettings();
            GeneralSettings.KnownPlayers.AddRange(settings.KnownPlayers);
            GeneralSettings.PlayConfirmation = settings.PlayConfirmation;
            GeneralSettings.MatchHistoryBaseUrl = settings.MatchHistoryBaseUrl;
            GeneralSettings.ItemsPerPage = settings.ItemsPerPage;
        }

        [JsonProperty("request_settings")]
        public RequestSettings RequestSettings { get; set; }

        [JsonProperty("replay_settings")]
        public ReplaySettings ReplaySettings { get; set; }

        [JsonProperty("general_settings")]
        public GeneralSettings GeneralSettings { get; set; }
    }
}
