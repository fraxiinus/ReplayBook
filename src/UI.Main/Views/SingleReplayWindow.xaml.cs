using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Models.View;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using ModernWpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for SingleReplayWindow.xaml
    /// </summary>
    public partial class SingleReplayWindow : Window
    {
        private readonly FileManager _files;
        private readonly RiZhi _log;

        public string ReplayFileLocation { get; set; }

        public SingleReplayWindow(RiZhi log,
            ObservableConfiguration config,
            StaticDataManager staticData,
            ExecutableManager executables,
            FileManager files,
            ReplayPlayer player,
            bool subWindow = false)
        {
            InitializeComponent();
            
            _log = log;
            _files = files;

            // things to only do if launching by itself
            if (!subWindow)
            {
                Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
                {
                    log.Error(e.Exception.ToString());
                    log.WriteLog();
                };

                var context = new MainWindowViewModel(files, staticData, config, executables, player, log);

                DataContext = context;

                // Decide to show welcome window
                context.ShowWelcomeWindow();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            FileResult replay = null;

            try
            {
                replay = await _files.GetSingleFile(ReplayFileLocation).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to load file {ReplayFileLocation}");

                ContentDialog errDialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
                errDialog.DefaultButton = ContentDialogButton.Primary;
                errDialog.PrimaryButtonText = TryFindResource("General__ExitButton") as string;
                errDialog.Title = TryFindResource("LoadingFailureTitle") as string;

                var exceptionText = new TextBox
                {
                    Text = ex.ToString(),
                    IsReadOnly = true,
                    IsReadOnlyCaretVisible = true,
                    TextWrapping = TextWrapping.Wrap,
                    Width = 300,
                    Height = 300,
                    Margin = new Thickness(0, 20, 0, 0)
                };

                Grid.SetColumn(exceptionText, 0);
                Grid.SetRow(exceptionText, 1);
                (errDialog.Content as Grid).Children.Add(exceptionText);

                errDialog.SetLabelText(TryFindResource("FailedToLoadReplayText") as string);
                errDialog.GetContentDialogLabel().TextWrapping = TextWrapping.Wrap;
                errDialog.GetContentDialogLabel().Width = 300;

                _ = await errDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                Application.Current.Shutdown();
            }
            
            if (replay != null)
            {
                // Let the view model know about the replay
                ReplayPreview previewReplay = context.AddReplayToCollection(replay);
                var replayDetail = new ReplayDetail(context.StaticDataManager, replay, previewReplay);
                await replayDetail.LoadRunes();
                DetailView.DataContext = replayDetail;
                (DetailView.FindName("BlankContent") as Grid).Visibility = Visibility.Hidden;
                (DetailView.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

                await context.LoadItemThumbnails(replayDetail);
                await context.LoadSinglePreviewPlayerThumbnails(previewReplay);
            }
        }
    }
}
