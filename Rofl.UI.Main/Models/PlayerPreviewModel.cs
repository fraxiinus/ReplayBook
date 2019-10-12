using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class PlayerPreviewModel : INotifyPropertyChanged
    {
        public PlayerPreviewModel(Player player)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }
            
            ChampionName = player.SKIN;
            PlayerName = player.NAME;
            isKnown = false;
            imgSrc = @"D:\Sync\Pictures\comissions\CalamariPop\ThinkYuumi.png";

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ChampionName { get; private set; }

        public string PlayerName { get; private set; }

        private bool isKnown;
        public bool IsKnownPlayer 
        { 
            get { return isKnown; }
            set
            {
                isKnown = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(IsKnownPlayer)));
            }
        }

        private string imgSrc;
        public string ImageSource 
        {
            get { return imgSrc; }
            set 
            {
                imgSrc = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ImageSource)));
            } 
        }

        public string CombinedName
        {
            get
            {
                return $"{PlayerName} - {ChampionName}";
            }
        }
    }
}
