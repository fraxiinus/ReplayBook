using Etirps.RiZhi;
using Rofl.Files;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            var context = new MainWindowViewModel(_files, _requests, _settingsManager, _player, _log);
            this.DataContext = context;

            // Decide to show welcome window
            context.ShowWelcomeWindow();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            var replay = await _files.GetSingleFile(ReplayFileLocation).ConfigureAwait(true);

            if (replay == null)
            {
                _log.Error($"Failed to load file {ReplayFileLocation}");
                MessageBox.Show((string)TryFindResource("FailedToLoadReplayText"),
                                (string)TryFindResource("ErrorTitle"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
            else
            {
                // Let the view model know about the replay
                var previewReplay = context.AddReplay(replay);
                var replayDetail = new ReplayDetail(replay, previewReplay);
                DetailView.DataContext = replayDetail;
                (DetailView.FindName("BlankContent") as Grid).Visibility = Visibility.Hidden;
                (DetailView.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

                await context.LoadItemThumbnails(replayDetail).ConfigureAwait(true);
                await context.LoadSinglePreviewPlayerThumbnails(previewReplay).ConfigureAwait(true);
            }
        }
    }
}
