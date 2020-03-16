using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Rofl.UI.Main.Models
{
    public class StatusBarModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StatusBarModel()
        {
            _statusMessage = String.Empty;
            _color = Brushes.Red;
            _visible = false;
        }

        private string _statusMessage;
        public string StatusMessage 
        {
            get { return _statusMessage; }
            set 
            { 
                _statusMessage = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }

        private Brush _color;
        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Color)));
            }
        }

        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Visible)));
            }
        }
    }
}
