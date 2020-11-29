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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
                await ShowUnsupportedDialog(replay.ReplayFile.GameVersion).ConfigureAwait(true);

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
                
                // Only continue if the user pressed the yes button
                var dialogResult = await ShowConfirmationDialog().ConfigureAwait(true);
                if (dialogResult != ContentDialogResult.Primary)
                {
                    return null;
                }
            }

            _log.Information($"Using {target.Name} to play replay {replay.FileInfo.Path}");

            Process gameHandle = null;
            try
            {
                gameHandle = target.PlayReplay(replay.FileInfo.Path);
            }
            catch (Exception ex)
            {
                await ShowExceptionDialog(ex).ConfigureAwait(true);
                _log.Error(ex.ToString());
            }

            return gameHandle;
        }

        private static async Task<LeagueExecutable> ShowChooseReplayDialog(IReadOnlyCollection<LeagueExecutable> executables)
        {
            var dialog = new ExecutableSelectDialog
            {
                Owner = Application.Current.MainWindow,
                DataContext = executables
            };

            // Make background overlay transparent when in the dialog host window,
            // making the dialog appear seamlessly
            if (Application.Current.MainWindow is DialogHostWindow)
            {
                // allows us to look at the visual tree before showing the dialog
                dialog.ApplyTemplate();

                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

            return dialog.Selection;
        }

        private static async Task<ContentDialogResult> ShowConfirmationDialog()
        {
            // Creating content dialog
            var dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: true);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = Application.Current.TryFindResource("YesText") as string;
            dialog.SecondaryButtonText = Application.Current.TryFindResource("NoText") as string;
            dialog.Title = Application.Current.TryFindResource("ReplayPlayConfirmTitle") as string;
            dialog.SetLabelText(Application.Current.TryFindResource("ReplayPlayConfirmOptOut") as string);

            // Make background overlay transparent when in the dialog host window,
            // making the dialog appear seamlessly
            if (Application.Current.MainWindow is DialogHostWindow)
            {
                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            return await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private static async Task ShowUnsupportedDialog(string version)
        {
            // Creating content dialog
            var dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = Application.Current.TryFindResource("OKButtonText") as string;
            dialog.Title = Application.Current.TryFindResource("ExecutableNotFoundErrorTitle") as string;
            dialog.SetLabelText(Application.Current.TryFindResource("ExecutableNotFoundErrorText") as string + " " + version);

            // Make background overlay transparent when in the dialog host window,
            // making the dialog appear seamlessly
            if (Application.Current.MainWindow is DialogHostWindow)
            {
                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private static async Task ShowExceptionDialog(Exception ex)
        {
            // Creating content dialog
            var dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = Application.Current.TryFindResource("OKButtonText") as string;
            dialog.Title = Application.Current.TryFindResource("ReplayPlayExceptionTitle") as string;
            dialog.SetLabelText(ex.ToString());

            // Make dialog as long as the exception
            var label = dialog.GetContentDialogLabel();
            label.MaxWidth = 350;
            label.TextWrapping = TextWrapping.Wrap;

            // Make background overlay transparent when in the dialog host window,
            // making the dialog appear seamlessly
            if (Application.Current.MainWindow is DialogHostWindow)
            {
                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }
    }
}
