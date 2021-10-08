using ModernWpf.Controls;
using Rofl.UI.Main.Models;
using System.Windows;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for ExportWizardPlayers.xaml
    /// </summary>
    public partial class ExportWizardPlayers : Page
    {
        public ExportWizardPlayers()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            // check if they checked any players

            _ = context.ContentFrame.Navigate(typeof(ExportWizardAttributes));
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
    }
}
