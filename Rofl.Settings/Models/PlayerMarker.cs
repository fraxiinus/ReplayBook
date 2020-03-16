using Newtonsoft.Json;
using System.ComponentModel;

namespace Rofl.Settings.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PlayerMarker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        [JsonProperty("name")]
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

        private string _color;
        [JsonProperty("color")]
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

    }
}
