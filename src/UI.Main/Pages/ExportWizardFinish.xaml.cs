namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using ModernWpf.Controls;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.Views;
using System;
using System.Windows;

/// <summary>
/// Interaction logic for ExportWizardFinish.xaml
/// </summary>
public partial class ExportWizardFinish : Page
{
    private ExportDataContext Context
    {
        get => (DataContext is ExportDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

    public ExportWizardFinish()
    {
        InitializeComponent();
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

    private async void PresetButton_Click(object sender, RoutedEventArgs e)
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

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        Context.ContentFrame.GoBack();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        bool exit = false;
        try
        {
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
