using Fraxiinus.ReplayBook.UI.Main.Extensions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ColorWrapper.xaml
    /// </summary>
    public partial class ColorWrapper : UserControl
    {
        public ColorWrapper()
        {
            InitializeComponent();
        }

        public Popup GetColorPickerPopup => ColorPickerPopup;

        public Color SelectedColor
        {
            get => (Color)ColorPickerContent.SelectedColor;
            set => ColorPickerContent.SelectedColor = value;
        }

        public string SelectedColorHex
        {
            get => ColorPickerContent.SelectedColor?.ToHexString();
            set => ColorPickerContent.SelectedColor = (Color)ColorConverter.ConvertFromString(value);
        }

        public Brush SelectedColorAsBrush => new SolidColorBrush((Color)ColorPickerContent.SelectedColor);

    }
}
