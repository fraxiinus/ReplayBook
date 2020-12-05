using Etirps.RiZhi;
using ModernWpf;
using Rofl.Files;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Rofl.UI.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FileManager _files;
        private RequestManager _requests;
        private SettingsManager _settingsManager;
        private RiZhi _log;
        private ReplayPlayer _player;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            CreateCommonObjects();

            // Apply appearence theme
            ApplyThemeSetting();

            if (e.Args.Length == 1)
            {
                var selectedFile = e.Args[0];
                // 0 = directly play, 1 = open in replaybook
                if (_settingsManager.Settings.FileAction == 0)
                {
                    StartDialogHost();
                    await _player.PlayReplay(selectedFile).ConfigureAwait(true);
                    Application.Current.Shutdown();
                }
                else if (_settingsManager.Settings.FileAction == 1)
                {
                    StartSingleReplayWindow(selectedFile);
                }
                else { }

            }
            else
            {
                StartMainWindow();
            }
        }

        private void StartDialogHost()
        {
            // Start a blank/invisible window that will host dialogs, otherwise dialogs are invisible
            var host = new DialogHostWindow();
            host.Show();
        }

        private void StartMainWindow()
        {
            var mainWindow = new MainWindow(_log, _settingsManager, _requests, _files, _player);
            mainWindow.Show();
        }

        private void StartSingleReplayWindow(string path)
        {
            var singleWindow = new SingleReplayWindow(_log, _settingsManager, _requests, _files, _player)
            {
                ReplayFileLocation = path
            };
            singleWindow.Show();
        }

        private void CreateCommonObjects()
        {
            // Create common objects
            var assemblyName = Assembly.GetEntryAssembly()?.GetName();

            _log = new RiZhi()
            {
                FilePrefix = "ReplayBookLog",
                AssemblyName = assemblyName.Name,
                AssemblyVersion = assemblyName.Version.ToString(2)
            };

            _log.Error($"Log files are generated for each run while in prerelease");

            _settingsManager = new SettingsManager(_log);
            _files = new FileManager(_settingsManager.Settings, _log);
            _requests = new RequestManager(_settingsManager.Settings, _log);
            _player = new ReplayPlayer(_files, _settingsManager, _log);
        }

        private void ApplyThemeSetting()
        {
            // Set theme mode
            switch (_settingsManager.Settings.ThemeMode)
            {
                case 0: // system default
                    ThemeManager.Current.ApplicationTheme = null;
                    break;
                case 1: // 
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    break;
                case 2: // light
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    break;
            }

            // Set accent color
            if (_settingsManager.Settings.AccentColor == null)
            {
                ThemeManager.Current.AccentColor = null;
            }
            else
            {
                ThemeManager.Current.AccentColor = (Color)ColorConverter.ConvertFromString(_settingsManager.Settings.AccentColor);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _log.WriteLog();
        }
    }
}
