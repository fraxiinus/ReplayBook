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

            DataDragonBaseUrl = config.RequestSettings.DataDragonBaseUrl;

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

            ThemeMode = config.AppearanceSettings.ThemeMode;
            AccentColor = config.AppearanceSettings.AccentColor;

            Stash = config.Stash;
        }

        /// Replay Settings
        public ObservableCollection<string> ReplayFolders { get; private set; }

        /// Request Settings
        public string DataDragonBaseUrl { get; set; }

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

        public int ItemsPerPage { get; set; }

        public bool AutoUpdateCheck { get;set; }

        private Language _language;
        public Language Language
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
