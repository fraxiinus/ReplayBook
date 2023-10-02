using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Fraxiinus.ReplayBook.Configuration.Models
{
    public class ObservableConfiguration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableConfiguration(ConfigurationFile config)
        {
            ReplayFolders = config.ReplaySettings.FolderList != null
                ? new ObservableCollection<string>(config.ReplaySettings.FolderList)
                : new ObservableCollection<string>();
            SearchMinimumScore = config.ReplaySettings.SearchMinimumScore;

            DataDragonBaseUrl = config.RequestSettings.DataDragonBaseUrl;
            CommunityDragonBaseUrl = config.RequestSettings.CommunityDragonBaseUrl;
            UseCurrentLanguageAsLocale = config.RequestSettings.UseCurrentLanguageAsLocale;
            StaticDataDownloadLanguage = config.RequestSettings.StaticDataDownloadLanguage;

            PlayerMarkers = config.GeneralSettings.KnownPlayers != null
                ? new ObservableCollection<PlayerMarkerConfiguration>(config.GeneralSettings.KnownPlayers)
                : new ObservableCollection<PlayerMarkerConfiguration>();
            MarkerStyle = config.GeneralSettings.MarkerStyle;
            FileAction = config.GeneralSettings.FileAction;
            PlayConfirm = config.GeneralSettings.PlayConfirm;
            RenameFile = config.GeneralSettings.RenameFile;
            ItemsPerPage = config.GeneralSettings.ItemsPerPage;
            AutoUpdateCheck = config.GeneralSettings.AutoUpdateCheck;
            Language = config.GeneralSettings.Language;
            ReplayCategories = config.GeneralSettings.ReplayCategories != null
                ? new ObservableCollection<CategoryItem>(config.GeneralSettings.ReplayCategories)
                : new ObservableCollection<CategoryItem>();

            ThemeMode = config.AppearanceSettings.ThemeMode;
            AccentColor = config.AppearanceSettings.AccentColor;

            Stash = config.Stash;
        }

        /// Replay Settings
        public ObservableCollection<string> ReplayFolders { get; private set; }

        public int ItemsPerPage { get; set; }

        public float SearchMinimumScore { get; set; }

        /// Static Data Settings
        public string DataDragonBaseUrl { get; set; }

        public string CommunityDragonBaseUrl { get; set; }

        private bool _useCurrentLanguageAsLocale;
        public bool UseCurrentLanguageAsLocale
        {
            get => _useCurrentLanguageAsLocale;
            set
            {
                _useCurrentLanguageAsLocale = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(UseCurrentLanguageAsLocale)));
            }
        }

        public LeagueLocale StaticDataDownloadLanguage { get; set; }

        /// General Settings
        public ObservableCollection<PlayerMarkerConfiguration> PlayerMarkers { get; private set; }

        private MarkerStyle _markerStyle;
        public MarkerStyle MarkerStyle
        {
            get => _markerStyle;
            set
            {
                _markerStyle = value;
                // create new event so UI elements update
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(MarkerStyle)));
            }
        }

        public FileAction FileAction { get; set; }

        public bool PlayConfirm { get; set; }

        public bool RenameFile { get; set; }

        public bool AutoUpdateCheck { get;set; }

        private ApplicationLanguage _language;
        public ApplicationLanguage Language
        {
            get => _language;
            set
            {
                _language = value;
                // create new event so UI elements update
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Language)));
            }
        }

        public ObservableCollection<CategoryItem> ReplayCategories { get; private set; }

        public void SortReplayCategories()
        {
            ReplayCategories = new ObservableCollection<CategoryItem>(ReplayCategories.OrderBy(c => c.Name));
            // Create new event, as reassigning the variable does not event on its own!
            PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ReplayCategories)));
        }

        /// Appearance Settings
        private Theme _themeMode;
        public Theme ThemeMode
        {
            get => _themeMode;
            set
            {
                _themeMode = value;
                // create new event so UI elements update
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ThemeMode)));
            }
        }

        private string? _accentColor;
        public string? AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                // create new event so UI elements update
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(AccentColor)));
            }
        }

        public Dictionary<string, object> Stash { get; private set; }
    }
}
