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
                BluePlayers.Add(new PlayerDetailModel(bPlayer.Player, bPlayer.Preview));
            }

            RedPlayers = new List<PlayerDetailModel>();
            var combinedRedPlayers = replay.RedPlayers.Zip(previewModel.RedPreviewPlayers, (p, r) => new { Player = p, Preview = r });
            foreach (var rPlayer in combinedRedPlayers)
            {
                RedPlayers.Add(new PlayerDetailModel(rPlayer.Player, rPlayer.Preview));
            }
        }

        public ReplayPreviewModel PreviewModel { get; private set; }

        public IList<PlayerDetailModel> BluePlayers { get; private set; }

        public IList<PlayerDetailModel> RedPlayers { get; private set; }
    }
}
