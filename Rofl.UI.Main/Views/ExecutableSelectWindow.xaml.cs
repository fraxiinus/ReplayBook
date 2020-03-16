using Rofl.Executables.Models;
using System.Windows;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExecutableSelectWindow.xaml
    /// </summary>
    public partial class ExecutableSelectWindow : Window
    {
        public ExecutableSelectWindow()
        {
            InitializeComponent();
        }

        public LeagueExecutable Selection { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.ActualWidth;
            this.MinHeight = this.ActualHeight;
            this.MaxHeight = this.ActualHeight;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            Selection = selectedExecutable;
            this.DialogResult = true;
            return;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            return;
        }
    }
}
