using Etirps.RiZhi;
using Rofl.Files;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for SingleReplayWindow.xaml
    /// </summary>
    public partial class SingleReplayWindow : Window
    {
        private readonly FileManager _files;
        private readonly RequestManager _requests;
        private readonly SettingsManager _settingsManager;
        private readonly RiZhi _log;
        private readonly ReplayPlayer _player;

        public string ReplayFileLocation { get; set; }

        public SingleReplayWindow(RiZhi log, SettingsManager settingsManager, RequestManager requests, FileManager files, ReplayPlayer player)
        {
            InitializeComponent();

            _log = log;
            _settingsManager = settingsManager;
            _requests = requests;
            _files = files;
            _player = player;

            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
            {
                _log.Error(e.Exception.ToString());
                _log.WriteLog();
            };

            MainWindowViewModel context = new MainWindowViewModel(_files, _requests, _settingsManager, _player, _log);
            DataContext = context;

            // Decide to show welcome window
            context.ShowWelcomeWindow();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

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
                ReplayDetail replayDetail = new ReplayDetail(replay, previewReplay);
                DetailView.DataContext = replayDetail;
                (DetailView.FindName("BlankContent") as Grid).Visibility = Visibility.Hidden;
                (DetailView.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

                await context.LoadItemThumbnails(replayDetail).ConfigureAwait(true);
                await context.LoadSinglePreviewPlayerThumbnails(previewReplay).ConfigureAwait(true);
            }
        }
    }
}
