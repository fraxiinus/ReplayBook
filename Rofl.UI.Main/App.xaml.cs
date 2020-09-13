using Etirps.RiZhi;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Files;
using Rofl.Requests;
using Rofl.Settings;
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CreateCommonObjects();

            if (e.Args.Length == 1)
            {
                var selectedFile = e.Args[0];
                // 0 = directly play, 1 = open in replaybook
                if (_settingsManager.Settings.FileAction == 0)
                {
                    PlayReplay(selectedFile);
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
            var mainWindow = new MainWindow(_log, _settingsManager, _requests, _files);
            mainWindow.Show();
        }

        private static void StartSingleReplayWindow(string path)
        {
            var singleWindow = new SingleReplayWindow
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
        }

        private async Task PlayReplay(string path)
        {
            var replay =  await _files.GetSingleFile(path).ConfigureAwait(true);
            var executables = _settingsManager.Executables.GetExecutablesByPatch(replay.ReplayFile.GameVersion);

            if (!executables.Any())
            {
                _log.Information($"No executables found to play replay");

                // No executable found that can be used to play
                MessageBox.Show
                (
                    Application.Current.TryFindResource("ExecutableNotFoundErrorText") as string + " " + replay.ReplayFile.GameVersion,
                    Application.Current.TryFindResource("ExecutableNotFoundErrorTitle") as string,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            LeagueExecutable target;
            if (executables.Count > 1)
            {
                _log.Information($"More than one possible executable, asking user...");
                // More than one?????
                target = ShowChooseReplayDialog(executables);
                if (target == null) return;
            }
            else
            {
                target = executables.First();
            }

            if (_settingsManager.Settings.PlayConfirmation)
            {
                _log.Information($"Asking user for confirmation");
                // Show confirmation dialog
                var msgResult = MessageBox.Show
                    (
                        Application.Current.TryFindResource("ReplayPlayConfirmationText") as string,
                        Application.Current.TryFindResource("ReplayPlayConfirmationText") as string,
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question
                    );

                if (msgResult != MessageBoxResult.OK) return;
            }

            _log.Information($"Using {target.Name} to play replay {replay.FileInfo.Path}");
            ReplayPlayer.Play(target, replay.FileInfo.Path);
        }

        private static LeagueExecutable ShowChooseReplayDialog(IReadOnlyCollection<LeagueExecutable> executables)
        {
            var selectWindow = new ExecutableSelectWindow
            {
                Top = App.Current.MainWindow.Top + 50,
                Left = App.Current.MainWindow.Left + 50,
                DataContext = executables,
            };

            if (selectWindow.ShowDialog().Equals(true))
            {
                return selectWindow.Selection;
            }
            return null;
        }
    }
}
