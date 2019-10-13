using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class ReplayDetailModel
    {
        public ReplayDetailModel(ReplayFile replay, ReplayPreviewModel previewModel)
        {
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

            PreviewModel = previewModel ?? throw new ArgumentNullException(nameof(previewModel));

            BluePlayers = new List<PlayerDetailModel>();
            var combinedBluePlayers = replay.BluePlayers.Zip(previewModel.BluePreviewPlayers, (p, r) => new { Player = p, Preview = r });
            foreach (var bPlayer in combinedBluePlayers)
            {
                var newPlayer = new PlayerDetailModel(bPlayer.Player, bPlayer.Preview);
                BlueKills += newPlayer.ChampionsKilled;
                BlueDeaths += newPlayer.Deaths;
                BlueAssists += newPlayer.Assists;
                BlueGoldEarned += newPlayer.GoldEarned;
                BlueMinionsKilled += newPlayer.MinionsKilled;
                BluePlayers.Add(newPlayer);
            }

            RedPlayers = new List<PlayerDetailModel>();
            var combinedRedPlayers = replay.RedPlayers.Zip(previewModel.RedPreviewPlayers, (p, r) => new { Player = p, Preview = r });
            foreach (var rPlayer in combinedRedPlayers)
            {
                var newPlayer = new PlayerDetailModel(rPlayer.Player, rPlayer.Preview);
                RedKills += newPlayer.ChampionsKilled;
                RedDeaths += newPlayer.Deaths;
                RedAssists += newPlayer.Assists;
                RedGoldEarned += newPlayer.GoldEarned;
                RedMinionsKilled += newPlayer.MinionsKilled;
                RedPlayers.Add(newPlayer);
            }
        }

        public ReplayPreviewModel PreviewModel { get; private set; }

        public int BlueKills { get; private set; }

        public int BlueDeaths { get; private set; }

        public int BlueAssists { get; private set; }

        public int BlueGoldEarned { get; private set; }

        public int BlueMinionsKilled { get; private set; }

        public int RedKills { get; private set; }

        public int RedDeaths { get; private set; }

        public int RedAssists { get; private set; }

        public int RedGoldEarned { get; private set; }

        public int RedMinionsKilled { get; private set; }

        public IList<PlayerDetailModel> BluePlayers { get; private set; }

        public IList<PlayerDetailModel> RedPlayers { get; private set; }
    }
}
