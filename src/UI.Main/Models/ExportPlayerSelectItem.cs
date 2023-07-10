using System.ComponentModel;
using Fraxiinus.ReplayBook.UI.Main.Models.View;

namespace Fraxiinus.ReplayBook.UI.Main.Models
{
    public class ExportPlayerSelectItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _checked;
        public bool Checked
        {
            get => _checked;

            set
            {
                _checked = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Checked)));
            }
        }

        private PlayerPreview _playerPreview;
        public PlayerPreview PlayerPreview
        {
            get => _playerPreview;
            set
            {
                _playerPreview = value;

                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ReplayPreview)));
            }
        }
    }
}
