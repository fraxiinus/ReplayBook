namespace Fraxiinus.ReplayBook.Configuration.Models;

using System;
using System.ComponentModel;

/// <summary>
/// Model used to present replay categories
/// </summary>
public class CategoryItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly string _id;
    private string _name = "Default Category";
    private string _description = "";
    private string _searchTerm = "";

    public CategoryItem(string name, string description, string searchTerm)
    {
        _id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
        SearchTerm = searchTerm;
    }

    public string Id => _id;

    /// <summary>
    /// Displayed as the title
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

    /// <summary>
    /// Displayed as the tooltip
    /// </summary>
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(Description)));
        }
    }

    /// <summary>
    /// The search term to use when showing the category
    /// </summary>
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            _searchTerm = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(SearchTerm)));
        }
    }
}
