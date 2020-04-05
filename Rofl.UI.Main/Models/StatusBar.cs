using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Rofl.UI.Main.Models
{
    public class StatusBar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StatusBar()
        {
            _statusMessage = string.Empty;
            _color = Brushes.Red;
            _visible = false;
        }

        private string _statusMessage;
        public string StatusMessage 
        {
            get => _statusMessage;
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
            get => _color;
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
            get => _visible;
            set
            {
                _visible = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Visible)));
            }
        }

        private bool _showProgressBar;
        public bool ShowProgressBar
        {
            get => _showProgressBar;
            set
            {
                _showProgressBar = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ShowProgressBar)));
            }
        }
    }
}
