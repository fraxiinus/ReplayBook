using System.ComponentModel;

namespace Rofl.UI.Main.Models
{
    public class ExportSelectItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;

        public string Name
        {
            get => _name;

            set
            {
                _name = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private string _internalString;
        public string InternalString
        {
            get => _internalString;

            set
            {
                _internalString = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

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

        
    }
}
