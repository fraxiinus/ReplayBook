using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rofl.UI.Main.Extensions;

namespace Rofl.UI.Main.Controls
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

        public Color SelectedColor
        {
            get => (Color) ColorPickerContent.SelectedColor;
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
