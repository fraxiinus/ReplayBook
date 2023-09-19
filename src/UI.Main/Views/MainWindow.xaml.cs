namespace Fraxiinus.ReplayBook.UI.Main;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Pages;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Windows.UI.ApplicationSettings;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ObservableConfiguration _config;
    private readonly RiZhi _log;

    private readonly MainReplayPage _replayPage;
    private readonly SettingsPage _settingsPage;

    public MainWindow(RiZhi log,
        ObservableConfiguration config,
        StaticDataManager staticData,
        ExecutableManager executables,
        FileManager files,
        ReplayPlayer player)
    {
        InitializeComponent();

        _log = log;
        _config = config;

        // Catch all unhandled exceptions to write logs
        Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
        {
            log.Error(e.Exception.ToString());
            log.WriteLog();

            MessageBox.Show(
                messageBoxText: (TryFindResource("General__GlobalExceptionHandler__Body") as string) + $"\nException: {e.Exception.GetType()} {e.Exception.Message}",
                caption: TryFindResource("ErrorTitle") as string,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        };

        // Create new viewmodel and set as mainwindow context
        var context = new MainWindowViewModel(files, staticData, config, executables, player, log);
        DataContext = context;

        // Decide to show welcome window
        context.ShowWelcomeWindow();

        // Check if replay folders are missing and need to be removed
        context.ShowMissingReplayFoldersMessageBox();

        // Create pages
        _replayPage = new MainReplayPage
        {
            DataContext = context
        };
        _settingsPage = new SettingsPage
        {
            DataContext = new SettingsWindowDataContext
            {
                Configuration = config,
                Executables = executables,
                StaticData = staticData
            }
        };

        // Navigate to first page
        MainNavigationView.SelectedItem = MainNavigationView.MenuItems.OfType<NavigationViewItem>().First();
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

    /// <summary>
    /// Window has been rendered to the screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Window_ContentRendered(object sender, EventArgs e)
    {
        // Auto check for updates
        if (_config.AutoUpdateCheck)
        {
            _config.Stash["UpdateAvailable"] = await GithubConnection.CheckForUpdate(_log);
        }

        // If update claims to be available, show popup
        var updateVariableExists = _config.Stash.TryGetBool("UpdateAvailable", out bool updateAvailable);
        if (updateVariableExists && updateAvailable)
        {
            Flyout updateNotif = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: true);
            updateNotif.SetFlyoutLabelText(TryFindResource("UpdateAvailableNotifText") as string);
            updateNotif.SetFlyoutButtonText(TryFindResource("General__DownloadButton") as string);
            
            // Button opens github releases page
            updateNotif.GetFlyoutButton().Click += (object e1, RoutedEventArgs a) =>
            {
                _ = Process.Start("explorer", (TryFindResource("GitHubReleasesLink") as Uri).ToString());
            };

            updateNotif.Placement = ModernWpf.Controls.Primitives.FlyoutPlacementMode.RightEdgeAlignedBottom;

            //updateNotif.ShowAt(SettingsButton);
            updateNotif.ShowAt(MainNavigationView.SettingsItem as NavigationViewItem);
        }
    }

    private async void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (DataContext is not MainWindowViewModel context) { return; }

        if (args.IsSettingsSelected)
        {
            MainNavigationFrame.Navigate(_settingsPage);
            //await context.ShowSettingsDialog().ConfigureAwait(true);
        }
        else if (args.SelectedItem is NavigationViewItem selectedNVItem)
        {
            switch (selectedNVItem.Tag as string)
            {
                case "ReplayPage":
                    MainNavigationFrame.Navigate(_replayPage);
                    break;
                case "PlayerPage":
                    MainNavigationFrame.Navigate(typeof(PlayersPage));
                    break;
                case "ExecutablePage":
                    MainNavigationFrame.Navigate(typeof(ExecutablesPage));
                    break;
                default:
                    break;
            }
        }
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

    private void Window_Closed(object sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel context) { return; }

        context.ClearCachedData();
    }

    // TODO: the themeing gets weird when toggling pinned nav, might need to be mroe involved than this
    //private void PinNavigationItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //{
    //    if (MainNavigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact)
    //    {
    //        MainNavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
    //    }
    //    else
    //    {
    //        MainNavigationView.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
    //    }
    //}
}
