using Rofl.Reader.Models;
using Rofl.Settings.Models;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Rofl.UI.Main.Models
{
    public class PlayerPreview : INotifyPropertyChanged
    {
        public PlayerPreview(Player player, MarkerStyle markerStyle)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }

            ChampionName = player.SKIN;
            PlayerName = player.NAME;
            PlayerMarkerStyle = markerStyle;
            marker = null;
            imgSrc = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ChampionName { get; private set; }

        public string PlayerName { get; private set; }

        public MarkerStyle PlayerMarkerStyle { get; private set; }

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

        private Geometry _overlayIcon;
        public Geometry OverlayIcon
        {
            get => _overlayIcon;
            set
            {
                _overlayIcon = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(OverlayIcon)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(OverlayVisible)));
            }
        }

        public System.Windows.Visibility OverlayVisible
        {
            get
            {
                if (_overlayIcon != null)
                {
                    return System.Windows.Visibility.Visible;
                }

                return System.Windows.Visibility.Collapsed;
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
