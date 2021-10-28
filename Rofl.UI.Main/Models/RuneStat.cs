using Rofl.UI.Main.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Rofl.UI.Main.Models
{
    public class RuneStat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RuneStat(string id, string firstValue, string secondValue, string thirdValue)
        {
            RuneId = id;

            Rune staticData = RuneHelper.GetRune(id);
            RuneName = staticData.Name;
            Descriptions = new List<string>();
            for (int i = 0; i < staticData.EndOfGameStatDescs.Count; i++)
            {
                Descriptions.Add(RuneHelper.FillInDescriptions(staticData.EndOfGameStatDescs[i], firstValue, secondValue, thirdValue));
            }

            Value0 = int.TryParse(firstValue, out int parsedValue0) ? parsedValue0 : 0;
            Value1 = int.TryParse(secondValue, out int parsedValue1) ? parsedValue1 : 0;
            Value2 = int.TryParse(thirdValue, out int parsedValue2) ? parsedValue2 : 0;

            // default to error icon
            OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
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

        public System.Windows.Visibility OverlayVisible => _overlayIcon != null
            ? System.Windows.Visibility.Visible
            : System.Windows.Visibility.Collapsed;

        public string RuneId { get; set; }

        public string RuneName { get; set; }

        public List<string> Descriptions { get; private set; }

        public int Value0 { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }
    }
}
