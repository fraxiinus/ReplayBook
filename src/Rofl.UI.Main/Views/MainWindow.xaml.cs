using Etirps.RiZhi;
using ModernWpf.Controls;
using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Rofl.UI.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileManager _files;
        private readonly RequestManager _requests;
        private readonly SettingsManager _settingsManager;
        private readonly RiZhi _log;
        private readonly ReplayPlayer _player;

        private ReplayPreview _lastSelection;

        public MainWindow(RiZhi log, SettingsManager settingsManager, RequestManager requests, FileManager files, ReplayPlayer player)
        {
            InitializeComponent();

            _log = log;
            _settingsManager = settingsManager;
            _requests = requests;
            _files = files;
            _player = player;

            _lastSelection = null;

            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
            {
                _log.Error(e.Exception.ToString());
                _log.WriteLog();
            };

            MainWindowViewModel context = new MainWindowViewModel(_files, _requests, _settingsManager, _player, _log);
            DataContext = context;

            // Decide to show welcome window
            context.ShowWelcomeWindow();
            context.ShowMissingReplayFoldersMessageBox();
        }

        // Window is loaded and ready to be shown on screen
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> values = _settingsManager.TemporaryValues;

            if (values.TryGetDouble("WindowHeight", out double savedHeight) &&
                values.TryGetDouble("WindowWidth", out double savedWidth) &&
                values.TryGetDouble("WindowLeft", out double savedLeft) &&
                values.TryGetDouble("WindowTop", out double savedTop) &&
                values.TryGetBool("WindowMaximized", out bool savedMaximized))
            {
                Height = savedHeight;
                Width = savedWidth;
                Left = savedLeft;
                Top = savedTop;
                if (savedMaximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }
        }

        // Window has been rendered to the screen
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            if (!_settingsManager.Settings.AutoUpdateCheck) { return; }

            string latestVersion;
            try
            {
                _log.Information("Checking for updates...");
                latestVersion = await GithubConnection.GetLatestVersion().ConfigureAwait(true);
            }
            catch (HttpRequestException ex)
            {
                _log.Warning("Failed to check for updates - " + ex.ToString());
                return; // keep in mind when adding anything to this function!
            }

            if (string.IsNullOrEmpty(latestVersion))
            {
                _log.Warning("Failed to check for updates - github returned nothing or error code");
                return; // keep in mind when adding anything to this function!
            }

            AssemblyName assemblyName = Assembly.GetEntryAssembly()?.GetName();
            string assemblyVersion = assemblyName.Version.ToString(3);

            if (latestVersion.Equals(assemblyVersion, StringComparison.OrdinalIgnoreCase))
            {
                _settingsManager.TemporaryValues["UpdateAvailable"] = false;
            }
            else
            {
                _settingsManager.TemporaryValues["UpdateAvailable"] = true;

                Flyout updateNotif = FlyoutHelper.CreateFlyout(true, true);
                updateNotif.SetFlyoutLabelText(TryFindResource("UpdateAvailableNotifText") as string);
                updateNotif.SetFlyoutButtonText(TryFindResource("UpdateAvailableNotifButton") as string);

                updateNotif.GetFlyoutButton().Click += (object e1, RoutedEventArgs a) =>
                {
                    _ = Process.Start((TryFindResource("GitHubReleasesLink") as Uri).ToString());
                };

                updateNotif.Placement = ModernWpf.Controls.Primitives.FlyoutPlacementMode.Bottom;
                updateNotif.ShowAt(SettingsButton);
            }

            return;
        }

        private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            await context.ReloadReplayList().ConfigureAwait(true);
        }

        private async void ReplayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is System.Windows.Controls.ListView replayList)) { return; }
            if (!(replayList.SelectedItem is ReplayPreview previewModel)) { return; }

            // Deselect the last selected item
            if (_lastSelection != null && _lastSelection.IsSelected) { _lastSelection.IsSelected = false; }

            previewModel.IsSelected = true;
            _lastSelection = previewModel;

            FileResult replayFile = context.FileResults[previewModel.Location];

            ReplayDetail replayDetail = new ReplayDetail(replayFile, previewModel);

            ReplayDetailControl detailControl = FindName("DetailView") as ReplayDetailControl;
            detailControl.DataContext = replayDetail;

            (detailControl.FindName("BlankContent") as Grid).Visibility = Visibility.Hidden;
            (detailControl.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

            (DataContext as MainWindowViewModel).LoadItemThumbnails(replayDetail);

            // See if tab control needs to update runes:
            if ((detailControl.FindName("DetailTabControl") as TabControl).SelectedIndex == 1)
            {
                await context.LoadRuneThumbnails(replayDetail).ConfigureAwait(true);
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is Button sortButton)) { return; }

            // Get the button and menu
            ContextMenu contextMenu = sortButton.ContextMenu;
            // Set placement and open
            contextMenu.PlacementTarget = sortButton;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;

            string name = Enum.GetName(context.SortParameters.SortMethod.GetType(), context.SortParameters.SortMethod);
            if (FindName(name) is RadioMenuItem selectItem)
            {
                // Select our item
                selectItem.IsChecked = true;
            }
        }

        /// <summary>
        /// Sort menu item click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is RadioMenuItem selectedItem)) { return; }

            if (Enum.TryParse(selectedItem.Name, out SortMethod selectSort))
            {
                context.SortParameters.SortMethod = selectSort;
            }

            await context.ReloadReplayList().ConfigureAwait(false);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            await context.ShowSettingsDialog().ConfigureAwait(true);
        }

        /// <summary>
        /// Display or hide LoadMoreButton if scrolled to the bottom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplayListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel)) { return; }

            // If we scrolled at all...
            if (Math.Abs(e.VerticalChange) > 0)
            {
                // If we reached the end, show the button!!!
                ReplayPageBar.Visibility = e.VerticalOffset + e.ViewportHeight == e.ExtentHeight
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handler for LoadMoreButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            if (context.LoadReplaysFromDatabase() == 0)
            {
                // Create and show flyout above the button
                Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false, includeCustom: false);
                flyout.SetFlyoutLabelText(TryFindResource("NoReplaysFoundTitle") as string);

                flyout.ShowAt(LoadMoreButton);

                return;
            }

            // Hide the button bar once we've loaded more
            ReplayPageBar.Visibility = Visibility.Collapsed;
            context.LoadPreviewPlayerThumbnails();
        }

        private async void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }
            if (e.Key != System.Windows.Input.Key.Enter) { return; }
            if (!(sender is AutoSuggestBox searchBox)) { return; }

            context.SortParameters.SearchTerm = searchBox.Text;

            context.ClearReplays();
            _ = context.LoadReplaysFromDatabase();
            context.LoadPreviewPlayerThumbnails();
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox auto, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }
            if (string.IsNullOrEmpty(args.QueryText))
            {
                context.ValidateReplayStorage(closeOnComplete: true);
                await context.ReloadReplayList().ConfigureAwait(true);
            }

            context.SortParameters.SearchTerm = args.QueryText;

            context.ClearReplays();
            _ = context.LoadReplaysFromDatabase();
            context.LoadPreviewPlayerThumbnails();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            context.ClearDeletedReplays();

            _settingsManager.TemporaryValues["WindowHeight"] = Height;
            _settingsManager.TemporaryValues["WindowWidth"] = Width;
            _settingsManager.TemporaryValues["WindowLeft"] = Left;
            _settingsManager.TemporaryValues["WindowTop"] = Top;
            _settingsManager.TemporaryValues["WindowMaximized"] = WindowState == WindowState.Maximized;
            _settingsManager.SaveTemporaryValues();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            await context.ClearCache().ConfigureAwait(true);
        }

        public void SelectReplayItem(ReplayPreview replay)
        {
            ReplayListView.SelectedItem = replay;
        }

        private void ReplayStatusBarDismissButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            context.StatusBarModel.Visible = false;
        }

        private async void ReplayStatusBar_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            // do not show error dialog if there are no errors
            if (context.StatusBarModel.Errors == null) { return; }

            ReplayLoadErrorDialog errorDialog = new ReplayLoadErrorDialog
            {
                DataContext = context.StatusBarModel
            };
            _ = await errorDialog.ShowAsync().ConfigureAwait(true);
        }
    }
}
