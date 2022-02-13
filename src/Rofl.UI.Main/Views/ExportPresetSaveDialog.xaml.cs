using ModernWpf.Controls;
using Rofl.UI.Main.Utilities;
using System;
using System.Windows;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExportPresetSaveDialog.xaml
    /// </summary>
    public partial class ExportPresetSaveDialog : ContentDialog
    {
        public ExportPresetSaveDialog()
        {
            InitializeComponent();
        }

        private void PresetNameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(PresetNameBox.Text)) { return; }

            try
            {
                if (ExportHelper.PresetNameExists(PresetNameBox.Text))
                {
                    ErrorTextBlock.Visibility = Visibility.Visible;
                    ErrorTextBlock.Text = TryFindResource("ErdPresetExistsNotify") as string;
                }
                else
                {
                    ErrorTextBlock.Visibility = Visibility.Collapsed;
                }

                // button might have been disabled by exception, always enable
                IsPrimaryButtonEnabled = true;
            }
            catch (ArgumentException)
            {
                // input name contains invalid characters, show error and disable primary button
                ErrorTextBlock.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = TryFindResource("ErdPresetInvalidNotify") as string;
                IsPrimaryButtonEnabled = false;
            }
        }
    }
}