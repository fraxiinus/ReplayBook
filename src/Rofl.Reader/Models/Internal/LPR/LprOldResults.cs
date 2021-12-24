namespace Rofl.Reader.Models.Internal.LPR
{
    public class LprOldResults
    {
        public int NumberOfPlayers { get; set; }
        
        //public int PlayerNameLength { get; set; }
        public string PlayerName { get; set; }

        //public int ChampionNameLength { get; set; }
        public string ChampionName { get; set; }

        public int Team { get; set; }
        public int ClientID { get; set; }
    }
}
