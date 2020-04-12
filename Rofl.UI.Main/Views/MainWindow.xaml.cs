using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Logger;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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
        private readonly Scribe _log;

        public MainWindow()
        {
            InitializeComponent();

            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) =>
            {
                _log.Error("RootHandler", e.Exception.ToString());
                _log.WriteToFile();
            };

            _log = new Scribe();

            _settingsManager = new SettingsManager(_log);

            _files = new FileManager(_settingsManager.Settings, _log);
            _requests = new RequestManager(_settingsManager.Settings, _log);

            var context = new MainWindowViewModel(_files, _requests, _settingsManager, _log);
            this.DataContext = context;

            _log.Error("PRERELEASE", "Log files are generated for each run while in prerelease");

            // Decide to show welcome window
            context.ShowWelcomeWindow();
        }

        private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            await context.ReloadReplayList().ConfigureAwait(true);
        }

        private async void ReplayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is ListView replayList)) { return; }
            if (!(replayList.SelectedItem is ReplayPreview previewModel)) { return; }

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
            if (this.FindName(name) is MenuItem selectItem)
            {
                // Select our item
                selectItem.BorderBrush = SystemColors.HighlightBrush;
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
            if (!(sender is MenuItem selectedItem)) { return; }

            if (Enum.TryParse(selectedItem.Name, out SortMethod selectSort))
            {
                context.SortParameters.SortMethod = selectSort;
            }

            // Clear all selections
            foreach (Object item in (this.FindName("SortMenu") as ContextMenu).Items)
            {
                if (item is MenuItem menuItem)
                {
                    menuItem.BorderBrush = Brushes.Transparent;
                }
            }

            await context.ReloadReplayList().ConfigureAwait(true);
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

            if (Math.Abs(e.VerticalChange) > 0)
            {
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    ReplayPageBar.Visibility = Visibility.Visible;
                }
                else
                {
                    ReplayPageBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            context.LoadReplays();
            await context.LoadPreviewPlayerThumbnails().ConfigureAwait(true);
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            await context.ReloadReplayList().ConfigureAwait(true);
        }

        private async void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (e.Key != System.Windows.Input.Key.Enter) { return; }
            if (!(sender is TextBox searchBox)) { return; }

            context.SortParameters.SearchTerm = searchBox.Text;

            context.ClearReplays();
            context.LoadReplays();
            await context.LoadPreviewPlayerThumbnails().ConfigureAwait(true);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _log.WriteToFile();
        }

        //private void ReplayItemControl_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (!(this.DataContext is MainWindowViewModel context)) { return; }

        //    // if(!(sender is ReplayItemControl replayItem)) { return; }
        //    // Get the button and menu
        //    ReplayContextMenu
        //    // ContextMenu contextMenu = ReplayContextMenu;
        //    // Set placement and open
        //    contextMenu.PlacementTarget = moreButton;
        //    contextMenu.Placement = PlacementMode.Bottom;
        //    contextMenu.IsOpen = true;

        //    MessageBox.Show($"{ReplayListView.SelectedItems.Count}");
        //}

        //private void ReplayMenuOpenContainingFolder_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!(this.DataContext is MainWindowViewModel context)) { return; }
        //    if (!(sender is ReplayPreview replay)) { return; }

        //    context.OpenReplayContainingFolder(replay.Location);
        //}

        //public void ReplayMenuViewOnlineMatchHistory_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //public void ReplayMenuExportReplayData_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
