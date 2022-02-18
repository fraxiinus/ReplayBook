namespace Fraxiinus.ReplayBook.Reader.Models.Internal.ROFL
{
    /// <summary>
    /// Low level model of metadata header in ROFL file
    /// </summary>
    public class MatchMetadata
    {
        public ulong GameDuration { get; set; }
        public string GameVersion { get; set; }
        public uint LastGameChunkID { get; set; }
        public uint LastKeyframeID { get; set; }

        public Player[] BluePlayers { get; set; }

        public Player[] RedPlayers { get; set; }
    }
}
