namespace Fraxiinus.ReplayBook.Files.Models.Search;

using System;
using System.ComponentModel;

public enum SortMethod { NameAsc, NameDesc, DateAsc, DateDesc, SizeAsc, SizeDesc }

public class SearchParameters : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _queryString;
    public string QueryString
    {
        get { return _queryString; }
        set
        {
            _queryString = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(QueryString)));
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
