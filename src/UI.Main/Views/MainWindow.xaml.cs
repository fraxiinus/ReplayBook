namespace Fraxiinus.ReplayBook.UI.Main;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Models.Search;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.UI.Main.Controls;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Models.View;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ObservableConfiguration _config;
    private readonly RiZhi _log;
    private ReplayPreview _lastSelection;

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
        _lastSelection = null;

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

            updateNotif.Placement = ModernWpf.Controls.Primitives.FlyoutPlacementMode.Bottom;
            updateNotif.ShowAt(SettingsButton);
        }
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

        // Select the new item
        previewModel.IsSelected = true;
        _lastSelection = previewModel;

        // Get the replay file in question, and then create the detail model
        FileResult replayFile = context.FileResults[previewModel.Location];
        var replayDetail = new ReplayDetail(context.StaticDataManager, replayFile, previewModel);

        // Set the detail control that is used for displaying the replay
        ReplayDetailControl detailControl = FindName("DetailView") as ReplayDetailControl;
        detailControl.DataContext = replayDetail;

        if (replayDetail.ErrorInfo != default)
        {
            // Hide everything besides the error display
            (detailControl.FindName("BlankContent") as Grid).Visibility = Visibility.Hidden;
            (detailControl.FindName("ErrorContent") as Grid).Visibility = Visibility.Visible;
            (detailControl.FindName("ReplayContent") as Grid).Visibility = Visibility.Hidden;
        }
        else
        {
            // Hide everything besides the replay display
            await replayDetail.LoadRunes();

            (detailControl.FindName("BlankContent") as Grid).Visibility = Visibility.Hidden;
            (detailControl.FindName("ErrorContent") as Grid).Visibility = Visibility.Hidden;
            (detailControl.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

            await (DataContext as MainWindowViewModel).LoadItemThumbnails(replayDetail);

            // See if tab control needs to update runes:
            if ((detailControl.FindName("DetailTabControl") as TabControl).SelectedIndex == 1)
            {
                await context.LoadRuneThumbnails(replayDetail).ConfigureAwait(true);
            }
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
    private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel context) { return; }

        var (received, searchResults, _) = context.LoadReplaysFromDatabase();

        if (received == 0)
        {
            // Create and show flyout above the button
            Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false, includeCustom: false);
            flyout.SetFlyoutLabelText(TryFindResource("NoReplaysFoundTitle") as string);

            flyout.ShowAt(LoadMoreButton);

            return;
        }
        else
        {
            context.StatusBarModel.StatusMessage = $"{context.PreviewReplays.Count} / {searchResults}";
        }

        // Hide the button bar once we've loaded more
        ReplayPageBar.Visibility = Visibility.Collapsed;
        await context.LoadPreviewPlayerThumbnails();
    }

    private async void SearchBox_QuerySubmitted(AutoSuggestBox auto, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (DataContext is not MainWindowViewModel context) { return; }

        context.SortParameters.QueryString = args.QueryText;

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

    private void Window_Closed(object sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel context) { return; }

        context.ClearCachedData();
    }

    private void ReplayStatusBarDismissButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel context) { return; }

        context.StatusBarModel.Visible = false;
    }

    private void ReplayStatusBar_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DataContext is not MainWindowViewModel context) { return; }

        // do not show error dialog if there are no errors
        if (context.StatusBarModel.Errors == null) { return; }

        // The status bar should NOT stay on screen once loading is done
    }
}
