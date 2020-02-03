using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Rofl.Settings.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]

    public class ObservableSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableSettings()
        {
            KnownPlayers = new ObservableCollection<PlayerMarker>();
            PlayConfirmation = true;
            MatchHistoryBaseUrl = @"https://matchhistory.na.leagueoflegends.com/en/#match-details/NA1/";
            ItemsPerPage = 50;

            SourceFolders = new ObservableCollection<string>();

            DataDragonBaseUrl = @"http://ddragon.leagueoflegends.com/cdn/";
            ChampionRelativeUrl = @"/img/champion/";
            MapRelativeUrl = @"/img/map/map";
            ItemRelativeUrl = @"/img/item/";
        }

        public ObservableSettings(SettingsModel settings)
        {
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

            KnownPlayers = new ObservableCollection<PlayerMarker>(settings.GeneralSettings.KnownPlayers);
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
        }


        // General Settings
        public ObservableCollection<PlayerMarker> KnownPlayers { get; private set; }


        // Replay Settings
        public ObservableCollection<string> SourceFolders { get; private set; }

        private bool _playConfirmation;
        public bool PlayConfirmation
        {
            get
            {
                return _playConfirmation;
            }
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
            get
            {
                return _matchHistoryBaseUrl;
            }
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
            get { return _itemsPerPage; }
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
            get
            {
                return _dataDragonBaseUrl;
            }
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
            get
            {
                return _championRelativeUrl;
            }
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
            get
            {
                return _mapRelativeUrl;
            }
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
            get
            {
                return _itemRelativeUrl;
            }
            set
            {
                _itemRelativeUrl = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ItemRelativeUrl)));
            }
        }
    }
}
