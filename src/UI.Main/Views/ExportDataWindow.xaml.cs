namespace Fraxiinus.ReplayBook.UI.Main.Views;

using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Text.Json.Serialization;

/// <summary>
/// Interaction logic for ExportDataWindow.xaml
/// </summary>
public partial class ExportDataWindow : Window
{
    private ExportDataContext Context
    {
        get => (DataContext is ExportDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

    public ExportDataWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // load preview players into list
        Context.Players = Context.ReplayPreview.BluePreviewPlayers
            .Union(Context.ReplayPreview.RedPreviewPlayers)
            .Select(x => new ExportPlayerSelectItem
            {
                Checked = false,
                PlayerPreview = x
            })
            .ToList();

        _ = Context.ContentFrame.Navigate(typeof(Pages.ExportWizardLanding));

        LoadAttributes();
    }

    private void LoadAttributes()
    {
        // do not reload attributes list if it has been loaded before (when navigating back to this page for example)
        if (Context.Attributes is null)
        {
            Context.Attributes = typeof(PlayerStats)
                .GetProperties()
                .Where(x => !x.Name.Equals("UniqueID", StringComparison.OrdinalIgnoreCase)) // ignore internal properties
                .OrderBy(x => x.Name)
                .Select(x => new ExportAttributeSelectItem
                {
                    Checked = false,
                    Name = x.GetCustomAttribute<JsonPropertyNameAttribute>().Name,
                    PropertyName = x.Name,
                    Value = "N/A"
                })
                .ToList();
        }
    }
}