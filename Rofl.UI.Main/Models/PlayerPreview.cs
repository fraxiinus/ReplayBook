using Rofl.Reader.Models;
using Rofl.Settings.Models;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Rofl.UI.Main.Models
{
    public class PlayerPreview : INotifyPropertyChanged
    {
        public PlayerPreview(Player player)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }

            ChampionName = player.SKIN;
            PlayerName = player.NAME;
            marker = null;
            imgSrc = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ChampionName { get; private set; }

        public string PlayerName { get; private set; }

        public bool IsKnownPlayer => marker != null;

        private PlayerMarker marker;
        public PlayerMarker Marker
        {
            get => marker;
            set
            {
                marker = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Marker)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(IsKnownPlayer)));
            }
        }

        private ImageSource imgSrc;
        public ImageSource ImageSource
        {
            get => imgSrc;
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
                if (Marker == null)
                {
                    return $"{PlayerName} - {ChampionName}";
                }
                else if (String.IsNullOrWhiteSpace(Marker.Note))
                {
                    return $"{PlayerName} - {ChampionName}";
                }
                else
                {
                    return $"{PlayerName} - {ChampionName}\n{Marker.Note}";
                }
            }
        }
    }
}
