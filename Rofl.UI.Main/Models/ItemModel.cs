using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class ItemModel : INotifyPropertyChanged
    {
        public ItemModel(string itemId)
        {
            this.ItemId = itemId;
            this.ItemName = "Item";
            this.imgSrc = @"D:\Sync\Pictures\comissions\CalamariPop\ThinkYuumi.png";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ItemId { get; set; }

        public string ItemName { get; set; }

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
    }
}
