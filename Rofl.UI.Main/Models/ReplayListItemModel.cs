using Rofl.Reader.Models;
using System;
using System.Linq;

namespace Rofl.UI.Main.Models
{
    public class ReplayListItemModel : ReplayFile
    {

        public ReplayListItemModel(ReplayFile replayFile, DateTimeOffset creationDate, bool newFile = false)
        {
            // Copy all the replay file fields
            Type = replayFile.Type;
            Name = replayFile.Name;
            Location = replayFile.Location;
            GameDuration = replayFile.GameDuration;
            GameVersion = replayFile.GameVersion;
            MatchId = replayFile.MatchId;
            Players = replayFile.Players;
            BluePlayers = replayFile.BluePlayers;
            RedPlayers = replayFile.RedPlayers;
            RawJsonString = replayFile.RawJsonString;
            MapId = replayFile.MapId;
            MapName = replayFile.MapName;
            IsBlueVictorious = replayFile.IsBlueVictorious;

            // Set new fields
            CreationDate = creationDate;
            IsNewFile = newFile;

            PlayerNames = (from player in Players
                           select player.NAME).ToArray();
            ChampionNames = (from player in Players
                             select player.SKIN).ToArray();

            BluePreviewPlayers = (from bplayer in BluePlayers
                                  select new PlayerPreviewModel()
                                  {
                                      ChampionName = bplayer.SKIN,
                                      PlayerName = bplayer.NAME,
                                      IsKnownPlayer = false
                                  }).ToArray();

            RedPreviewPlayers = (from rplayer in RedPlayers
                                 select new PlayerPreviewModel()
                                 {
                                     ChampionName = rplayer.SKIN,
                                     PlayerName = rplayer.NAME,
                                     IsKnownPlayer = false
                                 }).ToArray();
        }

        public DateTimeOffset CreationDate { get; set; }

        public bool IsNewFile { get; set; }

        public string GameLengthString
        {
            get {
                return $"{(int)GameDuration.TotalMinutes} m {GameDuration.Seconds} s";
            }
        }

        public string[] PlayerNames { get; set; }

        public string[] ChampionNames { get; set; }

        public PlayerPreviewModel[] BluePreviewPlayers { get; set; }

        public PlayerPreviewModel[] RedPreviewPlayers { get; set; }
    }
}
