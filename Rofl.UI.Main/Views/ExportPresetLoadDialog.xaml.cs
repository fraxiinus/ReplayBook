using ModernWpf.Controls;
using Rofl.UI.Main.Utilities;
using System.Windows.Controls;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExportPresetLoadDialog.xaml
    /// </summary>
    public partial class ExportPresetLoadDialog : ContentDialog
    {
        public ExportPresetLoadDialog()
        {
            InitializeComponent();

            PresetNamesBox.ItemsSource = ExportHelper.FindAllPresets();
        }

        private void PresetNamesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string presetName = PresetNamesBox.SelectedItem as string;

            DataContext = ExportHelper.LoadPreset(presetName);
        }
    }
}
