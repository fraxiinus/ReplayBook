using ModernWpf.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System;
using System.Windows;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for ExportWizardFinish.xaml
    /// </summary>
    public partial class ExportWizardFinish : Page
    {
        public ExportWizardFinish()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Call this function to update preview box contents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewBox_Update(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            context.ExportPreview = ExportHelper.ConstructExportString(context);
        }

        private async void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            // create preset preview object
            ExportPreset preview = context.CreatePreset();

            ExportPresetSaveDialog dialog = new ExportPresetSaveDialog
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
                        ContentDialog errDialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: true);
                        errDialog.DefaultButton = ContentDialogButton.Primary;
                        errDialog.SecondaryButtonText = TryFindResource("CancelButtonText") as string;
                        errDialog.PrimaryButtonText = TryFindResource("ErdPresetOverwrite") as string;
                        errDialog.Title = TryFindResource("ErdFailedToSave") as string;
                        errDialog.SetLabelText(TryFindResource("ErdPresetExists") as string);
                        errDialog.GetContentDialogLabel().TextWrapping = TextWrapping.Wrap;
                        errDialog.GetContentDialogLabel().Width = 300;

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
                        ContentDialog errDialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
                        errDialog.DefaultButton = ContentDialogButton.Primary;
                        errDialog.PrimaryButtonText = TryFindResource("CloseText") as string;
                        errDialog.Title = TryFindResource("ErdPresetSavedTitle") as string;
                        errDialog.SetLabelText(preview.PresetName);
                        errDialog.GetContentDialogLabel().TextWrapping = TextWrapping.Wrap;
                        errDialog.GetContentDialogLabel().Width = 300;
                        
                        // show confirmation
                        _ = await errDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                    }
                }
                catch (Exception ex)
                {
                    context.Log.Error(TryFindResource("ErdFailedToSave") as string);
                    context.Log.Error(ex.ToString());

                    ContentDialog errDialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
                    errDialog.DefaultButton = ContentDialogButton.Primary;
                    errDialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
                    errDialog.Title = TryFindResource("ErdFailedToSave") as string;
                    errDialog.SetLabelText(ex.ToString());
                    errDialog.GetContentDialogLabel().TextWrapping = TextWrapping.Wrap;
                    errDialog.GetContentDialogLabel().Width = 300;

                    _ = await errDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            context.ContentFrame.GoBack();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            bool exit = false;
            try
            {
                exit = ExportHelper.ExportToFile(context, Window.GetWindow(this));
            }
            catch (Exception ex)
            {
                context.Log.Error(TryFindResource("ErdFailedToSave") as string);
                context.Log.Error(ex.ToString());

                ContentDialog dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
                dialog.Title = TryFindResource("ErdFailedToSave") as string;
                dialog.SetLabelText(ex.ToString());
                dialog.GetContentDialogLabel().TextWrapping = TextWrapping.Wrap;
                dialog.GetContentDialogLabel().Width = 300;

                _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            }

            if (exit)
            {
                // close window
                Window.GetWindow(this).Close();
            }
        }
    }
}
