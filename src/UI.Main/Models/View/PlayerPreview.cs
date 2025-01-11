using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.UI.Main.Extensions;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Models.View;

public class PlayerPreview : INotifyPropertyChanged
{
    public PlayerPreview(PlayerStats2 player, MarkerStyle markerStyle)
    {
        if (player == null) { throw new ArgumentNullException(nameof(player)); }

        ChampionId = player.Skin;
        PlayerName = player.GetPlayerNameOrID();
        PlayerMarkerStyle = markerStyle;
        marker = null;

        // default to error icon
        OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string ChampionId { get; private set; }

    private string _championName;
    public string ChampionName
    {
        get => _championName;
        set
        {
            _championName = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ChampionName)));
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(CombinedName)));
        }
    }

    public string PlayerName { get; private set; }

    public MarkerStyle PlayerMarkerStyle { get; private set; }

    public bool IsKnownPlayer => marker != null;

    private PlayerMarkerConfiguration marker;
    public PlayerMarkerConfiguration Marker
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

    private ImageBrush _image;
    public ImageBrush Image
    {
        get => _image;
        set
        {
            _image = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(Image)));
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

    public System.Windows.Visibility OverlayVisible => _overlayIcon != null
        ? System.Windows.Visibility.Visible
        : System.Windows.Visibility.Collapsed;

    public string CombinedName
    {
        get
        {
            string combinedName = $"{PlayerName} - {ChampionName}";

            if (marker != null)
            {
                combinedName += $"\n{Marker.Note}";
            }

            return combinedName;
        }
    }
}
