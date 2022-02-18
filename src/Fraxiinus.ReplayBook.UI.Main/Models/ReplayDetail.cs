﻿using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fraxiinus.ReplayBook.UI.Main.Models
{
    public class ReplayDetail
    {
        public ReplayDetail(StaticDataProvider staticDataProvider, FileResult replay, ReplayPreview previewModel)
        {
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

            PreviewModel = previewModel ?? throw new ArgumentNullException(nameof(previewModel));
            FileInfo = replay.FileInfo ?? throw new ArgumentNullException(nameof(replay));

            AllPlayers = new List<PlayerDetail>();

            BluePlayers = new List<PlayerDetail>();
            var combinedBluePlayers = replay.ReplayFile.BluePlayers.Zip(previewModel.BluePreviewPlayers, (p, r) => new { Player = p, Preview = r });
            foreach (var bPlayer in combinedBluePlayers)
            {
                var newPlayer = new PlayerDetail(staticDataProvider, bPlayer.Player, bPlayer.Preview, true);
                BlueKills += newPlayer.ChampionsKilled;
                BlueDeaths += newPlayer.Deaths;
                BlueAssists += newPlayer.Assists;
                BlueGoldEarned += newPlayer.GoldEarned;

                // Includes both minions and jungle camps
                BlueMinionsKilled += newPlayer.TotalMinionsKilled;

                BluePlayers.Add(newPlayer);
                AllPlayers.Add(newPlayer);
            }

            RedPlayers = new List<PlayerDetail>();
            var combinedRedPlayers = replay.ReplayFile.RedPlayers.Zip(previewModel.RedPreviewPlayers, (p, r) => new { Player = p, Preview = r });
            foreach (var rPlayer in combinedRedPlayers)
            {
                var newPlayer = new PlayerDetail(staticDataProvider, rPlayer.Player, rPlayer.Preview, false);
                RedKills += newPlayer.ChampionsKilled;
                RedDeaths += newPlayer.Deaths;
                RedAssists += newPlayer.Assists;
                RedGoldEarned += newPlayer.GoldEarned;

                // Includes both minions and jungle camps
                RedMinionsKilled += newPlayer.TotalMinionsKilled;

                RedPlayers.Add(newPlayer);
                AllPlayers.Add(newPlayer);
            }
        }

        public ReplayPreview PreviewModel { get; private set; }

        public ReplayFileInfo FileInfo { get; private set; }

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

        public IList<PlayerDetail> BluePlayers { get; private set; }

        public IList<PlayerDetail> RedPlayers { get; private set; }

        public IList<PlayerDetail> AllPlayers { get; private set; }
    }
}
