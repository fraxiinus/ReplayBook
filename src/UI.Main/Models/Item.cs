using Fraxiinus.ReplayBook.UI.Main.Utilities;
using System.ComponentModel;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Models
{
    public class Item : INotifyPropertyChanged
    {
        /// <summary>
        /// Represents an item from the replay file
        /// </summary>
        /// <param name="itemId"></param>
        public Item(string itemId)
        {
            ItemId = itemId;
            ItemName = "Item";

            // default to error icon
            OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ItemId { get; set; }

        private string _itemName;
        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ItemName)));
            }
        }

        private bool _showBorder;
        public bool ShowBorder
        {
            get => _showBorder;
            set
            {
                _showBorder = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ShowBorder)));
            }
        }

        private ImageBrush _image;
        public ImageBrush Image
        {
            get => _image;
            set
            {
                _image = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Image)));
            }
        }

        private Geometry _overlayIcon;
        public Geometry OverlayIcon
        {
            get => _overlayIcon;
            set
            {
                _overlayIcon = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(OverlayIcon)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(OverlayVisible)));
            }
        }

        public System.Windows.Visibility OverlayVisible => _overlayIcon != null
            ? System.Windows.Visibility.Visible
            : System.Windows.Visibility.Collapsed;
    }
}
