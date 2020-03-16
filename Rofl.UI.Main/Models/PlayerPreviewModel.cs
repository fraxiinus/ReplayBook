using Rofl.Reader.Models;
using Rofl.Settings.Models;
using System;
using System.ComponentModel;

namespace Rofl.UI.Main.Models
{
    public class PlayerPreviewModel : INotifyPropertyChanged
    {
        public PlayerPreviewModel(Player player)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }
            
            ChampionName = player.SKIN;
            PlayerName = player.NAME;
            marker = null;
            imgSrc = @"D:\Sync\Pictures\comissions\CalamariPop\ThinkYuumi.png";

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ChampionName { get; private set; }

        public string PlayerName { get; private set; }

        public bool IsKnownPlayer 
        { 
            get 
            {
                return marker != null;
            }
        }

        private PlayerMarker marker;
        public PlayerMarker Marker
        { 
            get { return marker; }
            set
            {
                marker = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Marker)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(IsKnownPlayer)));
            }
        }

        private string imgSrc;
        public string ImageSource 
        {
            get { return imgSrc; }
            set 
            {
                imgSrc = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ImageSource)));
            } 
        }

        public string CombinedName
        {
            get
            {
                return $"{PlayerName} - {ChampionName}";
            }
        }
    }
}
