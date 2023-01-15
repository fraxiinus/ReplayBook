namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

/// <summary>
/// Interaction logic for ExportWizardAttributes.xaml
/// </summary>
public partial class ExportWizardAttributes : ModernWpf.Controls.Page
{
    private ExportDataContext Context
    {
        get => (DataContext is ExportDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

    public ExportWizardAttributes()
    {
        InitializeComponent();
    }

    private void ExportWizardAttributes_Loaded(object sender, RoutedEventArgs e)
    {
        // get first checked player
        string playerName = Context.Players.FirstOrDefault(x => x.Checked)?.PlayerPreview.PlayerName;
        PlayerStats player = Context.Replay.Players
            .FirstOrDefault(x => x.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));

        if (player != null)
        {
            // loop over all attributes and set preview value
            foreach (ExportAttributeSelectItem attribute in Context.Attributes)
            {
                attribute.Value = player.GetType().GetProperty(attribute.PropertyName).GetValue(player)?.ToString() ?? "N/A";
            }
        }

        // for some reason the first item in the attribute list box
        // gets selected on load. Unselect it 
        AttributeListBox.UnselectAll();
    }

    private void AttributeFilterBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textbox) { return; }

        if (string.IsNullOrEmpty(textbox.Text))
        {
            Context.AttributesView.Filter -= new FilterEventHandler(AttributeFilter);
            return;
        }

        Context.AttributesView.Filter -= new FilterEventHandler(AttributeFilter);
        Context.AttributesView.Filter += new FilterEventHandler(AttributeFilter);

        Context.AttributesView.View.Refresh();
    }

    private void AttributeFilter(object sender, FilterEventArgs e)
    {
        string filterText = Context.AttributeFilterText;

        if (e.Item is not ExportAttributeSelectItem src)
        {
            e.Accepted = false;
        }
        else if (src != null && !src.Name.ToUpper(CultureInfo.InvariantCulture).Contains(filterText.ToUpper(CultureInfo.InvariantCulture)))// here is FirstName a Property in my YourCollectionItem
        {
            e.Accepted = false;
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        Context.ContentFrame.GoBack();
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        _ = Context.ContentFrame.Navigate(typeof(ExportWizardFinish));
    }

    private void SelectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (ExportAttributeSelectItem attributeSelect in Context.Attributes)
        {
            attributeSelect.Checked = true;
        }
    }

    private void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (ExportAttributeSelectItem attributeSelect in Context.Attributes)
        {
            attributeSelect.Checked = false;
        }
    }
}
