using System.ComponentModel;

namespace Rofl.UI.Main.Models
{
    public class Item : INotifyPropertyChanged
    {
        public Item(string itemId)
        {
            this.ItemId = itemId;
            this.ItemName = "Item";
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
