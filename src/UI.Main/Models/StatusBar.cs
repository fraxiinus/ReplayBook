using Fraxiinus.ReplayBook.Files.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Fraxiinus.ReplayBook.UI.Main.Models
{
    public class StatusBar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StatusBar()
        {
            _statusMessage = string.Empty;
            _visible = false;
        }

        public IEnumerable<ReplayErrorInfo> Errors { get; set; }

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

        private bool _showDismissButton;
        public bool ShowDismissButton
        {
            get => _showDismissButton;
            set
            {
                _showDismissButton = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ShowDismissButton)));
            }
        }
    }
}
