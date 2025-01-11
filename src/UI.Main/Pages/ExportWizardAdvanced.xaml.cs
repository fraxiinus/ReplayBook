using Fraxiinus.ReplayBook.UI.Main.Extensions;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.Views;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;
using ModernWpf.Controls;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Fraxiinus.ReplayBook.UI.Main.Pages;
/// <summary>
/// Interaction logic for ExportWizardAdvanced.xaml
/// </summary>
public partial class ExportWizardAdvanced : ModernWpf.Controls.Page
{
    private ExportDataContext Context
    {
        get => (DataContext is ExportDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

    public ExportWizardAdvanced()
    {
        InitializeComponent();
    }

    private void ExportWizardAdvanced_Loaded(object sender, RoutedEventArgs e)
    {
        Context.WindowTitleText = TryFindResource("ErdTitleAdvanced") as string;

        // for some reason the first item in the attribute list box
        // gets selected on load. Unselect it 
        AttributeListBox.UnselectAll();
    }

    /// <summary>
    /// Call this function to update preview box contents
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void PreviewBox_Update(object sender, RoutedEventArgs e)
    {
        Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
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

    private async void PlayerCheckBox_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkbox) { return; }

        // get name of the player we just selected

        var playerPreview = (checkbox.DataContext as ExportPlayerSelectItem).PlayerPreview;
        var playerIdentifier = $"{playerPreview.PlayerName}{playerPreview.ChampionId}";

        // get the player object
        PlayerStats2 player = Context.Replay.Players
            .First(x => $"{x.GetPlayerNameOrID()}{x.Skin}".Equals(playerIdentifier, StringComparison.OrdinalIgnoreCase));

        // loop over all attributes and set preview value
        foreach (ExportAttributeSelectItem attribute in Context.Attributes)
        {
            attribute.Value = player.GetType().GetProperty(attribute.PropertyName).GetValue(player)?.ToString() ?? "N/A";
        }

        // regeneratee export preview
        Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
    }

    private async void AttributeCheckBox_Click(object sender, RoutedEventArgs e)
    {
        Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
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

    private async void SelectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (ExportPlayerSelectItem playerSelect in Context.Players)
        {
            playerSelect.Checked = true;
        }

        // update preview
        Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
    }

    private async void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (ExportPlayerSelectItem playerSelect in Context.Players)
        {
            playerSelect.Checked = false;
        }

        // update preview
        Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
    }

    private async void SelectAllAttributeMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (ExportAttributeSelectItem attributeSelect in Context.Attributes)
        {
            attributeSelect.Checked = true;
        }

        // update preview
        Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
    }

    private async void DeselectAllAttributeMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (ExportAttributeSelectItem attributeSelect in Context.Attributes)
        {
            attributeSelect.Checked = false;
        }

        // update preview
        Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // show or hide the preview text box based on window size
        if (ActualWidth > 950)
        {
            PreviewColumn.Width = new GridLength(1, GridUnitType.Star);
        }
        else if (PreviewColumn.Width != new GridLength(0))
        {
            PreviewColumn.Width = new GridLength(0);
        }
    }

    private async void PresetLoadButton_Click(object sender, RoutedEventArgs e)
    {
        // create an initial preset, it is probably empty
        ExportPreset preview = Context.CreatePreset();

        var dialog = new ExportPresetLoadDialog(Context.LastPreset)
        {
            DataContext = preview
        };

        ContentDialogResult result = ContentDialogResult.Secondary;
        try
        {
            // show export dialog
            result = await dialog.ShowAsync(ContentDialogPlacement.Popup);
        }
        catch (Exception ex)
        {
            Context.Log.Error(TryFindResource("ErdFailedToSave") as string);
            Context.Log.Error(ex.ToString());

            var errDialog = ContentDialogHelper.CreateContentDialog(
                title: TryFindResource("ErdPresetFailed") as string,
                description: ex.ToString(),
                primaryButtonText: TryFindResource("OKButtonText") as string);

            _ = await errDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        // Load button pressed
        if (result == ContentDialogResult.Primary)
        {
            // Load preset
            Context.LoadPreset(dialog.DataContext as ExportPreset);

            // update preview
            Context.ExportPreview = await ExportHelper.ConstructExportString(Context);
        }
    }

    private async void PresetSaveButton_Click(object sender, RoutedEventArgs e)
    {
        // create preset preview object
        ExportPreset preview = Context.CreatePreset();

        var dialog = new ExportPresetSaveDialog
        {
            DataContext = preview
        };

        // show preset dialog, if user wanted to save
        if (await dialog.ShowAsync(ContentDialogPlacement.Popup) == ContentDialogResult.Primary)
        {
            // attempt to save preset to file
            try
            {
                // file already exists
                if (ExportHelper.PresetNameExists(preview.PresetName))
                {
                    // create dialog to inform user of overwrite
                    var errDialog = ContentDialogHelper.CreateContentDialog(
                        title: TryFindResource("ErdFailedToSave") as string,
                        description: TryFindResource("ErdPresetExists") as string,
                        primaryButtonText: TryFindResource("ErdPresetOverwrite") as string,
                        secondaryButtonText: TryFindResource("CancelButtonText") as string);

                    // if user wants to overwrite
                    if (await errDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true) == ContentDialogResult.Primary)
                    {
                        ExportHelper.SavePresetToFile(preview);
                    }
                }
                else // save to file
                {
                    ExportHelper.SavePresetToFile(preview);

                    // create dialog to inform save succeeded
                    var resultDialog = ContentDialogHelper.CreateContentDialog(
                        title: TryFindResource("ErdPresetSavedTitle") as string,
                        description: preview.PresetName,
                        primaryButtonText: TryFindResource("CloseText") as string);

                    // show confirmation
                    _ = await resultDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                Context.Log.Error(TryFindResource("ErdFailedToSave") as string);
                Context.Log.Error(ex.ToString());

                var errDialog = ContentDialogHelper.CreateContentDialog(
                    title: TryFindResource("ErdFailedToSave") as string,
                    description: ex.ToString(),
                    primaryButtonText: TryFindResource("OKButtonText") as string);

                _ = await errDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            }
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        bool exit = false;
        try
        {
            // Save the preset as the last loaded
            Context.LastPreset = Context.PresetName;

            exit = await ExportHelper.ExportToFile(Context, Window.GetWindow(this));
        }
        catch (Exception ex)
        {
            Context.Log.Error(TryFindResource("ErdFailedToSave") as string);
            Context.Log.Error(ex.ToString());

            var dialog = ContentDialogHelper.CreateContentDialog(
                title: TryFindResource("ErdFailedToSave") as string,
                description: ex.ToString(),
                primaryButtonText: TryFindResource("OKButtonText") as string);

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        if (exit)
        {
            // close window
            Window.GetWindow(this).Close();
        }
    }
}
