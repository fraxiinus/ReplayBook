using Etirps.RiZhi;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Files;
using Rofl.Settings;
using Rofl.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rofl.UI.Main.Utilities
{
    public class ReplayPlayer
    {
        private FileManager _files;
        private SettingsManager _settingsManager;
        private RiZhi _log;

        public ReplayPlayer(FileManager files, SettingsManager settings, RiZhi log)
        {
            _files = files;
            _settingsManager = settings;
            _log = log;
        }

        public async Task PlayReplay(string path)
        {
            var replay = await _files.GetSingleFile(path).ConfigureAwait(true);
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
            target.PlayReplay(replay.FileInfo.Path);
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
