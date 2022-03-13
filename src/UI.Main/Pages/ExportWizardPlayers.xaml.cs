using ModernWpf.Controls;
using Fraxiinus.ReplayBook.UI.Main.Models;
using System;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for ExportWizardPlayers.xaml
    /// </summary>
    public partial class ExportWizardPlayers : Page
    {
        private ExportDataContext Context
        {
            get => (DataContext is ExportDataContext context)
                ? context
                : throw new Exception("Invalid data context");
        }

        public ExportWizardPlayers()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            _ = Context.ContentFrame.Navigate(typeof(ExportWizardAttributes));
        }

        private void SelectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ExportPlayerSelectItem playerSelect in Context.Players)
            {
                playerSelect.Checked = true;
            }
        }

        private void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ExportPlayerSelectItem playerSelect in Context.Players)
            {
                playerSelect.Checked = false;
            }
        }
    }
}
