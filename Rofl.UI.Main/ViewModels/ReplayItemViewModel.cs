using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.ViewModels
{
    public class ReplayItemViewModel
    {
        public ObservableCollection<ReplayItemModel> Replays { get; private set; }

        public void LoadReplays()
        {
            Replays = new ObservableCollection<ReplayItemModel>();

            Replays.Add(new ReplayItemModel()
            {
                ItemName = "Replay test item",
                MapName = "Howling Abyss",
                GameLength = 1212,
                PatchNumber = "69.69",
                IsBlueVictorious = false,
                BluePlayers = new PlayerInfoModel[]
                {
                    new PlayerInfoModel()
                    {
                        ChampionName = "Ahri",
                        PlayerName = "LostLegendLister",
                        IsKnownPlayer = true
                    },
                    new PlayerInfoModel()
                    {
                        ChampionName = "Yuumi",
                        PlayerName = "Etirps",
                        IsKnownPlayer = true
                    }
                },
                RedPlayers = new PlayerInfoModel[]
                {
                    new PlayerInfoModel()
                    {
                        ChampionName = "Tristana",
                        PlayerName = "Lord Retard",
                        IsKnownPlayer = false
                    }
                }
            });
        }
    }
}
