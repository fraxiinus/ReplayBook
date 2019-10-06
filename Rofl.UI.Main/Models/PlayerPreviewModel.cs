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
        public event PropertyChangedEventHandler PropertyChanged;

        public string ChampionName { get; set; }

        public string PlayerName { get; set; }

        public bool IsKnownPlayer { get; set; }

        private string imgSrc;

        public string ImageSource 
        {
            get { return imgSrc; }
            set 
            {
                imgSrc = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs("ImageSource"));
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
