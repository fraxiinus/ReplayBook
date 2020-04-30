using Rofl.UI.Main.Extensions;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The color that is selected (user pressed "select")
        /// </summary>
        private Color _selectedColor;
        public Color SelectedColor 
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                CurrentColor = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(SelectedColor)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(SelectedColorAsHex)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(SelectedColorAsBrush)));
            }
        }

        public string SelectedColorAsHex 
        { 
            get => SelectedColor.ToHexString();
            set
            {
                SelectedColor = (Color) ColorConverter.ConvertFromString(value);
            }
        }

        public Brush SelectedColorAsBrush { get => (Brush) new SolidColorBrush(SelectedColor); }

        /// <summary>
        /// The color that is currently displayed, initally and while selecting
        /// </summary>
        private Color _currentColor;
        public Color CurrentColor 
        {
            get => _currentColor;
            set
            {
                _currentColor = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(CurrentColor)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(CurrentColorAsHex)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(CurrentColorAsBrush)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(RedAttribute)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(GreenAttribute)));
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(BlueAttribute)));
            }
        }

        public string CurrentColorAsHex
        {
            get => CurrentColor.ToHexString();
            set
            {
                CurrentColor = (Color) ColorConverter.ConvertFromString(value);
            }
        }

        public Brush CurrentColorAsBrush
        {
            get => (Brush) new SolidColorBrush(CurrentColor);
        }

        public int RedAttribute
        {
            get => CurrentColor.R;
            set => CurrentColor = Color.FromRgb((byte) value, CurrentColor.G, CurrentColor.B);
        }

        public int GreenAttribute
        {
            get => CurrentColor.G;
            set => CurrentColor = Color.FromRgb(CurrentColor.R, (byte) value, CurrentColor.B);
        }

        public int BlueAttribute
        {
            get => CurrentColor.B;
            set => CurrentColor = Color.FromRgb(CurrentColor.R, CurrentColor.G, (byte)value);
        }

        private static bool IsInRange(double num, double lo, double hi) => num >= lo && num <= hi;

        public ColorPicker()
        {
            InitializeComponent();

            // Hide the popup if we click outside
            PreviewMouseDown += (s, e) =>
            {
                if (ColorPickerPopup.IsOpen)
                {
                    Point p = e.GetPosition(ColorPickerPopup.Child);
                    if (!IsInRange(p.X, 0, ((FrameworkElement)ColorPickerPopup.Child).ActualWidth) ||
                        !IsInRange(p.Y, 0, ((FrameworkElement)ColorPickerPopup.Child).ActualHeight))
                        ColorPickerPopup.IsOpen = false;
                }
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerPopup.IsOpen = false;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = CurrentColor;
            ColorPickerPopup.IsOpen = false;
        }
    }
}
