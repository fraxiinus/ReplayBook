using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using ModernWpf.Controls;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for StaticDataRetryDialog.xaml
    /// </summary>
    public partial class StaticDataRetryDialog : ContentDialog
    {

        /// <summary>
        /// Expects full patch string from replay
        /// </summary>
        public string PatchToDownload { get; set; }

        public string LanguageTried { get; set; }

        public string HttpErrorMessage { get; set; }

        private MainWindowViewModel ViewModel
        {
            get => (Window.GetWindow(this)?.DataContext is MainWindowViewModel viewModel)
                ? viewModel
                : throw new Exception("Invalid viewmodel");
        }

        public StaticDataRetryDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = ErrorMessage.Text
                .Replace("$0", PatchToDownload)
                .Replace("$1", LanguageTried);

            HttpErrorMessageBox.Text = HttpErrorMessage;
        }
    }
}
