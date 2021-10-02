using ModernWpf.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
