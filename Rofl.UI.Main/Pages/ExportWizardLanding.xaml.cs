using ModernWpf.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System;
using System.Windows;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for ExportWizardLanding.xaml
    /// </summary>
    public partial class ExportWizardLanding : Page
    {
        public ExportWizardLanding()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            _ = context.ContentFrame.Navigate(typeof(ExportWizardPlayers));
        }

        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            Window window = Window.GetWindow(this);
            window.Width = 800;
            window.MinWidth = 850;

            context.HideHeader = true;

            _ = context.ContentFrame.Navigate(typeof(ExportWizardAdvanced));
        }

        private void EverythingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            // select everything
            foreach (ExportPlayerSelectItem playerSelect in context.Players)
            {
                playerSelect.Checked = true;
            }

            foreach (ExportAttributeSelectItem attributeSelect in context.Attributes)
            {
                attributeSelect.Checked = true;
            }

            context.IncludeMatchDuration = true;
            context.IncludeMatchID = true;
            context.IncludePatchVersion = true;

            ExportHelper.ExportToFile(context, Window.GetWindow(this));
        }

        private async void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            // create an initial preset, it is probably empty
            ExportPreset preview = context.CreatePreset("preview");

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

            // Export button pressed
            if (result == ContentDialogResult.Primary)
            {
                // Load preset
                context.LoadPreset(dialog.DataContext as ExportPreset);

                // try to export data to file, close the wizard if its done
                if (ExportHelper.ExportToFile(context, Window.GetWindow(this)))
                {
                    Window.GetWindow(this).Close();
                }
            }
        }
    }
}
