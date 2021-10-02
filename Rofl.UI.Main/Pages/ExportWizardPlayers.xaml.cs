using ModernWpf.Controls;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
