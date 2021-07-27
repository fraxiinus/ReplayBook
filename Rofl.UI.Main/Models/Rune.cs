using Rofl.UI.Main.Utilities;
using System.ComponentModel;
using System.Windows.Media;

namespace Rofl.UI.Main.Models
{
    public class Rune : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Rune(int order, string id, string firstValue, string secondValue, string thirdValue)
        {
            GridPosition = order;
            RuneId = id;
            RuneName = RuneHelper.GetRune(id).Name;
            Value0 = int.TryParse(firstValue, out int parsedValue0) ? parsedValue0 : 0;
            Value1 = int.TryParse(secondValue, out int parsedValue1) ? parsedValue1 : 0;
            Value2 = int.TryParse(thirdValue, out int parsedValue2) ? parsedValue2 : 0;
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

        public int GridPosition { get; set; }

        public string RuneId { get; set; }

        public string RuneName { get; set; }

        public int Value0 { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }
    }
}
