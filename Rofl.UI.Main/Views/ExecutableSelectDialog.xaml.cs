using ModernWpf.Controls;
using Rofl.Executables.Models;
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
using System.Windows.Shapes;

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

            this.Hide();
        }

        private void ContentDialog_CloseButtonClick(object sender, ContentDialogButtonClickEventArgs args)
        {
            Selection = null;
            this.Hide();
        }

        private void ExecutablesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContentDialog_PrimaryButtonClick(null, null);
        }
    }
}
