using Fraxiinus.ReplayBook.Files.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using Fraxiinus.ReplayBook.Configuration.Models;

namespace Fraxiinus.ReplayBook.UI.Main.Models.View;

public class ReplayPreview : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private readonly bool _showRealName;

    public ReplayPreview(FileResult file, ObservableConfiguration config)
    {
        if (file == null) { throw new ArgumentNullException(nameof(file)); }
        
        Name = file.FileName;
        AlternativeName = file.AlternativeName;
        Location = file.FileInfo.Path;
        CreationDate = file.FileCreationTime;

        if (file.ReplayFile == default)
        {
            IsErrorReplay = true;
            return;
        }
        IsErrorReplay = false;
        // Copy all the replay file fields
        GameDuration = file.ReplayFile.GameDuration;
        GameVersion = file.ReplayFile.GameVersion;
        MatchId = file.ReplayFile.MatchId;
        MapName = file.ReplayFile.MapName;
        IsBlueVictorious = file.ReplayFile.IsBlueVictorious;

        // Set new fields
        _showRealName = config.RenameFile;
        IsPlaying = false;
        IsSelected = false;

        // Setup preview players, including markers
        BluePreviewPlayers = new List<PlayerPreview>();
        RedPreviewPlayers = new List<PlayerPreview>();
        foreach (var bPlayer in file.ReplayFile.BluePlayers)
        {
            var bluePlayer = new PlayerPreview(bPlayer, config.MarkerStyle);
            bluePlayer.Marker = config.PlayerMarkers
                .FirstOrDefault(x => x.Name.Equals(bluePlayer.PlayerName, StringComparison.OrdinalIgnoreCase));

            BluePreviewPlayers.Add(bluePlayer);
        }
        foreach (var rPlayer in file.ReplayFile.RedPlayers)
        {
            var redPlayer = new PlayerPreview(rPlayer, config.MarkerStyle);
            redPlayer.Marker = config.PlayerMarkers
                .FirstOrDefault(x => x.Name.Equals(redPlayer.PlayerName, StringComparison.OrdinalIgnoreCase));

            RedPreviewPlayers.Add(redPlayer);
        }
    }

    public string Name { get; private set; }

    public bool IsErrorReplay { get; private set; }

    public string AlternativeName { get; private set; }

    private string _displayName;
    /// <summary>
    /// Returns the name to be displayed
    /// Setting this only changes the displayed value and will not save
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(_displayName)) { return _showRealName ? Name : AlternativeName; }
            else { return _displayName; }
        }
        set
        {
            _displayName = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(DisplayName)));
        }
    }

    public TimeSpan GameDuration { get; private set; }

    public string GameDurationString => $"{(int)GameDuration.TotalMinutes} m {GameDuration.Seconds} s";

    public string GameVersion { get; private set; }

    public string GameVersionString => "Patch " + GameVersion?.VersionSubstring();

    public string MatchId { get; private set; }

    public string MapName { get; private set; }

    public bool IsBlueVictorious { get; private set; }

    public string Location { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public bool IsSupported { get; set; }

    public IList<PlayerPreview> BluePreviewPlayers { get; private set; }

    public IList<PlayerPreview> RedPreviewPlayers { get; private set; }

    private bool _isPlaying;
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(IsPlaying)));
        }
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    private bool _isHovered;
    public bool IsHovered
    {
        get => _isHovered;
        set
        {
            _isHovered = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(IsHovered)));
        }
    }
}
