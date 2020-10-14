using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rofl.Settings.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]

    public class ObservableSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableSettings()
        {
            KnownPlayers = new ObservableCollection<PlayerMarker>();
            FileAction = 0;
            PlayConfirmation = true;
            MatchHistoryBaseUrl = @"https://matchhistory.na.leagueoflegends.com/en/#match-details/NA1/";
            ItemsPerPage = 50;

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

        public ObservableSettings(SettingsModel settings)
        {
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

            KnownPlayers = new ObservableCollection<PlayerMarker>(settings.GeneralSettings.KnownPlayers);
            FileAction = settings.GeneralSettings.FileAction;
            PlayConfirmation = settings.GeneralSettings.PlayConfirmation;
            MatchHistoryBaseUrl = settings.GeneralSettings.MatchHistoryBaseUrl;

            ItemsPerPage = settings.GeneralSettings.ItemsPerPage;
            if(ItemsPerPage < 10 || ItemsPerPage > 200)
            {
                ItemsPerPage = 50;
            }

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


        // Replay Settings
        public ObservableCollection<string> SourceFolders { get; private set; }

        private int _fileAction;
        public int FileAction
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

        private string _matchHistoryBaseUrl;
        public string MatchHistoryBaseUrl
        {
            get => _matchHistoryBaseUrl;
            set
            {
                _matchHistoryBaseUrl = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(MatchHistoryBaseUrl)));
            }
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
