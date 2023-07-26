using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using Fraxiinus.ReplayBook.StaticData.Models;
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
    /// Interaction logic for StaticDataDownloadDialog.xaml
    /// </summary>
    public partial class StaticDataDownloadDialog : ContentDialog
    {
        private CancellationTokenSource cancellationToken;

        /// <summary>
        /// Expects full patch string from replay
        /// </summary>
        public string PatchToDownload { get; set; }

        public string LanguageToDownload { get; set; }

        public bool DownloadResult { get; set; }

        /// <summary>
        /// Error message is set if <see cref="DownloadResult"/> = true
        /// </summary>
        public string ErrorMessage { get; set; }

        private static MainWindowViewModel ViewModel
        {
            get => (Application.Current.MainWindow?.DataContext is MainWindowViewModel viewModel)
                ? viewModel
                : throw new Exception("Invalid viewmodel");
        }

        public StaticDataDownloadDialog()
        {
            InitializeComponent();
        }

        private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            //var targetPatch = Context.GameVersion.VersionSubstring();
            cancellationToken = new CancellationTokenSource();

            ProgressBarPanel.Visibility = Visibility.Visible;

            var progressBar = (ProgressBarPanel.Children[0] as ProgressBar);
            
            string createdBundle = null;
            try
            {
                (DownloadResult, createdBundle) = await StartDownload(progressBar);
            }
            catch (TaskCanceledException)
            {
                // download has been canceled,
                // stop progress bar
                // set canceled label
                progressBar.IsIndeterminate = false;
                progressBar.Value = progressBar.Maximum;

                DownloadResult = false;
            }

            if (DownloadResult == false && createdBundle != null)
            {
                // delete bundle on failure
                ViewModel.StaticDataManager.DeleteBundle(createdBundle);
            }

            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            cancellationToken.Cancel();
        }

        private async Task<(bool, string)> StartDownload(ProgressBar progressBar)
        {
            progressBar.IsIndeterminate = true;
            // find the matching target patch
            await ViewModel.StaticDataManager.GetPatchesIfOutdated(cancellationToken.Token);
            var targetPatch = ViewModel.StaticDataManager.Context.KnownPatchNumbers
                .FirstOrDefault(x => x.VersionSubstring().Equals(PatchToDownload.VersionSubstring()));

            // There is a chance the list isn't updated, refresh the list and try again
            if (targetPatch == null)
            {
                await ViewModel.StaticDataManager.RefreshPatches();
                targetPatch = ViewModel.StaticDataManager.Context.KnownPatchNumbers
                    .FirstOrDefault(x => x.VersionSubstring().Equals(PatchToDownload.VersionSubstring()))
                    ?? throw new Exception($"failed to find patch for {PatchToDownload.VersionSubstring()}");
            }

            progressBar.IsIndeterminate = false;
            try
            {
                await ViewModel.StaticDataManager.DownloadProperties(targetPatch,
                    StaticDataType.Rune | StaticDataType.Item | StaticDataType.Champion,
                    LanguageToDownload,
                    cancellationToken.Token);

                progressBar.Value = 33;

                await ViewModel.StaticDataManager.DownloadImages(targetPatch,
                    StaticDataType.Item | StaticDataType.Champion,
                    cancellationToken.Token);

                progressBar.Value = 66;

                await ViewModel.StaticDataManager.DownloadRuneImages(targetPatch,
                    LanguageToDownload,
                    cancellationToken.Token);

                progressBar.Value = 100;
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.ToString();
                return (false, targetPatch);
            }

            // save bundle information
            await ViewModel.StaticDataManager.SaveIndexAsync();

            await Task.Delay(1000, cancellationToken.Token);
            return (true, targetPatch);
        }

        /// <summary>
        /// Open dialog and start download for a given patch. Includes provisions for retrying
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public static async Task StartDownloadDialog(string patch, string targetLanguageCode)
        {
            var retryDownload = true;

            while (retryDownload)
            {
                // attempt normal download
                var downloadDialog = new StaticDataDownloadDialog()
                {
                    PatchToDownload = patch,
                    LanguageToDownload = targetLanguageCode
                };
                await downloadDialog.ShowAsync();

                // failed to download due to httprequestexception
                if (!downloadDialog.DownloadResult)
                {
                    // show retry dialog
                    var retryDialog = new StaticDataRetryDialog()
                    {
                        PatchToDownload = patch,
                        LanguageTried = targetLanguageCode,
                        HttpErrorMessage = downloadDialog.ErrorMessage
                    };
                    var result = await retryDialog.ShowAsync();

                    // retry download in english
                    if (result == ContentDialogResult.Primary)
                    {
                        targetLanguageCode = ConfigurationDefinitions.GetRiotRegionCode(LeagueLocale.EnglishUS);
                    }
                    else
                    {
                        retryDownload = false;
                    }
                }
                else
                {
                    retryDownload = false;
                }
            }
        }
    }
}
