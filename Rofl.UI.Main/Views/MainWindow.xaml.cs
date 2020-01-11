using Microsoft.Extensions.Configuration;
using Rofl.Executables;
using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Logger;
using Rofl.Reader;
using Rofl.Reader.Models;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            //_config = new ConfigurationBuilder()
            //    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .Build();
            _log = new Scribe();

            _settingsManager = new SettingsManager(_log);

            _files = new FileManager(_settingsManager.Settings, _log);
            _requests = new RequestManager(_settingsManager.Settings, _log);

            _log.Error("lol", "ALPHA DEBUG");

            this.DataContext = new MainWindowViewModel(_files, _requests, _settingsManager);
        }

        private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
        {
            // configure await it set to true, otherwise we get some errors...
            await (DataContext as MainWindowViewModel).LoadReplays().ConfigureAwait(true);
            await (DataContext as MainWindowViewModel).LoadPreviewPlayerThumbnails().ConfigureAwait(true);
        }

        private void PreviewReplaysView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (this.DataContext as MainWindowViewModel).FilterPreviewReplay(e.Item as ReplayPreviewModel);
        }

        private async void ReplayItemControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is ReplayItemControl replayItem)) { return; }

            ReplayPreviewModel previewModel = replayItem.DataContext as ReplayPreviewModel;

            FileResult replayFile = context.FileResults[previewModel.MatchId];

            ReplayDetailModel replayDetailModel = new ReplayDetailModel(replayFile, previewModel);

            ReplayDetailControl detailControl = this.FindName("DetailView") as ReplayDetailControl;
            detailControl.DataContext = replayDetailModel;

            (detailControl.FindName("BlankContent") as StackPanel).Visibility = Visibility.Hidden;
            (detailControl.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;


            await (this.DataContext as MainWindowViewModel).LoadItemThumbnails(replayDetailModel).ConfigureAwait(true);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            if (_typingTimer == null)
            {
                _typingTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000)
                };

                _typingTimer.Tick += new EventHandler(this.TypingTimer_Timeout);
            }

            _typingTimer.Stop();
            _typingTimer.Tag = (sender as TextBox).Text;
            _typingTimer.Start();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
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

            context.SortPreviewReplays(this.FindResource("PreviewReplaysView") as CollectionViewSource);
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            await context.ShowSettingsDialog().ConfigureAwait(true);
        }

        private void TypingTimer_Timeout(object sender, EventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is DispatcherTimer timer)) { return; }

            string searchText = timer.Tag.ToString();

            context.SortParameters.SearchTerm = searchText;

            (this.FindResource("PreviewReplaysView") as CollectionViewSource).View.Refresh();

            timer.Stop();
        }
    }
}
