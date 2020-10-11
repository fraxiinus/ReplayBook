using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Rofl.UI.Main.Models
{
    public class ReplayPreview : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReplayPreview(ReplayFile replayFile, DateTimeOffset creationDate, bool newFile = false)
        {
            if (replayFile == null) { throw new ArgumentNullException(nameof(replayFile)); }

            // Copy all the replay file fields
            Name = replayFile.Name;
            GameDuration = replayFile.GameDuration;
            GameVersion = replayFile.GameVersion;
            MatchId = replayFile.MatchId;
            MapName = replayFile.MapName;
            IsBlueVictorious = replayFile.IsBlueVictorious;
            Location = replayFile.Location;

            // Set new fields
            CreationDate = creationDate;
            IsNewFile = newFile;

            BluePreviewPlayers = (from bplayer in replayFile.BluePlayers
                                  select new PlayerPreview(bplayer)).ToList();

            RedPreviewPlayers = (from rplayer in replayFile.RedPlayers
                                 select new PlayerPreview(rplayer)).ToList();

            IsPlaying = false;
            IsSelected = false;
        }

        public string Name { get; private set; }

        public TimeSpan GameDuration { get; private set; }

        public string GameVersion { get; private set; }

        public string MatchId { get; private set; }

        public string MapName { get; private set; }

        public bool IsBlueVictorious { get; private set; }

        public string Location { get; private set; }

        public DateTimeOffset CreationDate { get; set; }

        public bool IsNewFile { get; private set; }

        public bool IsSupported { get; set; }

        public string GameLengthString
        {
            get {
                return $"{(int)GameDuration.TotalMinutes} m {GameDuration.Seconds} s";
            }
        }

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
}
