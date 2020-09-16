using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rofl.UI.Main.Controls.akr
{
    /// <summary>
    /// ColorPicker.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();

            byte Alpha = 255;

            var colors = new List<Color>();

            for (byte j = 128; j < 255; j++)
                colors.Add(new Color { A = Alpha, R = j, G = 255, B = 0 });

            for (byte j = 0; j < 255; j++)
                colors.Add(new Color { A = Alpha, R = 255, G = (byte)(255 - j), B = 0 });

            for (byte j = 0; j < 255; j++)
                colors.Add(new Color { A = Alpha, R = 255, G = 0, B = j });

            for (byte j = 0; j < 255; j++)
                colors.Add(new Color { A = Alpha, R = (byte)(255 - j), G = 0, B = 255 });

            for (byte j = 0; j < 255; j++)
                colors.Add(new Color { A = Alpha, R = 0, G = j, B = 255 });

            for (byte j = 0; j < 255; j++)
                colors.Add(new Color { A = Alpha, R = 0, G = 255, B = (byte)(255 - j) });

            for (byte j = 0; j < 127; j++)
                colors.Add(new Color { A = Alpha, R = j, G = 255, B = 0 });

            List<Border> labels = colors.Select(color => new Border { Height = 22, Width = 2, Background = new SolidColorBrush(color), BorderThickness = new Thickness(0) }).ToList();
            foreach (var label in labels)
                panel.Children.Add(label);

            ColorShadeSelector.RenderTransform = colorShadeSelectorTransform;

            var RSliderData = new SliderData { Canvas = RCanvas, SelectorBorder = RSelector, Type = ColorType.R };
            RCanvas.Tag = RSliderData;
            RSelector.RenderTransform = RSliderData.SelectorTransform;
            var GSliderData = new SliderData { Canvas = GCanvas, SelectorBorder = GSelector, Type = ColorType.G };
            GCanvas.Tag = GSliderData;
            GSelector.RenderTransform = GSliderData.SelectorTransform;
            var BSliderData = new SliderData { Canvas = BCanvas, SelectorBorder = BSelector, Type = ColorType.B };
            BCanvas.Tag = BSliderData;
            BSelector.RenderTransform = BSliderData.SelectorTransform;
            //var ASliderData = new SliderData { Canvas = ACanvas, SelectorBorder = ASelector, Type = ColorType.A };
            //ACanvas.Tag = ASliderData;
            //ASelector.RenderTransform = ASliderData.SelectorTransform;


            DataContext = this;
            SliderStackPanel.DataContext = CurrentViewColor;
            RTextBox.DataContext = this;
            GTextBox.DataContext = this;
            BTextBox.DataContext = this;
            //ATextBox.DataContext = this;
            ColorShadingRectangle.DataContext = CurrentViewColor;
        }

        private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedColor == null) SelectedColor = Color.FromArgb(0xff, 0xff, 0, 0);
            CurrentViewColor.SetColor(SelectedColor);
        }

        #region Private Members

        private TranslateTransform colorShadeSelectorTransform = new TranslateTransform();
        private Point? currentColorShadePosition;
        private double currentSpectrumValue = 0;
        private bool stopPropertyChanged;
        private bool doUpdateSpectrumSliderValue = true;
        private ViewColor CurrentViewColor = new ViewColor();

        #endregion //Private Members

        #region Events

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectedColorChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color?>), typeof(ColorPicker));
        public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
        {
            add
            {
                AddHandler(SelectedColorChangedEvent, value);
            }
            remove
            {
                RemoveHandler(SelectedColorChangedEvent, value);
            }
        }

        #endregion //Events

        #region Properties

        #region SelectedColor

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorPicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));
        public Color? SelectedColor
        {
            get
            {
                return (Color?)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
                CurrentViewColor.SetColor(value);
            }
        }
        
        private static void OnSelectedColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker ColorPicker = o as ColorPicker;
            if (ColorPicker != null)
            {
                ColorPicker.OnSelectedColorChanged((Color?)e.OldValue, (Color?)e.NewValue);
            }
        }

        protected virtual void OnSelectedColorChanged(Color? oldValue, Color? newValue)
        {
            HexString = GetFormatedColorString(newValue);
            UpdateRGBValues(newValue);
            UpdateColorShadeSelectorPosition(newValue);

            var args = new RoutedPropertyChangedEventArgs<Color?>(oldValue, newValue);
            args.RoutedEvent = SelectedColorChangedEvent;
            RaiseEvent(args);
        }

        #endregion //SelectedColor

        #region RGB

        #region A

        public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)255, OnAChanged));
        public byte A
        {
            get => (byte)GetValue(AProperty);
            set => SetValue(AProperty, value);
        }

        private static void OnAChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker ColorPicker = o as ColorPicker;
            if (ColorPicker != null)
                ColorPicker.OnAChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnAChanged(byte oldValue, byte newValue)
        {
            if (!stopPropertyChanged)
                UpdateSelectedColor();
        }

        #endregion //A

        #region R

        public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)0, OnRChanged));
        public byte R
        {
            get => (byte)GetValue(RProperty);
            set => SetValue(RProperty, value);
        }

        private static void OnRChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker ColorPicker = o as ColorPicker;
            if (ColorPicker != null)
                ColorPicker.OnRChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnRChanged(byte oldValue, byte newValue)
        {
            if (!stopPropertyChanged)
                UpdateSelectedColor();
        }

        #endregion //R

        #region G

        public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof(G), typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)0, OnGChanged));
        public byte G
        {
            get => (byte)GetValue(GProperty);
            set => SetValue(GProperty, value);
        }

        private static void OnGChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker ColorPicker = o as ColorPicker;
            if (ColorPicker != null)
                ColorPicker.OnGChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnGChanged(byte oldValue, byte newValue)
        {
            if (!stopPropertyChanged)
                UpdateSelectedColor();
        }

        #endregion //G

        #region B

        public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(byte), typeof(ColorPicker), new UIPropertyMetadata((byte)0, OnBChanged));
        public byte B
        {
            get => (byte)GetValue(BProperty);
            set => SetValue(BProperty, value);
        }

        private static void OnBChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker ColorPicker = o as ColorPicker;
            if (ColorPicker != null)
                ColorPicker.OnBChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnBChanged(byte oldValue, byte newValue)
        {
            if (!stopPropertyChanged)
                UpdateSelectedColor();
        }

        #endregion //B

        #endregion //RGB

        #region HexString

        public static readonly DependencyProperty HexStringProperty = DependencyProperty.Register(nameof(HexString), typeof(string), typeof(ColorPicker), new UIPropertyMetadata("", OnHexStringChanged, OnCoerceHexString));
        public string HexString
        {
            get => (string)GetValue(HexStringProperty);
            set => SetValue(HexStringProperty, value);
        }

        private static void OnHexStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker ColorPicker = o as ColorPicker;
            if (ColorPicker != null)
                ColorPicker.OnHexStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnHexStringChanged(string oldValue, string newValue)
        {
            string currentColorString = GetFormatedColorString(SelectedColor);
            if (!currentColorString.Equals(newValue))
            {
                Color? col = null;
                if (!string.IsNullOrEmpty(newValue))
                {
                    col = (Color)ColorConverter.ConvertFromString(newValue);
                }
                UpdateSelectedColor(col);
            }

            SetHexTextBoxTextProperty(newValue);
        }

        private static object OnCoerceHexString(DependencyObject d, object basevalue)
        {
            var ColorPicker = (ColorPicker)d;
            if (ColorPicker == null)
                return basevalue;

            return ColorPicker.OnCoerceHexString(basevalue);
        }

        private object OnCoerceHexString(object newValue)
        {
            var value = newValue as string;
            string retValue = value;

            try
            {
                if (!string.IsNullOrEmpty(retValue))
                {
                    int outValue;
                    if (Int32.TryParse(retValue, System.Globalization.NumberStyles.HexNumber, null, out outValue))
                    {
                        retValue = "#" + retValue;
                    }
                    ColorConverter.ConvertFromString(retValue);
                }
            }
            catch
            {
                throw new InvalidOperationException("Color string is not correct");
            }

            return retValue;
        }

        #endregion //HexString

        #endregion //Properties

        #region HexTextBox

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (e.Key == Key.Enter && e.OriginalSource is TextBox)
            {
                TextBox textBox = (TextBox)e.OriginalSource;
                if (textBox == HexTextBox)
                    SetHexStringProperty(textBox.Text);
            }
        }

        void HexTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            SetHexStringProperty(textbox.Text);
        }

        #endregion

        #region Shading

        void ColorShadingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ColorShadingCanvas);
            UpdateColorShadeSelectorPositionAndCalculateColor(p, true);
            ColorShadingCanvas.CaptureMouse();
            e.Handled = true;
        }

        void ColorShadingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ColorShadingCanvas.ReleaseMouseCapture();
        }

        void ColorShadingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(ColorShadingCanvas);
                UpdateColorShadeSelectorPositionAndCalculateColor(p, true);
                Mouse.Synchronize();
            }
        }

        void ColorShadingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (currentColorShadePosition != null)
            {
                Point _newPoint = new Point
                {
                    X = ((Point)currentColorShadePosition).X * e.NewSize.Width,
                    Y = ((Point)currentColorShadePosition).Y * e.NewSize.Height
                };

                UpdateColorShadeSelectorPositionAndCalculateColor(_newPoint, false);
            }
        }

        private void UpdateColorShadeSelectorPositionAndCalculateColor(Point p, bool calculateColor)
        {
            if (p.Y < 0)
                p.Y = 0;

            if (p.X < 0)
                p.X = 0;

            if (p.X > ColorShadingCanvas.ActualWidth)
                p.X = ColorShadingCanvas.ActualWidth;

            if (p.Y > ColorShadingCanvas.ActualHeight)
                p.Y = ColorShadingCanvas.ActualHeight;

            colorShadeSelectorTransform.X = p.X - (ColorShadeSelector.Width / 2);
            colorShadeSelectorTransform.Y = p.Y - (ColorShadeSelector.Height / 2);

            p.X = p.X / ColorShadingCanvas.ActualWidth;
            p.Y = p.Y / ColorShadingCanvas.ActualHeight;

            currentColorShadePosition = p;

            if (calculateColor)
                CalculateColor(p);
        }

        private void UpdateColorShadeSelectorPosition(Color? color)
        {
            if ((color == null) || !color.HasValue)
                return;

            currentColorShadePosition = null;

            var hsv = ColorUtilities.RgbToHsv(color.Value.R, color.Value.G, color.Value.B);

            if (doUpdateSpectrumSliderValue)
            {
                currentSpectrumValue = hsv.H;
                CurrentViewColor.SetH(hsv.H);
                UpdateColorCircleSelectorPosition(hsv.H);
            }

            Point p = new Point(hsv.S, 1 - hsv.V);

            currentColorShadePosition = p;

            colorShadeSelectorTransform.X = (p.X * ColorShadingCanvas.Width) - (ColorShadeSelector.Width / 2);
            colorShadeSelectorTransform.Y = (p.Y * ColorShadingCanvas.Height) - (ColorShadeSelector.Height / 2);
        }


        #endregion

        #region Slider

        private enum ColorType
        {
            R, G, B, A
        }

        private Canvas draggingCanvas = null;

        private class SliderData
        {
            public ColorType Type;
            public Point CurrentColorPosition = new Point();
            public TranslateTransform SelectorTransform = new TranslateTransform();
            public Canvas Canvas;
            public Border SelectorBorder;
        }

        void SliderCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            var sliderData = (SliderData)canvas.Tag;
            Point p = e.GetPosition(canvas);
            UpdateColorSliderSelectorPositionAndCalculateColor(sliderData, p, true);
            canvas.CaptureMouse();
            draggingCanvas = canvas;
            e.Handled = true;
        }

        void SliderCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            var sliderData = (SliderData)canvas.Tag;
            canvas.ReleaseMouseCapture();
            draggingCanvas = null;
        }

        void SliderCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas != draggingCanvas) return;
            var sliderData = (SliderData)canvas.Tag;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(canvas);
                UpdateColorSliderSelectorPositionAndCalculateColor(sliderData, p, true);
                Mouse.Synchronize();
            }
        }

        void SliderCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var canvas = sender as Canvas;
            UpdateColorSliderSelectorPosition((SliderData)canvas.Tag);
        }

        #endregion

        #region Circle

        private void CircleCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            Point p = e.GetPosition(canvas);
            UpdateColorCircleSelectorPositionAndCalculateColor(canvas, p, true);
            canvas.CaptureMouse();
            e.Handled = true;
        }

        private void CircleCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = sender as Canvas;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(canvas);
                UpdateColorCircleSelectorPositionAndCalculateColor(canvas, p, true);
                Mouse.Synchronize();
            }
        }

        private void CircleCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            canvas.ReleaseMouseCapture();
        }

        private void CircleCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColorCircleSelectorPosition(currentSpectrumValue);
        }

        private void UpdateColorCircleSelectorPositionAndCalculateColor(Canvas canvas, Point p, bool calculateColor)
        {
            var r = canvas.ActualWidth / 2.0;
            var newx = p.X - r;
            var newy = p.Y - r;
            var rad = Math.Atan2(newx, newy);

            var hcw = ColorCircleSelector.ActualWidth / 2;
            var hch = ColorCircleSelector.ActualHeight / 2;
            newx = Math.Sin(rad) * (r - hcw) + r;
            newy = Math.Cos(rad) * (r - hch) + r;
            var hue = (rad * 180 / Math.PI) + 180 - 270;
            if (hue < 0) hue += 360;
            if (hue > 360) hue = 360;

            Canvas.SetLeft(ColorCircleSelector, newx - hcw);
            Canvas.SetTop(ColorCircleSelector, newy - hch);
            var color = SelectedColor;
            var hsv = ColorUtilities.RgbToHsv(color.Value.R, color.Value.G, color.Value.B);
            currentSpectrumValue = hue;
            CurrentViewColor.SetH(hue);
            SelectedColor = ColorUtilities.HsvToRgb(hue, hsv.S, hsv.V);
        }

        private void UpdateColorCircleSelectorPosition(double hue)
        {
            var r = CircleCanvas.ActualWidth / 2.0;
            var hcw = ColorCircleSelector.ActualWidth / 2;
            var hch = ColorCircleSelector.ActualHeight / 2;
            var realhue = hue;
            if (realhue > 270) hue -= 360;
            realhue += 270 - 180;
            var rad = realhue * Math.PI / 180;
            var newx = Math.Sin(rad) * (r - hcw) + r;
            var newy = Math.Cos(rad) * (r - hch) + r;
            Canvas.SetLeft(ColorCircleSelector, newx - hcw);
            Canvas.SetTop(ColorCircleSelector, newy - hch);
        }

        #endregion

        #region Methods

        private void UpdateSelectedColor()
        {
            SelectedColor = Color.FromArgb(A, R, G, B);
        }

        private void UpdateSelectedColor(Color? color)
        {
            SelectedColor = ((color != null) && color.HasValue)
                            ? (Color?)Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B)
                            : null;
        }

        private void UpdateRGBValues(Color? color)
        {
            if ((color == null) || !color.HasValue)
                return;

            stopPropertyChanged = true;

            A = color.Value.A;
            R = color.Value.R;
            G = color.Value.G;
            B = color.Value.B;

            UpdateColorSliderSelectorPosition((SliderData)RCanvas.Tag);
            UpdateColorSliderSelectorPosition((SliderData)GCanvas.Tag);
            UpdateColorSliderSelectorPosition((SliderData)BCanvas.Tag);
            //UpdateColorSliderSelectorPosition((SliderData)ACanvas.Tag);

            stopPropertyChanged = false;
        }

        private void UpdateColorSliderSelectorPositionAndCalculateColor(SliderData sliderData, Point p, bool calculateColor)
        {
            var canvas = sliderData.Canvas;
            var selectorTransform = sliderData.SelectorTransform;
            var maxWidth = canvas.ActualWidth - sliderData.SelectorBorder.Width;

            p.Y = 0;

            if (p.X < 0)
                p.X = 0;

            if (p.X > maxWidth)
                p.X = maxWidth;

            selectorTransform.X = p.X;
            selectorTransform.Y = 0;

            p.X = p.X / maxWidth;

            currentColorShadePosition = p;

            if (calculateColor)
                CalculateRGBAColor(sliderData.Type, p);
        }

        private void UpdateColorSliderSelectorPosition(SliderData sliderData)
        {
            var canvas = sliderData.Canvas;
            var selectorTransform = sliderData.SelectorTransform;
            var maxWidth = canvas.ActualWidth - sliderData.SelectorBorder.Width;
            byte color = 0;
            if (sliderData.Type == ColorType.R) color = R;
            else if (sliderData.Type == ColorType.G) color = G;
            else if (sliderData.Type == ColorType.B) color = B;
            else if (sliderData.Type == ColorType.A) color = A;

            selectorTransform.X = maxWidth * ((double)color / 255);
            selectorTransform.Y = 0;
        }
        private void CalculateColor(Point p)
        {
            HsvColor hsv = new HsvColor()
            {
                H = currentSpectrumValue,
                S = p.X,
                V = 1 - p.Y
            };
            var currentColor = ColorUtilities.HsvToRgb(hsv.H, hsv.S, hsv.V);
            currentColor.A = A;
            doUpdateSpectrumSliderValue = false;
            SelectedColor = currentColor;
            doUpdateSpectrumSliderValue = true;
            HexString = GetFormatedColorString(SelectedColor);
        }

        private void CalculateRGBAColor(ColorType colorType, Point p)
        {
            var currentColor = SelectedColor.Value;
            byte value = (byte)(p.X * 255);
            if (colorType == ColorType.R) currentColor.R = value;
            else if (colorType == ColorType.G) currentColor.G = value;
            else if (colorType == ColorType.B) currentColor.B = value;
            else if (colorType == ColorType.A) currentColor.A = value;
            SelectedColor = currentColor;
            HexString = GetFormatedColorString(SelectedColor);
        }

        private string GetFormatedColorString(Color? colorToFormat)
        {
            if ((colorToFormat == null) || !colorToFormat.HasValue)
                return string.Empty;
            return colorToFormat.ToString();
        }

        private void SetHexStringProperty(string newValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(newValue))
                {
                    if (int.TryParse(newValue, System.Globalization.NumberStyles.HexNumber, null, out int outValue))
                    {
                        newValue = "#" + newValue;
                    }
                    ColorConverter.ConvertFromString(newValue);
                }
                HexString = newValue;
            }
            catch
            {
                SetHexTextBoxTextProperty(HexString);
            }
        }

        private void SetHexTextBoxTextProperty(string newValue)
        {
            if (HexTextBox != null)
                HexTextBox.Text = newValue;
        }


        #endregion //Methods
    }

    public class ViewColor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Color currentColor;

        public void SetColor(Color? color)
        {
            currentColor = color ?? Colors.Black;
            var properties = new string[] {
                    nameof(RDarkColor),
                    nameof(RLightColor),
                    nameof(GDarkColor),
                    nameof(GLightColor),
                    nameof(BDarkColor),
                    nameof(BLightColor),
                    nameof(ADarkColor),
                    nameof(ALightColor),
                };
            foreach (var property in properties)
            {
                RaisePropertyChanged(property);
            }
        }

        public void SetH(double h)
        {
            currentH = h;
            RaisePropertyChanged(nameof(ShadingBaseColor));
        }

        public Color RDarkColor => Color.FromArgb(0xff, 0, currentColor.G, currentColor.B);
        public Color RLightColor => Color.FromArgb(0xff, 0xff, currentColor.G, currentColor.B);
        public Color GDarkColor => Color.FromArgb(0xff, currentColor.R, 0, currentColor.B);
        public Color GLightColor => Color.FromArgb(0xff, currentColor.R, 0xff, currentColor.B);
        public Color BDarkColor => Color.FromArgb(0xff, currentColor.R, currentColor.G, 0);
        public Color BLightColor => Color.FromArgb(0xff, currentColor.R, currentColor.G, 0xff);
        public Color ADarkColor => Color.FromArgb(0, currentColor.R, currentColor.G, currentColor.B);
        public Color ALightColor => Color.FromArgb(0xff, currentColor.R, currentColor.G, currentColor.B);
        private SolidColorBrush shadingBaseColorBlush = new SolidColorBrush();
        private double currentH = 0;
        public Brush ShadingBaseColor
        {
            get
            {
                shadingBaseColorBlush.Color = ColorUtilities.HsvToRgb(currentH, 1, 1);
                return shadingBaseColorBlush;
            }
        }
    }
}
