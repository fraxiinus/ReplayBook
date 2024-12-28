using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;
using ModernWpf.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Controls
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
            // Disable some menu options in single replay mode,
            if (Window.GetWindow(this) is SingleReplayWindow)
            {
                DeleteReplayFile.Visibility = Visibility.Collapsed;
                OpenNewWindow.Visibility = Visibility.Collapsed;
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
            if (DataContext is not ReplayDetail replay) { return; }
            if (replay.ErrorInfo != default)
            {
                ErrorContent__TextBox.Text = replay.ErrorInfo.ExceptionString;
                return;
            }

            PlayerIconsGrid.Children.Clear();
            PlayerIconsGrid.ColumnDefinitions.Clear();

            if (DetailTabControl.SelectedIndex == 2)
            {
                int counter = 0;
                foreach (PlayerDetail player in replay.AllPlayers)
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
                    _ = PlayerIconsGrid.Children.Add(newImage);

                    counter++;
                }
            }
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayDetail replay) { return; }

            _ = await context.PlayReplay(replay.PreviewModel).ConfigureAwait(true);
        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (sender is not TabControl tabControl) { return; }
            if (DataContext is not ReplayDetail replay) { return; }

            StatsScrollViewer.ScrollToVerticalOffset(0);

            if (tabControl.SelectedIndex == 1)
            {
                await context.LoadRuneThumbnails(replay);
            }

            if (tabControl.SelectedIndex == 2 && PlayerIconsGrid.ColumnDefinitions.Count < 1)
            {
                int counter = 0;
                foreach (PlayerDetail player in replay.AllPlayers)
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
                    _ = PlayerIconsGrid.Children.Add(newImage);

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
            if (sender is not Button moreButton) { return; }

            // Get the button and menu
            ContextMenu contextMenu = moreButton.ContextMenu;

            // Set placement and open
            contextMenu.PlacementTarget = moreButton;
            contextMenu.IsOpen = true;
        }

        private void ReplayFileName_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RenameReplayFile_OnClick(null, null);
        }

        private void ErrorContent__HelpButton_Click(object sender, RoutedEventArgs e)
        {
            var url = (TryFindResource("Help_ErrorLoadingReplay") as Uri).ToString();
            _ = Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }

        private async void ErrorContent__ClearCacheButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }

            context.ClearReplayCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            var dialog = ContentDialogHelper.CreateContentDialog(
                title: TryFindResource("RequestsCacheCloseToDeleteTitle") as string,
                description: TryFindResource("RequestsCacheCloseToDelete") as string,
                primaryButtonText: TryFindResource("Settings__Replays__ClearCacheRestartNow__Button") as string,
                secondaryButtonText: TryFindResource("CancelButtonText") as string);

            var confirmResult = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            if (confirmResult == ContentDialogResult.Primary)
            {
                context.RestartOnClose = true;
                Application.Current.MainWindow.Close();
            }
        }

        #region Context menu item handlers
        private void OpenNewWindow_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayDetail replay) { return; }

            context.OpenNewWindow(replay.PreviewModel.Location);
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayDetail replay) { return; }
            context.OpenReplayContainingFolder(replay.PreviewModel.Location);
        }

        private void ExportReplayData_OnClick(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayDetail replay) { return; }

            context.ShowExportReplayDataWindow(replay.PreviewModel);
        }

        private void RenameReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayDetail replay) { return; }

            ModernWpf.Controls.Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: true);
            flyout.GetFlyoutLabel().Visibility = Visibility.Collapsed;
            flyout.SetFlyoutButtonText(TryFindResource("RenameReplayFile") as string);

            // Create textbox to add as flyout custom element
            var fileNameBox = new TextBox
            {
                Text = replay.PreviewModel.DisplayName,
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
                string error = context.RenameFile(replay.PreviewModel, fileNameBox.Text);

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
            flyout.ShowAt(ReplayFileName);
            fileNameBox.SelectAll();
            _ = fileNameBox.Focus();
        }

        private void DeleteReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is not MainWindowViewModel context) { return; }
            if (DataContext is not ReplayDetail replay) { return; }

            // create the flyout
            ModernWpf.Controls.Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: false);

            // set the flyout texts
            flyout.SetFlyoutButtonText(TryFindResource("DeleteReplayFile") as string);
            flyout.SetFlyoutLabelText(TryFindResource("DeleteFlyoutLabel") as string);

            // set button click function
            flyout.GetFlyoutButton().Click += async (object eSender, RoutedEventArgs eConfirm) =>
            {
                await context.DeleteReplayFile(replay.PreviewModel).ConfigureAwait(false);

                // Hide the flyout
                Dispatcher.Invoke(() =>
                {
                    flyout.Hide();
                });
            };

            // Show the flyout and focus it
            flyout.ShowAt(ReplayFileName);
            _ = flyout.GetFlyoutButton().Focus();
        }
        #endregion

    }
}
