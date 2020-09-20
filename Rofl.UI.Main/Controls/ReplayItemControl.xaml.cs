using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ReplayItem.xaml
    /// </summary>
    public partial class ReplayItemControl : UserControl
    {

        public ReplayItemControl()
        {
            InitializeComponent();
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            await context.PlayReplay(replay).ConfigureAwait(true);
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }
            context.OpenReplayContainingFolder(replay.Location);
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button moreButton)) { return; }

            // Get the button and menu
            ContextMenu contextMenu = moreButton.ContextMenu;
            // Set placement and open
            contextMenu.PlacementTarget = moreButton;
            contextMenu.IsOpen = true;
        }

        private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DockPanelReplayContextMenu.Placement = PlacementMode.MousePoint;
            DockPanelReplayContextMenu.IsOpen = true;
        }

        private void ViewOnlineMatchHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }
            context.ViewOnlineMatchHistory(replay.MatchId);
        }

        private void ExportReplayData_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            context.ShowExportReplayDataWindow(replay);
        }

        private async void RenameReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            await context.ShowRenameDialog(replay).ConfigureAwait(false);
        }

        private async void DeleteReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            await context.DeleteReplayFile(replay).ConfigureAwait(false);
        }
    }
}
