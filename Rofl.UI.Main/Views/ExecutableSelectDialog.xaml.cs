using ModernWpf.Controls;
using Rofl.Executables.Models;
using System.Windows.Input;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExecutableSelectDialog.xaml
    /// </summary>
    public partial class ExecutableSelectDialog : ContentDialog
    {
        public LeagueExecutable Selection { get; private set; }

        public ExecutableSelectDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(object sender, ContentDialogButtonClickEventArgs args)
        {
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            Selection = selectedExecutable;

            Hide();
        }

        private void ContentDialog_CloseButtonClick(object sender, ContentDialogButtonClickEventArgs args)
        {
            Selection = null;
            Hide();
        }

        private void ExecutablesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContentDialog_PrimaryButtonClick(null, null);
        }
    }
}
