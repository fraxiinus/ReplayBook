using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rofl.Settings.Models
{
    public class ObservableSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create using default settings
        public ObservableSettings()
        {
            KnownPlayers = new ObservableCollection<PlayerMarker>();
            PlayerMarkerStyle = MarkerStyle.Border;
            FileAction = FileAction.Open;
            PlayConfirmation = true;
            RenameAction = RenameAction.Database;
            ItemsPerPage = 50;
            AutoUpdateCheck = true;
            ProgramLanguage = Language.En;

            SourceFolders = new ObservableCollection<string>();

            DataDragonBaseUrl = @"http://ddragon.leagueoflegends.com/cdn/";
            ChampionRelativeUrl = @"/img/champion/";
            MapRelativeUrl = @"/img/map/map";
            ItemRelativeUrl = @"/img/item/";
            UseMostRecent = true;

            // Appearance
            ThemeMode = 0;
            AccentColor = null;
        }

        // Create using existing settings
        public ObservableSettings(SettingsModel settings)
        {
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

            KnownPlayers = new ObservableCollection<PlayerMarker>(settings.GeneralSettings.KnownPlayers);
            PlayerMarkerStyle = settings.GeneralSettings.PlayerMarkerStyle;
            FileAction = settings.GeneralSettings.FileAction;
            PlayConfirmation = settings.GeneralSettings.PlayConfirmation;
            RenameAction = settings.GeneralSettings.RenameAction;

            ItemsPerPage = settings.GeneralSettings.ItemsPerPage;
            if (ItemsPerPage < 10 || ItemsPerPage > 200)
            {
                ItemsPerPage = 50;
            }

            AutoUpdateCheck = settings.GeneralSettings.AutoUpdateCheck;
            ProgramLanguage = settings.GeneralSettings.ProgramLanguage;

            SourceFolders = new ObservableCollection<string>(settings.ReplaySettings.SourceFolders);

            DataDragonBaseUrl = settings.RequestSettings.DataDragonBaseUrl;
            ChampionRelativeUrl = settings.RequestSettings.ChampionRelativeUrl;
            MapRelativeUrl = settings.RequestSettings.MapRelativeUrl;
            ItemRelativeUrl = settings.RequestSettings.ItemRelativeUrl;
            UseMostRecent = settings.RequestSettings.UseMostRecent;

            // Appearence
            ThemeMode = settings.AppearanceSettings.ThemeMode;
            AccentColor = settings.AppearanceSettings.AccentColor;
        }


        // General Settings
        public ObservableCollection<PlayerMarker> KnownPlayers { get; private set; }

        private MarkerStyle _playerMarkerStyle;
        public MarkerStyle PlayerMarkerStyle
        {
            get => _playerMarkerStyle;
            set
            {
                _playerMarkerStyle = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(PlayerMarkerStyle)));
            }
        }


        // Replay Settings
        public ObservableCollection<string> SourceFolders { get; private set; }

        private FileAction _fileAction;
        public FileAction FileAction
        {
            get => _fileAction;
            set
            {
                _fileAction = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(FileAction)));
            }
        }

        private bool _playConfirmation;
        public bool PlayConfirmation
        {
            get => _playConfirmation;
            set
            {
                _playConfirmation = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(PlayConfirmation)));
            }
        }

        private RenameAction _renameAction;
        public RenameAction RenameAction
        {
            get => _renameAction;
            set
            {
                _renameAction = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(RenameAction)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(SaveNamesToFile)));
            }
        }

        public bool SaveNamesToFile
        {
            set
            {
                if (value)
                {
                    RenameAction = RenameAction.File;
                }
                else
                {
                    RenameAction = RenameAction.Database;
                }
            }
            get => RenameAction == RenameAction.File;
        }

        private int _itemsPerPage;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                _itemsPerPage = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ItemsPerPage)));
            }
        }

        private bool _autoUpdateCheck;
        public bool AutoUpdateCheck
        {
            get => _autoUpdateCheck;
            set
            {
                _autoUpdateCheck = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(AutoUpdateCheck)));
            }
        }

        private Language _programLanguage;
        public Language ProgramLanguage
        {
            get => _programLanguage;
            set
            {
                _programLanguage = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ProgramLanguage)));
            }
        }

        // Request Settings
        private string _dataDragonBaseUrl;
        public string DataDragonBaseUrl
        {
            get => _dataDragonBaseUrl;
            set
            {
                _dataDragonBaseUrl = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(DataDragonBaseUrl)));
            }
        }


        private string _championRelativeUrl;
        public string ChampionRelativeUrl
        {
            get => _championRelativeUrl;
            set
            {
                _championRelativeUrl = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ChampionRelativeUrl)));
            }
        }


        private string _mapRelativeUrl;
        public string MapRelativeUrl
        {
            get => _mapRelativeUrl;
            set
            {
                _mapRelativeUrl = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(MapRelativeUrl)));
            }
        }


        private string _itemRelativeUrl;
        public string ItemRelativeUrl
        {
            get => _itemRelativeUrl;
            set
            {
                _itemRelativeUrl = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ItemRelativeUrl)));
            }
        }

        private bool _useMostRecent;

        public bool UseMostRecent
        {
            get => _useMostRecent;
            set
            {
                _useMostRecent = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(UseMostRecent)));
            }
        }

        private int _themeMode;
        public int ThemeMode
        {
            get => _themeMode;
            set
            {
                _themeMode = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ThemeMode)));
            }
        }

        private string _accentColor;
        public string AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(AccentColor)));
            }
        }
    }
}
