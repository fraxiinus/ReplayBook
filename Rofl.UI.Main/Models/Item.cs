using System.ComponentModel;
using System.Windows.Media;

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

        public string ItemName { get; set; } // currently unused

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

        private ImageSource _imgSrc;
        public ImageSource ImageSource
        {
            get => _imgSrc;
            set
            {
                _imgSrc = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ImageSource)));
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

        public System.Windows.Visibility OverlayVisible
        {
            get
            {
                if (_overlayIcon != null)
                {
                    return System.Windows.Visibility.Visible;
                }

                return System.Windows.Visibility.Collapsed;
            }
        }
    }
}
