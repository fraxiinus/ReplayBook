using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ReplayDetailView.xaml
    /// </summary>
    public partial class ReplayDetailControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReplayDetailControl()
        {
            InitializeComponent();
        }

        private Thickness _statsPlayerIconMargin;
        public Thickness StatsPlayerIconMargin
        {
            get => _statsPlayerIconMargin;
            set
            {
                _statsPlayerIconMargin = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(StatsPlayerIconMargin)));
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            context.PlayReplay(replay.PreviewModel);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is TabControl)) { return; }

            StatsScrollViewer.ScrollToVerticalOffset(0);
        }

        private void StatsScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            StatsScrollViewer.ScrollToVerticalOffset(0);
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button moreButton)) { return; }

            // Get the button and menu
            ContextMenu contextMenu = moreButton.ContextMenu;
            // Set placement and open
            contextMenu.PlacementTarget = moreButton;
            contextMenu.Placement = PlacementMode.Left;
            contextMenu.IsOpen = true;
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            context.OpenReplayContainingFolder(replay.PreviewModel.Location);
        }

        private void ViewOnlineMatchHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            context.ViewOnlineMatchHistory(replay.PreviewModel.MatchId);
        }

        private void PlayerIconsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is ListView iconList)) return;

            var currentStatsControlWidth = StatsControl.GetCombinedColumnWidth();
            var iconCount = iconList.Items.Count;

            // Calculate margin needed on both sides, round down
            var marginSize = (int) ((currentStatsControlWidth - (50d * iconCount)) / (2 * iconCount));

            StatsPlayerIconMargin = new Thickness(marginSize, 0, marginSize, 0);
        }
    }
}
