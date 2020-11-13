using Etirps.RiZhi;
using ModernWpf.Controls;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Files;
using Rofl.Settings;
using Rofl.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<Process> PlayReplay(string path)
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
                return null;
            }

            LeagueExecutable target;
            if (executables.Count > 1)
            {
                _log.Information($"More than one possible executable, asking user...");
                // More than one?????
                target = await ShowChooseReplayDialog(executables).ConfigureAwait(true);
                if (target == null) return null;
            }
            else
            {
                target = executables.First();
            }

            if (_settingsManager.Settings.PlayConfirmation)
            {
                _log.Information($"Asking user for confirmation");
                // Creating content dialog
                var dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: true);
                dialog.DefaultButton = ContentDialogButton.Primary;

                dialog.PrimaryButtonText = Application.Current.TryFindResource("YesText") as string;
                dialog.SecondaryButtonText = Application.Current.TryFindResource("NoText") as string;
                dialog.Title = Application.Current.TryFindResource("ReplayPlayConfirmTitle") as string;
                dialog.SetLabelText(Application.Current.TryFindResource("ReplayPlayConfirmOptOut") as string);

                // Handle both primary and secondary button clicks
                dialog.PrimaryButtonClick += (ContentDialog sender, ContentDialogButtonClickEventArgs args) =>
                {
                    dialog.Hide();
                };

                dialog.CloseButtonClick += (ContentDialog sender, ContentDialogButtonClickEventArgs args) =>
                {
                    dialog.Hide();
                };

                // Only continue if the user pressed the yes button
                var dialogResult = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                if (dialogResult != ContentDialogResult.Primary)
                {
                    return null;
                }
            }

            _log.Information($"Using {target.Name} to play replay {replay.FileInfo.Path}");
            return target.PlayReplay(replay.FileInfo.Path);
        }

        private static async Task<LeagueExecutable> ShowChooseReplayDialog(IReadOnlyCollection<LeagueExecutable> executables)
        {
            var dialog = new ExecutableSelectDialog
            {
                Owner = Application.Current.MainWindow,
                DataContext = executables
            };

            await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

            return dialog.Selection;
        }
    }
}
