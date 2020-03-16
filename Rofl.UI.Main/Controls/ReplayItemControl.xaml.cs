using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this).DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreviewModel replay)) { return; }

            context.PlayReplay(replay);
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this).DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreviewModel replay)) { return; }
            context.OpenReplayContainingFolder(replay.Location);
        }

        private void Morebutton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button moreButton)) { return; }

            // Get the button and menu
            ContextMenu contextMenu = moreButton.ContextMenu;
            // Set placement and open
            contextMenu.PlacementTarget = moreButton;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }

        private void ViewOnlineMatchHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this).DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreviewModel replay)) { return; }
            context.ViewOnlineMatchHistory(replay.MatchId);
        }
    }
}
