using ModernWpf.Controls;
using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for ExportWizardAdvanced.xaml
    /// </summary>
    public partial class ExportWizardAdvanced : ModernWpf.Controls.Page
    {
        public ExportWizardAdvanced()
        {
            InitializeComponent();
        }

        private void ExportWizardAdvanced_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            context.WindowTitleText = "Export Data Advanced Mode";

            // for some reason the first item in the attribute list box
            // gets selected on load. Unselect it 
            AttributeListBox.UnselectAll();
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

        private void AttributeFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }
            if (!(sender is TextBox textbox)) { return; }

            if (string.IsNullOrEmpty(textbox.Text))
            {
                context.AttributesView.Filter -= new FilterEventHandler(AttributeFilter);
                return;
            }

            context.AttributesView.Filter -= new FilterEventHandler(AttributeFilter);
            context.AttributesView.Filter += new FilterEventHandler(AttributeFilter);

            context.AttributesView.View.Refresh();
        }

        private void PlayerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }
            if (!(sender is CheckBox checkbox)) { return; }

            // get name of the player we just selected
            string playerName = (((SimpleStackPanel)checkbox.Content).Children[1] as TextBlock).Text;

            // get the player object
            Player player = context.Replay.Players.First(x => x.NAME.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            // loop over all attributes and set preview value
            foreach (ExportAttributeSelectItem attribute in context.Attributes)
            {
                attribute.Value = player.GetType().GetProperty(attribute.Name).GetValue(player)?.ToString() ?? "N/A";
            }

            // regeneratee export preview
            context.ExportPreview = ExportHelper.ConstructExportString(context);
        }

        private void AttributeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            context.ExportPreview = ExportHelper.ConstructExportString(context);
        }

        private void AttributeFilter(object sender, FilterEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            string filterText = context.AttributeFilterText;

            if (!(e.Item is ExportAttributeSelectItem src))
            {
                e.Accepted = false;
            }
            else if (src != null && !src.Name.Contains(filterText.ToUpper(CultureInfo.InvariantCulture)))// here is FirstName a Property in my YourCollectionItem
            {
                e.Accepted = false;
            }
        }

        private void SelectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            foreach (ExportPlayerSelectItem playerSelect in context.Players)
            {
                playerSelect.Checked = true;
            }
        }

        private void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            foreach (ExportPlayerSelectItem playerSelect in context.Players)
            {
                playerSelect.Checked = false;
            }
        }

        private void SelectAllAttributeMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            foreach (ExportAttributeSelectItem attributeSelect in context.Attributes)
            {
                attributeSelect.Checked = true;
            }
        }

        private void DeselectAllAttributeMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            foreach (ExportAttributeSelectItem attributeSelect in context.Attributes)
            {
                attributeSelect.Checked = false;
            }
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
            if (!(DataContext is ExportDataContext context)) { return; }

            // create an initial preset, it is probably empty
            ExportPreset preview = context.CreatePreset();

            ExportPresetLoadDialog dialog = new ExportPresetLoadDialog
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
                context.Log.Error(TryFindResource("ErdFailedToSave") as string);
                context.Log.Error(ex.ToString());

                ContentDialog errDialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
                errDialog.DefaultButton = ContentDialogButton.Primary;
                errDialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
                errDialog.Title = TryFindResource("ErdPresetFailed") as string;
                errDialog.SetLabelText(ex.ToString());
                errDialog.GetContentDialogLabel().TextWrapping = TextWrapping.Wrap;
                errDialog.GetContentDialogLabel().Width = 300;

                _ = await errDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            }

            // Load button pressed
            if (result == ContentDialogResult.Primary)
            {
                // Load preset
                context.LoadPreset(dialog.DataContext as ExportPreset);
            }
        }

        private async void PresetSaveButton_Click(object sender, RoutedEventArgs e)
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
    }
}
