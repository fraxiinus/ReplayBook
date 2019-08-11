using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class ReplayItemModel : INotifyPropertyChanged
    {
        public string ItemName { get; set; }

        public string MapName { get; set; }

        public int GameLength { get; set; }

        public string GameLengthString
        {
            get {
                int minutes = GameLength / 60;
                int seconds = GameLength - (minutes * 60);
                return $"{minutes} m {seconds} s";
            }
        }

        public string PatchNumber { get; set; }

        public PlayerInfoModel[] BluePlayers { get; set; }

        public PlayerInfoModel[] RedPlayers { get; set; }

        public bool IsBlueVictorious { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
