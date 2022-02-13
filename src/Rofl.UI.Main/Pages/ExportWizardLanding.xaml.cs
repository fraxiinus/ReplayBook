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
        private ExportDataContext Context
        {
            get => (DataContext is ExportDataContext context)
                ? context
                : throw new Exception("Invalid data context");
        }

        public ExportWizardLanding()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _ = Context.ContentFrame.Navigate(typeof(ExportWizardPlayers));
        }

        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Width = 1150;
            window.Height = 670;
            window.MinWidth = 850;

            Context.HideHeader = true;

            _ = Context.ContentFrame.Navigate(typeof(ExportWizardAdvanced));
        }

        private void EverythingButton_Click(object sender, RoutedEventArgs e)
        {
            // select everything
            foreach (ExportPlayerSelectItem playerSelect in Context.Players)
            {
                playerSelect.Checked = true;
            }

            foreach (ExportAttributeSelectItem attributeSelect in Context.Attributes)
            {
                attributeSelect.Checked = true;
            }

            Context.IncludeMatchDuration = true;
            Context.IncludeMatchID = true;
            Context.IncludePatchVersion = true;

            _ = ExportHelper.ExportToFile(Context, Window.GetWindow(this));
        }

        private async void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            // create an initial preset, it is probably empty
            ExportPreset preview = Context.CreatePreset();

            // pass in last preset, to auto-load
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

            // Export button pressed
            if (result == ContentDialogResult.Primary)
            {
                // Load preset
                Context.LoadPreset(dialog.DataContext as ExportPreset);

                // Save the preset that is currently loaded as the last preset
                Context.LastPreset = Context.PresetName;

                // try to export data to file, close the wizard if its done
                if (ExportHelper.ExportToFile(Context, Window.GetWindow(this)))
                {
                    Window.GetWindow(this).Close();
                }
            }
        }
    }
}
