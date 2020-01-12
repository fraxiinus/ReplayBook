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
            context.OpenReplayContainingFolder(replay.MatchId);
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
    }
}
