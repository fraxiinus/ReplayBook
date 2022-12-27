using System;

namespace Fraxiinus.ReplayBook.Reader.Models
{
    public class ReplayFile
    {
        public ReplayType Type { get; set; }
        
        // Name of the file
        public string Name { get; set; }

        // Full path of the file
        public string Location { get; set; }

        // These fields relate to what's inside the file
        public TimeSpan GameDuration { get; set; }

        public string GameVersion { get; set; }

        public string MatchId { get; set; }

        public Player[] Players { get; set; }

        public Player[] BluePlayers { get; set; }

        public Player[] RedPlayers { get; set; }

        public string RawJsonString { get; set; }

        // Inferred fields
        public MapCode MapId { get; set; }

        public string MapName { get; set; }

        public bool IsBlueVictorious { get; set; }

        // User assigned fields
        public string AlternativeName { get; set; }
    }
}
