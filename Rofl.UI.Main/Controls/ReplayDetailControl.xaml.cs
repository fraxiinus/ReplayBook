using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ReplayDetailView.xaml
    /// </summary>
    public partial class ReplayDetailControl : UserControl
    {
        public ReplayDetailControl()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this).DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetailModel replay)) { return; }

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
            if (!(Window.GetWindow(this).DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetailModel replay)) { return; }

            context.OpenReplayContainingFolder(replay.PreviewModel.Location);
        }

        private void ViewOnlineMatchHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this).DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetailModel replay)) { return; }

            context.ViewOnlineMatchHistory(replay.PreviewModel.MatchId);
        }
    }
}
