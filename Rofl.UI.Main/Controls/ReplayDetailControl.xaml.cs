using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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

        private void ReplayDetailControlElement_Loaded(object sender, RoutedEventArgs e)
        {
            // Disable delete menu option in single replay mode,
            if (Window.GetWindow(this) is SingleReplayWindow)
            {
                DeleteReplayFile.Visibility = Visibility.Collapsed;
            }
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

        private void ReplayDetailControlElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            PlayerIconsGrid.Children.Clear();
            PlayerIconsGrid.ColumnDefinitions.Clear();

            if (DetailTabControl.SelectedIndex == 1)
            {
                int counter = 0;
                foreach (var player in replay.AllPlayers)
                {
                    // Add a column
                    var newColumn = new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    };
                    PlayerIconsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    var newImage = new PlayerIconControl
                    {
                        DataContext = player.PreviewModel,
                        Width = 50,
                        Height = 50
                    };
                    Grid.SetRow(newImage, 0);
                    Grid.SetColumn(newImage, counter);
                    PlayerIconsGrid.Children.Add(newImage);

                    counter++;
                }
            }
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            await context.PlayReplay(replay.PreviewModel).ConfigureAwait(true);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is TabControl context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            StatsScrollViewer.ScrollToVerticalOffset(0);

            if (context.SelectedIndex == 1 && PlayerIconsGrid.ColumnDefinitions.Count < 1)
            {
                int counter = 0;
                foreach (var player in replay.AllPlayers)
                {
                    // Add a column
                    var newColumn = new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    };
                    PlayerIconsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    var newImage = new PlayerIconControl
                    {
                        DataContext = player.PreviewModel,
                        Width = 50,
                        Height = 50
                    };
                    Grid.SetRow(newImage, 0);
                    Grid.SetColumn(newImage, counter);
                    PlayerIconsGrid.Children.Add(newImage);

                    counter++;
                }
            }
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
            contextMenu.IsOpen = true;
        }

        #region Context menu item handlers
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

        private void ExportReplayData_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            context.ShowExportReplayDataWindow(replay.PreviewModel);
        }

        private void RenameReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            var flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: true);
            flyout.GetFlyoutLabel().Visibility = Visibility.Collapsed;
            flyout.SetFlyoutButtonText(TryFindResource("RenameReplayFile") as string);

            // Create textbox to add as flyout custom element
            var fileNameBox = new TextBox
            {
                Text = replay.PreviewModel.Name,
                MinWidth = 200
            };
            Grid.SetColumn(fileNameBox, 0);
            Grid.SetRow(fileNameBox, 1);
            (flyout.Content as Grid).Children.Add(fileNameBox);

            // Handle save button
            flyout.GetFlyoutButton().Click += async (object eSender, RoutedEventArgs eConfirm) =>
            {
                // Rename the file and see if an error was returned
                var error = await context.RenameFile(replay.PreviewModel, fileNameBox.Text).ConfigureAwait(false);

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
            flyout.ShowAt(ReplayFileName);
            fileNameBox.Focus();
        }
        private void DeleteReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            // create the flyout
            var flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: false);

            // set the flyout texts
            flyout.SetFlyoutButtonText(TryFindResource("DeleteReplayFile") as string);
            flyout.SetFlyoutLabelText(TryFindResource("DeleteFlyoutLabel") as string);

            // set button click function
            flyout.GetFlyoutButton().Click += async (object eSender, RoutedEventArgs eConfirm) =>
            {
                await context.DeleteReplayFile(replay.PreviewModel).ConfigureAwait(false);

                // Hide the flyout
                this.Dispatcher.Invoke(() =>
                {
                    flyout.Hide();
                });
            };

            // Show the flyout and focus it
            flyout.ShowAt(ReplayFileName);
            flyout.GetFlyoutButton().Focus();
        }
        #endregion
    }
}
