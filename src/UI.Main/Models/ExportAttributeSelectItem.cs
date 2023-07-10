using System.ComponentModel;

namespace Fraxiinus.ReplayBook.UI.Main.Models
{
    public class ExportAttributeSelectItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        /// <summary>
        /// Display Name
        /// </summary>
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

        private string _propertyName;
        /// <summary>
        /// Internal Object Name
        /// </summary>
        public string PropertyName
        {
            get => _propertyName;
            set
            {
                _propertyName = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(PropertyName)));
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

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }
}
