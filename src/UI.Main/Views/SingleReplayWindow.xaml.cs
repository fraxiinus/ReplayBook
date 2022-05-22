using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.Requests;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
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
            RequestManager requests,
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

                var context = new MainWindowViewModel(files, requests, staticData, config, executables, player, log);

                DataContext = context;

                // Decide to show welcome window
                context.ShowWelcomeWindow();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            Files.Models.FileResult replay = await _files.GetSingleFile(ReplayFileLocation).ConfigureAwait(true);

            if (replay == null)
            {
                _log.Error($"Failed to load file {ReplayFileLocation}");
                _ = MessageBox.Show((string)TryFindResource("FailedToLoadReplayText"),
                                (string)TryFindResource("ErrorTitle"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
            else
            {
                // Let the view model know about the replay
                ReplayPreview previewReplay = context.AddReplay(replay);
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
