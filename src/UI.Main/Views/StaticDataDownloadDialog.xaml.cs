using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using ModernWpf.Controls;
using System;
using System.Linq;
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

        private ReplayPreview Context
        {
            get => (DataContext is ReplayPreview context)
                ? context
                : throw new Exception("Invalid data context");
        }

        private MainWindowViewModel ViewModel
        {
            get => (Window.GetWindow(this)?.DataContext is MainWindowViewModel viewModel)
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
            progressBar.IsIndeterminate = false;
            string targetPatch = null;
            try
            {
                // find the matching target patch
                await ViewModel.StaticDataManager.GetPatchesIfOutdated(cancellationToken.Token);
                targetPatch = ViewModel.StaticDataManager.Context.KnownPatchNumbers
                    .FirstOrDefault(x => x.StartsWith(Context.GameVersion.VersionSubstring()))
                    ?? throw new Exception($"failed to find patch for {Context.GameVersion.VersionSubstring()}");

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

                await Task.Delay(1000, cancellationToken.Token);
            }
            catch (TaskCanceledException)
            {
                // download has been canceled,
                // stop progress bar
                // set canceled label
                // delete bundle, if created
                progressBar.IsIndeterminate = false;
                progressBar.Value = progressBar.Maximum;

                // delete bundle
                if (targetPatch != null)
                {
                    ViewModel.StaticDataManager.DeleteBundle(targetPatch);
                }
            }
            
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            cancellationToken.Cancel();
        }
    }
}
