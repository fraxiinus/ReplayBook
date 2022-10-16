using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Extensions;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupDownload.xaml
    /// </summary>
    public partial class WelcomeSetupDownload : ModernWpf.Controls.Page, IWelcomePage
    {

        public WelcomeSetupDownload()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not WelcomeSetupDataContext context) { return; }
            if (Application.Current.MainWindow is not MainWindow mainWindow) { return; }
            if (mainWindow.DataContext is not MainWindowViewModel mainViewModel) { return; }

            // disable nav buttons
            context.DisableNextButton = true;
            context.DisableBackButton = true;
            context.DisableSkipButton = true;

            // get patches
            await mainViewModel.StaticDataManager.RefreshPatches();
            var latestPatch = mainViewModel.StaticDataManager.Context.KnownPatchNumbers[1];

            await StaticDataDownloadDialog.StartDownloadDialog(latestPatch);

            // enable nav buttons, only allow them to proceed
            context.DisableNextButton = false;
        }

        public string GetTitle()
        {
            return (string)TryFindResource("WswDownloadFrameTitle");
        }

        public Type GetNextPage()
        {
            return typeof(WelcomeSetupFinish);
        }

        public Type GetPreviousPage()
        {
            return typeof(WelcomeSetupReplays);
        }
    }
}
