using Fraxiinus.ReplayBook.StaticData.Extensions;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
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
        private CancellationTokenSource cancellationToken;

        public WelcomeSetupDownload()
        {
            InitializeComponent();
            //NextButton.IsEnabled = false;
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

            // Make progress elements visible
            DownloadProgressGrid.Visibility = Visibility.Visible;
            DownloadProgressBar.Value = 0;
            DownloadProgressBar.Minimum = 0;
            DownloadProgressBar.Maximum = 3;
            DownloadProgressBar.IsIndeterminate = true;
            DownloadProgressText.Text = TryFindResource("Welcome__StaticData__DownloadProgress__StartLabel") as string;

            // create a new token, used by the cancel button
            cancellationToken = new CancellationTokenSource();

            string latest = null;
            try
            {
                // start download process
                // 1. get patches
                await mainViewModel.StaticDataManager.RefreshPatches(cancellationToken.Token);

                DownloadProgressBar.IsIndeterminate = false;
                DownloadProgressText.Text = TryFindResource("Welcome__StaticData__DownloadProgress__PropertiesLabel") as string;
                // 2. download all the properties for the current language
                latest = mainViewModel.StaticDataManager.Context.KnownPatchNumbers[0];
                await mainViewModel.StaticDataManager.DownloadProperties(latest,
                        StaticDataType.Rune | StaticDataType.Item | StaticDataType.Champion,
                        LanguageHelper.CurrentLanguage.GetRiotRegionCode(),
                        cancellationToken.Token);

                DownloadProgressBar.Value++;
                DownloadProgressText.Text = TryFindResource("Welcome__StaticData__DownloadProgress__ImagesLabel") as string;
                // 3. download all the images for items and champions
                await mainViewModel.StaticDataManager.DownloadImages(latest,
                    StaticDataType.Item | StaticDataType.Champion,
                    cancellationToken.Token);

                DownloadProgressBar.Value++;
                DownloadProgressText.Text = TryFindResource("Welcome__StaticData__DownloadProgress__RuneLabel") as string;
                // 4. download all the images for runes
                await mainViewModel.StaticDataManager.DownloadRuneImages(latest,
                    LanguageHelper.CurrentLanguage.GetRiotRegionCode(),
                    cancellationToken.Token);

                DownloadProgressBar.Value++;
                DownloadProgressText.Text = TryFindResource("Welcome__StaticData__DownloadProgress__FinishedLabel") as string;

                await Task.Delay(1000, cancellationToken.Token);
            }
            catch (TaskCanceledException)
            {
                // download has been canceled,
                // stop progress bar
                // set canceled label
                // delete bundle, if created
                DownloadProgressBar.IsIndeterminate = false;
                DownloadProgressBar.Value = DownloadProgressBar.Maximum;
                DownloadProgressText.Text = TryFindResource("Welcome__StaticData__DownloadProgress__CanceledLabel") as string;

                // delete bundle
                if (latest != null)
                {
                    mainViewModel.StaticDataManager.DeleteBundle(latest);
                }
            }

            DownloadProgressCancel.IsEnabled = false;
            // enable nav buttons
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

        private void DownloadProgressCancel_Click(object sender, RoutedEventArgs e)
        {
            cancellationToken.Cancel();
        }
    }
}
