﻿using Etirps.RiZhi;
using ModernWpf.Controls;
using Fraxiinus.ReplayBook.Configuration;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Requests;
using Fraxiinus.ReplayBook.UI.Main.Controls;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;
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

namespace Fraxiinus.ReplayBook.UI.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableConfiguration _config;
        private readonly RiZhi _log;

        private ReplayPreview _lastSelection;

        public MainWindow(RiZhi log, ObservableConfiguration config, RequestManager requests, ExecutableManager executables, FileManager files, ReplayPlayer player)
        {
            InitializeComponent();

            _log = log;
            _config = config;

            _lastSelection = null;

            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
            {
                log.Error(e.Exception.ToString());
                log.WriteLog();
            };

            var context = new MainWindowViewModel(files, requests, config, executables, player, log);
            DataContext = context;

            // Decide to show welcome window
            context.ShowWelcomeWindow();
            context.ShowMissingReplayFoldersMessageBox();
        }

        public void SelectReplayItem(ReplayPreview replay)
        {
            ReplayListView.SelectedItem = replay;
        }

        /// <summary>
        /// Applies saved window position, size, and maximization state
        /// </summary>
        public void RestoreSavedWindowState()
        {
            Dictionary<string, object> values = _config.Stash;

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

        // Window is loaded and ready to be shown on screen
        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            await context.StaticDataProvider.LoadStaticData();
        }

        // Window has been rendered to the screen
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            if (!_config.AutoUpdateCheck) { return; }

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
                _config.Stash["UpdateAvailable"] = false;
            }
            else
            {
                _config.Stash["UpdateAvailable"] = true;

                Flyout updateNotif = FlyoutHelper.CreateFlyout(true, true);
                updateNotif.SetFlyoutLabelText(TryFindResource("UpdateAvailableNotifText") as string);
                updateNotif.SetFlyoutButtonText(TryFindResource("UpdateAvailableNotifButton") as string);

                updateNotif.GetFlyoutButton().Click += (object e1, RoutedEventArgs a) =>
                {
                    _ = Process.Start("explorer", (TryFindResource("GitHubReleasesLink") as Uri).ToString());
                };

                updateNotif.Placement = ModernWpf.Controls.Primitives.FlyoutPlacementMode.Bottom;
                updateNotif.ShowAt(SettingsButton);
            }

            return;
        }

        private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            await context.ReloadReplayList(false).ConfigureAwait(true);
        }

        private async void ReplayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }
            if (sender is not System.Windows.Controls.ListView replayList) { return; }
            if (replayList.SelectedItem is not ReplayPreview previewModel) { return; }

            // Deselect the last selected item
            if (_lastSelection != null && _lastSelection.IsSelected) { _lastSelection.IsSelected = false; }

            previewModel.IsSelected = true;
            _lastSelection = previewModel;

            FileResult replayFile = context.FileResults[previewModel.Location];

            var replayDetail = new ReplayDetail(context.StaticDataProvider, replayFile, previewModel);

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
            if (DataContext is not MainWindowViewModel context) { return; }
            if (sender is not Button sortButton) { return; }

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
            if (DataContext is not MainWindowViewModel context) { return; }
            if (sender is not RadioMenuItem selectedItem) { return; }

            if (Enum.TryParse(selectedItem.Name, out SortMethod selectSort))
            {
                context.SortParameters.SortMethod = selectSort;
            }

            await context.ReloadReplayList(false).ConfigureAwait(false);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            await context.ShowSettingsDialog().ConfigureAwait(true);
        }

        /// <summary>
        /// Display or hide LoadMoreButton if scrolled to the bottom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplayListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel) { return; }

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
        private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

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
            if (DataContext is not MainWindowViewModel context) { return; }
            if (e.Key != System.Windows.Input.Key.Enter) { return; }
            if (sender is not AutoSuggestBox searchBox) { return; }

            context.SortParameters.SearchTerm = searchBox.Text;

            await context.ReloadReplayList(false);
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox auto, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (DataContext is not MainWindowViewModel context) { return; }
            if (string.IsNullOrEmpty(args.QueryText))
            {
                await context.ReloadReplayList(false).ConfigureAwait(true);
            }

            context.SortParameters.SearchTerm = args.QueryText;

            await context.ReloadReplayList(false);
        }

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            context.ClearDeletedReplays();

            _config.Stash["WindowHeight"] = Height;
            _config.Stash["WindowWidth"] = Width;
            _config.Stash["WindowLeft"] = Left;
            _config.Stash["WindowTop"] = Top;
            _config.Stash["WindowMaximized"] = WindowState == WindowState.Maximized;

            await _config.ToConfigurationFile().SaveConfigurationFile();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            await context.ClearCache().ConfigureAwait(true);
        }

        private void ReplayStatusBarDismissButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            context.StatusBarModel.Visible = false;
        }

        private async void ReplayStatusBar_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is not MainWindowViewModel context) { return; }

            // do not show error dialog if there are no errors
            if (context.StatusBarModel.Errors == null) { return; }

            var errorDialog = new ReplayLoadErrorDialog
            {
                DataContext = context.StatusBarModel
            };
            _ = await errorDialog.ShowAsync().ConfigureAwait(true);
        }

    }
}
