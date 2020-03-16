using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Logger;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private DispatcherTimer _typingTimer;

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

            this.DataContext = new MainWindowViewModel(_files, _requests, _settingsManager);

            _log.Error("PRERELEASE", "Log files are generated for each run while in prerelease");
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
            if (!(replayList.SelectedItem is ReplayPreviewModel previewModel)) { return; }

            FileResult replayFile = context.FileResults[previewModel.Location];

            ReplayDetailModel replayDetailModel = new ReplayDetailModel(replayFile, previewModel);

            ReplayDetailControl detailControl = this.FindName("DetailView") as ReplayDetailControl;
            detailControl.DataContext = replayDetailModel;

            (detailControl.FindName("BlankContent") as StackPanel).Visibility = Visibility.Hidden;
            (detailControl.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

            await (this.DataContext as MainWindowViewModel).LoadItemThumbnails(replayDetailModel).ConfigureAwait(true);
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
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

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

            var lastItem = context.PreviewReplays.Last();

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
    }
}
