using Etirps.RiZhi;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Files;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CreateCommonObjects();

            if (e.Args.Length == 1)
            {
                var selectedFile = e.Args[0];
                // 0 = directly play, 1 = open in replaybook
                if (_settingsManager.Settings.FileAction == 0)
                {
                    _player.PlayReplay(selectedFile);
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
    }
}
