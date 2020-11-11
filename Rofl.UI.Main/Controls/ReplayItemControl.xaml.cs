using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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
            if (!(Window.GetWindow(this) is MainWindow mainWindow)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }
            if (!(sender is Button moreButton)) { return; }

            // Select the item
            mainWindow.SelectReplayItem(replay);

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

        private void RenameReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            var flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: true);
            flyout.GetFlyoutLabel().Visibility = Visibility.Collapsed;
            flyout.SetFlyoutButtonText(TryFindResource("RenameReplayFile") as String);

            // Create textbox to add as flyout custom element
            var fileNameBox = new TextBox
            {
                Text = replay.Name,
                MinWidth = 200
            };
            Grid.SetColumn(fileNameBox, 0);
            Grid.SetRow(fileNameBox, 1);
            (flyout.Content as Grid).Children.Add(fileNameBox);

            // Handle save button
            flyout.GetFlyoutButton().Click += async (object eSender, RoutedEventArgs eConfirm) =>
            {
                // Rename the file and see if an error was returned
                var error = await context.RenameFile(replay, fileNameBox.Text).ConfigureAwait(false);
                
                if (error != null)
                {
                    // Display the error using the label
                    flyout.SetFlyoutLabelText(error.Replace('\n', ' '));
                    flyout.GetFlyoutLabel().Visibility = Visibility.Visible;
                    flyout.GetFlyoutLabel().Foreground = TryFindResource("SystemControlErrorTextForegroundBrush") as Brush;
                    fileNameBox.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    // Hide the flyout
                    this.Dispatcher.Invoke(() =>
                    {
                        flyout.Hide();
                    });
                }
            };

            // Show the flyout and focus it
            flyout.ShowAt(FilenameText);
            fileNameBox.Focus();
        }

        private void FileNameBox_KeyUp(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void DeleteReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            await context.DeleteReplayFile(replay).ConfigureAwait(false);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            replay.IsHovered = true;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(this.DataContext is ReplayPreview replay)) { return; }

            replay.IsHovered = false;
        }
    }
}
