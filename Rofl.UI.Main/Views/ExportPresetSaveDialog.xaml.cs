using ModernWpf.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using System.Windows.Media;
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
            // if preset already exists...
            ExistsTextBlock.Visibility = ExportHelper.PresetNameExists(PresetNameBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}