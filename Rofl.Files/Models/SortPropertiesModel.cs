using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.Files.Models
{
    public enum SortMethod { NameAsc, NameDesc, DateAsc, DateDesc, SizeAsc, SizeDesc }

    public class SortPropertiesModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                _searchTerm = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(SearchTerm)));
            }
        }

        private SortMethod _sortMethod;
        public SortMethod SortMethod
        {
            get { return _sortMethod; }
            set
            {
                _sortMethod = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(SortMethod)));
            }
        }

    }
}
