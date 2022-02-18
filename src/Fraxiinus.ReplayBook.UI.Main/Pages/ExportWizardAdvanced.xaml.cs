using ModernWpf.Controls;
using Fraxiinus.ReplayBook.Reader.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.Views;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Fraxiinus.ReplayBook.UI.Main.Pages
{
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
        private void PreviewBox_Update(object sender, RoutedEventArgs e)
        {
            Context.ExportPreview = ExportHelper.ConstructExportString(Context);
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

        private void PlayerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkbox) { return; }

            // get name of the player we just selected
            string playerName = (((SimpleStackPanel)checkbox.Content).Children[1] as TextBlock).Text;

            // get the player object
            Player player = Context.Replay.Players.First(x => x.NAME.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            // loop over all attributes and set preview value
            foreach (ExportAttributeSelectItem attribute in Context.Attributes)
            {
                attribute.Value = player.GetType().GetProperty(attribute.Name).GetValue(player)?.ToString() ?? "N/A";
            }

            // regeneratee export preview
            Context.ExportPreview = ExportHelper.ConstructExportString(Context);
        }

        private void AttributeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            Context.ExportPreview = ExportHelper.ConstructExportString(Context);
        }

        private void AttributeFilter(object sender, FilterEventArgs e)
        {
            string filterText = Context.AttributeFilterText;

            if (e.Item is not ExportAttributeSelectItem src)
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
            foreach (ExportPlayerSelectItem playerSelect in Context.Players)
            {
                playerSelect.Checked = true;
            }

            // update preview
            Context.ExportPreview = ExportHelper.ConstructExportString(Context);
        }

        private void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ExportPlayerSelectItem playerSelect in Context.Players)
            {
                playerSelect.Checked = false;
            }

            // update preview
            Context.ExportPreview = ExportHelper.ConstructExportString(Context);
        }

        private void SelectAllAttributeMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ExportAttributeSelectItem attributeSelect in Context.Attributes)
            {
                attributeSelect.Checked = true;
            }

            // update preview
            Context.ExportPreview = ExportHelper.ConstructExportString(Context);
        }

        private void DeselectAllAttributeMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ExportAttributeSelectItem attributeSelect in Context.Attributes)
            {
                attributeSelect.Checked = false;
            }

            // update preview
            Context.ExportPreview = ExportHelper.ConstructExportString(Context);
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
                Context.LoadPreset(dialog.DataContext as ExportPreset);

                // update preview
                Context.ExportPreview = ExportHelper.ConstructExportString(Context);
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
                    Context.Log.Error(TryFindResource("ErdFailedToSave") as string);
                    Context.Log.Error(ex.ToString());

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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool exit = false;
            try
            {
                // Save the preset as the last loaded
                Context.LastPreset = Context.PresetName;

                exit = ExportHelper.ExportToFile(Context, Window.GetWindow(this));
            }
            catch (Exception ex)
            {
                Context.Log.Error(TryFindResource("ErdFailedToSave") as string);
                Context.Log.Error(ex.ToString());

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
