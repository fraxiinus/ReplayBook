using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class ExportSelectItemModel : INotifyPropertyChanged
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
