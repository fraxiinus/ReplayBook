using Etirps.RiZhi;
using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Extensions;
using ModernWpf.Controls;

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

            var context = new MainWindowViewModel(_files, _requests, _settingsManager, _player, _log);
            this.DataContext = context;

            // Decide to show welcome window
            context.ShowWelcomeWindow();
            context.ShowMissingReplayFoldersMessageBox();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var values = _settingsManager.TemporaryValues;

            if (values.TryGetDouble("WindowHeight", out double savedHeight) &&
                values.TryGetDouble("WindowWidth", out double savedWidth) &&
                values.TryGetDouble("WindowLeft", out double savedLeft) &&
                values.TryGetDouble("WindowTop", out double savedTop) &&
                values.TryGetBool("WindowMaximized", out bool savedMaximized))
            {
                this.Height = savedHeight;
                this.Width = savedWidth;
                this.Left = savedLeft;
                this.Top = savedTop;
                if (savedMaximized)
                {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

        private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            await context.ReloadReplayList().ConfigureAwait(true);
        }

        private async void ReplayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is System.Windows.Controls.ListView replayList)) { return; }
            if (!(replayList.SelectedItem is ReplayPreview previewModel)) { return; }

            // Deselect the last selected item
            if (_lastSelection != null && _lastSelection.IsSelected) _lastSelection.IsSelected = false;

            previewModel.IsSelected = true;
            _lastSelection = previewModel;

            FileResult replayFile = context.FileResults[previewModel.Location];

            ReplayDetail replayDetail = new ReplayDetail(replayFile, previewModel);

            ReplayDetailControl detailControl = this.FindName("DetailView") as ReplayDetailControl;
            detailControl.DataContext = replayDetail;

            (detailControl.FindName("BlankContent") as StackPanel).Visibility = Visibility.Hidden;
            (detailControl.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

            await (this.DataContext as MainWindowViewModel).LoadItemThumbnails(replayDetail).ConfigureAwait(true);
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is Button sortButton)) { return; }

            // Get the button and menu
            ContextMenu contextMenu = sortButton.ContextMenu;
            // Set placement and open
            contextMenu.PlacementTarget = sortButton;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;

            var name = Enum.GetName(context.SortParameters.SortMethod.GetType(), context.SortParameters.SortMethod);
            if (this.FindName(name) is RadioMenuItem selectItem)
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
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is RadioMenuItem selectedItem)) { return; }

            if (Enum.TryParse(selectedItem.Name, out SortMethod selectSort))
            {
                context.SortParameters.SortMethod = selectSort;
            }

            await context.ReloadReplayList().ConfigureAwait(false);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            await context.ShowSettingsDialog().ConfigureAwait(true);
        }

        /// <summary>
        /// Display or hide LoadMoreButton if scrolled to the bottom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplayListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel)) { return; }

            // If we scrolled at all...
            if (Math.Abs(e.VerticalChange) > 0)
            {
                // If we reached the end, show the button!!!
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    ReplayPageBar.Visibility = Visibility.Visible;
                }
                else // Hide the button
                {
                    ReplayPageBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Handler for LoadMoreButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            if (context.LoadReplays() == 0)
            {
                // Create and show flyout above the button
                var flyout = FlyoutHelper.CreateFlyout(includeButton: false, includeCustom: false);
                flyout.SetFlyoutLabelText(TryFindResource("NoReplaysFoundTitle") as string);

                flyout.ShowAt(LoadMoreButton);

                return;
            }

            // Hide the button bar once we've loaded more
            ReplayPageBar.Visibility = Visibility.Collapsed;
            await context.LoadPreviewPlayerThumbnails().ConfigureAwait(true);
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            RefreshButton.IsEnabled = false;
            context.ValidateReplayStorage();
            await context.ReloadReplayList().ConfigureAwait(true);
            RefreshButton.IsEnabled = true;
        }

        private async void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (e.Key != System.Windows.Input.Key.Enter) { return; }
            if (!(sender is AutoSuggestBox searchBox)) { return; }

            context.SortParameters.SearchTerm = searchBox.Text;

            context.ClearReplays();
            context.LoadReplays();
            await context.LoadPreviewPlayerThumbnails().ConfigureAwait(true);
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (string.IsNullOrEmpty(args.QueryText)) { return; }

            context.SortParameters.SearchTerm = args.QueryText;

            context.ClearReplays();
            context.LoadReplays();
            await context.LoadPreviewPlayerThumbnails().ConfigureAwait(true);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            context.ClearDeletedReplays();

            _settingsManager.TemporaryValues["WindowHeight"] = this.Height;
            _settingsManager.TemporaryValues["WindowWidth"] = this.Width;
            _settingsManager.TemporaryValues["WindowLeft"] = this.Left;
            _settingsManager.TemporaryValues["WindowTop"] = this.Top;
            _settingsManager.TemporaryValues["WindowMaximized"] = (this.WindowState == WindowState.Maximized);

            _settingsManager.SaveTemporaryValues();
        }
        
        private async void Window_Closed(object sender, EventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            await context.ClearImageCache().ConfigureAwait(true);
        }

        public void SelectReplayItem(ReplayPreview replay)
        {
            ReplayListView.SelectedItem = replay;
        }

        
    }
}
