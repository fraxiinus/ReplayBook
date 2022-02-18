using Microsoft.Extensions.Configuration;
using System.ComponentModel;

namespace Fraxiinus.ReplayBook.Configuration.Models
{
    public class PlayerMarkerConfiguration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // this will never be in a situation where it is null
        // be quiet
        private string _name = default!;
        [ConfigurationKeyName("name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private string _color = default!;
        [ConfigurationKeyName("color")]
        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Color)));
            }
        }

        private string _note = default!;
        [ConfigurationKeyName("note")]
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Note)));
            }
        }
    }
}
