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

        public ProgramLanguage LanguageToDownload { get; set; } = LanguageHelper.CurrentLanguage;

        public bool DownloadResult { get; set; }

        private MainWindowViewModel ViewModel
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
                .FirstOrDefault(x => x.StartsWith(PatchToDownload.VersionSubstring()))
                ?? throw new Exception($"failed to find patch for {PatchToDownload.VersionSubstring()}");

            progressBar.IsIndeterminate = false;
            try
            {
                await ViewModel.StaticDataManager.DownloadProperties(targetPatch,
                    StaticDataType.Rune | StaticDataType.Item | StaticDataType.Champion,
                    LanguageHelper.CurrentLanguage.GetRiotRegionCode(),
                    cancellationToken.Token);

                progressBar.Value = 33;

                await ViewModel.StaticDataManager.DownloadImages(targetPatch,
                    StaticDataType.Item | StaticDataType.Champion,
                    cancellationToken.Token);

                progressBar.Value = 66;

                await ViewModel.StaticDataManager.DownloadRuneImages(targetPatch,
                    LanguageHelper.CurrentLanguage.GetRiotRegionCode(),
                    cancellationToken.Token);

                progressBar.Value = 100;
            }
            catch (HttpRequestException)
            {
                return (false, targetPatch);
            }

            await Task.Delay(1000, cancellationToken.Token);
            return (true, targetPatch);
        }

        public static async Task StartDownloadDialog(string patch)
        {
            var retryDownload = true;
            var targetLanguage = LanguageHelper.CurrentLanguage;

            while (retryDownload)
            {
                // attempt normal download
                var downloadDialog = new StaticDataDownloadDialog()
                {
                    PatchToDownload = patch,
                    LanguageToDownload = targetLanguage
                };
                await downloadDialog.ShowAsync();

                // failed to download due to httprequestexception
                if (!downloadDialog.DownloadResult)
                {
                    // show retry dialog
                    var retryDialog = new StaticDataRetryDialog()
                    {
                        PatchToDownload = patch
                    };
                    var result = await retryDialog.ShowAsync();

                    // retry download in english
                    if (result == ContentDialogResult.Primary)
                    {
                        targetLanguage = ProgramLanguage.En;
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
