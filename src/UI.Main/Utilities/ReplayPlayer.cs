using Etirps.RiZhi;
using ModernWpf.Controls;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Executables.Old.Models;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.ServiceProcess;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
{
    public class ReplayPlayer
    {
        private readonly FileManager _files;
        private readonly ObservableConfiguration _config;
        private readonly RiZhi _log;
        private readonly ExecutableManager _executables;

        public ReplayPlayer(FileManager files, ObservableConfiguration config, ExecutableManager executables, RiZhi log)
        {
            _files = files;
            _config = config;
            _executables = executables;
            _log = log;
        }

        public async Task<Process> PlayReplay(string path)
        {
            Files.Models.FileResult replay = await _files.GetSingleFile(path).ConfigureAwait(true);
            if (replay is null)
            {
                // replay file could not be read
                await ShowExceptionDialog(
                    new NotSupportedException(
                        Application.Current.TryFindResource("FailedToLoadReplayText").ToString()))
                    .ConfigureAwait(true);

                return null;
            }

            IReadOnlyCollection<LeagueExecutable> executables = _executables.GetExecutablesByPatch(replay.ReplayFile.GameVersion);
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
                if (target == null) { return null; }
            }
            else
            {
                target = executables.First();
            }

            if (_config.PlayConfirm)
            {
                _log.Information($"Asking user for confirmation");

                // Only continue if the user pressed the yes button
                ContentDialogResult dialogResult = await ShowConfirmationDialog().ConfigureAwait(true);
                if (dialogResult != ContentDialogResult.Primary)
                {
                    return null;
                }
            }

            _log.Information($"Using {target.Name} to play replay {replay.FileInfo.Path}");

            if (VanguardServiceHelper.IsVanguardRunning())
            {
                var vanguardDisabled = await ShowVanguardDialog();
                if (!vanguardDisabled) // could not disable vanguard, stop play attempt
                {
                    return null;
                }
            }

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
                _ = dialog.ApplyTemplate();

                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

            return dialog.Selection;
        }

        private static async Task<ContentDialogResult> ShowConfirmationDialog()
        {
            // Creating content dialog
            ContentDialog dialog = ContentDialogHelper.CreateContentDialog(
                title: Application.Current.TryFindResource("ReplayPlayConfirmTitle") as string,
                description: Application.Current.TryFindResource("ReplayPlayConfirmOptOut") as string,
                primaryButtonText: Application.Current.TryFindResource("YesText") as string,
                secondaryButtonText: Application.Current.TryFindResource("NoText") as string);
            dialog.DefaultButton = ContentDialogButton.Primary;

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
            ContentDialog dialog = ContentDialogHelper.CreateContentDialog(
                title: Application.Current.TryFindResource("ExecutableNotFoundErrorTitle") as string,
                description: (Application.Current.TryFindResource("ExecutableNotFoundErrorText") as string) + " " + version,
                primaryButtonText: Application.Current.TryFindResource("OKButtonText") as string);

            // Make background overlay transparent when in the dialog host window,
            // making the dialog appear seamlessly
            if (Application.Current.MainWindow is DialogHostWindow)
            {
                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private static async Task ShowExceptionDialog(Exception ex)
        {
            // Creating content dialog
            ContentDialog dialog = ContentDialogHelper.CreateContentDialog(
                title: Application.Current.TryFindResource("ReplayPlayExceptionTitle") as string,
                description: ex.ToString(),
                primaryButtonText: Application.Current.TryFindResource("OKButtonText") as string,
                labelWidth: 350);

            // Make background overlay transparent when in the dialog host window,
            // making the dialog appear seamlessly
            if (Application.Current.MainWindow is DialogHostWindow)
            {
                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private static async Task<bool> ShowVanguardDialog()
        {
            // inform the user that replays cannot be played while vanguard is running
            var dialog = ContentDialogHelper.CreateContentDialog(
                title: Application.Current.TryFindResource("Main__VanguardWarning__Title") as string,
                description: Application.Current.TryFindResource("Main__VanguardWarning__Body") as string,
                primaryButtonText: Application.Current.TryFindResource("YesText") as string,
                secondaryButtonText: Application.Current.TryFindResource("NoText") as string);
            
            // Make background overlay transparent when in the dialog host window,
            // making the dialog appear seamlessly
            if (Application.Current.MainWindow is DialogHostWindow)
            {
                dialog.SetBackgroundSmokeColor(Brushes.Transparent);
            }

            var diagResult = await dialog.ShowAsync(ContentDialogPlacement.Popup);

            // Start attempt to disable Vanguard
            if (diagResult == ContentDialogResult.Primary)
            {
                var disableVanguardDialog = new VanguardDisableDialog();
                _ = await disableVanguardDialog.ShowAsync(ContentDialogPlacement.Popup);
                if (!disableVanguardDialog.Success)
                {
                    await ShowExceptionDialog(disableVanguardDialog.Exception);
                }
                return disableVanguardDialog.Success;
            }
            
            return false;
        }
    }
}
