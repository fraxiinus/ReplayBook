using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Controls
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
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayPreview replay) { return; }

            _ = await context.PlayReplay(replay).ConfigureAwait(true);
        }


        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is not MainWindow mainWindow) { return; }
            if (DataContext is not ReplayPreview replay) { return; }
            if (sender is not Button moreButton) { return; }

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

        private void OpenNewWindow_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayPreview replay) { return; }
            context.OpenNewWindow(replay.Location);
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayPreview replay) { return; }
            context.OpenReplayContainingFolder(replay.Location);
        }

        private void ExportReplayData_OnClick(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayPreview replay) { return; }

            context.ShowExportReplayDataWindow(replay);
        }

        private void RenameReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayPreview replay) { return; }

            ModernWpf.Controls.Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: true);
            flyout.GetFlyoutLabel().Visibility = Visibility.Collapsed;
            flyout.SetFlyoutButtonText(TryFindResource("RenameReplayFile") as string);

            // Create textbox to add as flyout custom element
            var fileNameBox = new TextBox
            {
                Text = replay.DisplayName,
                IsReadOnly = false,
                MinWidth = 200
            };
            Grid.SetColumn(fileNameBox, 0);
            Grid.SetRow(fileNameBox, 1);
            _ = (flyout.Content as Grid).Children.Add(fileNameBox);

            // Handle save button
            flyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                // Rename the file and see if an error was returned
                string error = context.RenameFile(replay, fileNameBox.Text);

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
                    Dispatcher.Invoke(() =>
                    {
                        flyout.Hide();
                    });
                }
            };

            // Show the flyout and focus it
            flyout.ShowAt(FilenameText);
            fileNameBox.SelectAll();
            _ = fileNameBox.Focus();
        }

        private void FileNameBox_KeyUp(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayPreview replay) { return; }

            // create the flyout
            ModernWpf.Controls.Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: false);

            // set the flyout texts
            flyout.SetFlyoutButtonText(TryFindResource("DeleteReplayFile") as string);
            flyout.SetFlyoutLabelText(TryFindResource("DeleteFlyoutLabel") as string);

            // set button click function
            flyout.GetFlyoutButton().Click += async (object eSender, RoutedEventArgs eConfirm) =>
            {
                await context.DeleteReplayFile(replay).ConfigureAwait(false);

                // Hide the flyout
                Dispatcher.Invoke(() =>
                {
                    flyout.Hide();
                });
            };

            // Show the flyout and focus it
            flyout.ShowAt(FilenameText);
            _ = flyout.GetFlyoutButton().Focus();
        }

        private async void RefreshReplayList_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }

            await context.ReloadReplayList(true).ConfigureAwait(true);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (DataContext is not ReplayPreview replay) { return; }

            replay.IsHovered = true;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (DataContext is not ReplayPreview replay) { return; }

            replay.IsHovered = false;
        }
    }
}
