using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.UI.Main.Models.View;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;
using System;
using System.Diagnostics;
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

        private ReplayPreview Context
        {
            get => (DataContext is ReplayPreview context)
                ? context
                : null;
        }

        private MainWindowViewModel ViewModel
        {
            get => (Window.GetWindow(this)?.DataContext is MainWindowViewModel viewModel)
                ? viewModel
                : null;
        }

        public ReplayItemControl()
        {
            InitializeComponent();
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Context != null)
            {
                _ = await ViewModel.PlayReplay(Context).ConfigureAwait(true);
            }
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (Context == null) { return; }
            if (sender is not Button moreButton) { return; }

            // Get the button and menu
            ContextMenu contextMenu = moreButton.ContextMenu;

            // Hide static data option if item is error or is already downloaded
            if (Context.IsErrorReplay == true)
            {
                DownloadStaticData_MenuItem__2.Visibility = Visibility.Collapsed;
                ExportReplayData_MenuItem__2.Visibility = Visibility.Collapsed;
            }
            else if (ViewModel.StaticDataManager.DoesBundleExist(Context.GameVersion))
            {
                DownloadStaticData_MenuItem__2.Visibility = Visibility.Collapsed;
            }
            else
            {
                DownloadStaticData_MenuItem__2.Header = (DownloadStaticData_MenuItem__2.Header as string)
                    .Replace("$", Context.GameVersionString);
            }

            // Set placement and open
            contextMenu.PlacementTarget = moreButton;
            contextMenu.IsOpen = true;
        }

        private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DockPanelReplayContextMenu.Placement = PlacementMode.MousePoint;
            DockPanelReplayContextMenu.IsOpen = true;
        }

        private void DockPanelReplayContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (Context == null) { return; }

            // Hide static data option if item is error or is already downloaded
            if (Context.IsErrorReplay == true)
            {
                DownloadStaticData_MenuItem__1.Visibility = Visibility.Collapsed;
                ExportReplayData_MenuItem__1.Visibility = Visibility.Collapsed;
            }
            else if (ViewModel.StaticDataManager.DoesBundleExist(Context.GameVersion))
            {
                DownloadStaticData_MenuItem__1.Visibility = Visibility.Collapsed;
            }
            else
            {
                DownloadStaticData_MenuItem__1.Header = (DownloadStaticData_MenuItem__1.Header as string)
                    .Replace("$", Context.GameVersionString);
            }
        }

        private void OpenNewWindow_Click(object sender, RoutedEventArgs e)
        {
            if (Context != null)
            {
                ViewModel.OpenNewWindow(Context.Location);
            }
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (Context != null)
            {
                ViewModel.OpenReplayContainingFolder(Context.Location);
            }
        }

        private void ExportReplayData_OnClick(object sender, RoutedEventArgs e)
        {
            if (Context != null)
            {
                ViewModel.ShowExportReplayDataWindow(Context);
            }
        }

        private void RenameReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (Context == null) { return; }

            var flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: true);
            flyout.GetFlyoutLabel().Visibility = Visibility.Collapsed;
            flyout.SetFlyoutButtonText(TryFindResource("RenameReplayFile") as string);

            // Create textbox to add as flyout custom element
            var fileNameBox = new TextBox
            {
                Text = Context.DisplayName,
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
                string error = ViewModel.RenameFile(Context, fileNameBox.Text);

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

        private void DeleteReplayFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (Context == null) { return; }

            // create the flyout
            var flyout = FlyoutHelper.CreateFlyout(includeButton: true, includeCustom: false);

            // set the flyout texts
            flyout.SetFlyoutButtonText(TryFindResource("DeleteReplayFile") as string);
            flyout.SetFlyoutLabelText(TryFindResource("DeleteFlyoutLabel") as string);

            // set button click function
            flyout.GetFlyoutButton().Click += async (object eSender, RoutedEventArgs eConfirm) =>
            {
                await ViewModel.DeleteReplayFile(Context).ConfigureAwait(false);

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

        private async void DownloadStaticData_Click(object sender, RoutedEventArgs e)
        {
            if (Context == null) { return; }

            var language = ViewModel.Configuration.UseCurrentLanguageAsLocale
                    ? ConfigurationDefinitions.GetRiotRegionCode(LanguageHelper.CurrentLanguage)
                    : ConfigurationDefinitions.GetRiotRegionCode(ViewModel.Configuration.StaticDataDownloadLanguage);

            await StaticDataDownloadDialog.StartDownloadDialog(Context.GameVersion, language);

            await ViewModel.LoadSinglePreviewPlayerThumbnails(Context);
        }

        private async void RefreshReplayList_Click(object sender, RoutedEventArgs e)
        {
            if (Context != null)
            {
                await ViewModel.ReloadReplayList(true).ConfigureAwait(true);
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Context != null)
            {
                Context.IsHovered = true;
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Context != null)
            {
                Context.IsHovered = false;
            }
        }

        private async void DisabledButtonHandler_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Context.IsSupported)
            {
                var dialog = ContentDialogHelper.CreateContentDialog(
                    title: TryFindResource("ExecutableNotFoundErrorTitle") as string,
                    description: TryFindResource("ExecutableNotFoundDescriptionText") as string,
                    primaryButtonText: TryFindResource("OKButtonText") as string,
                    secondaryButtonText: TryFindResource("HelpButtonText") as string);
                
                if (await dialog.ShowAsync() == ModernWpf.Controls.ContentDialogResult.Secondary)
                {
                    var url = (TryFindResource("Help_PlayingExpiredReplays") as Uri).ToString();
                    _ = Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                }
            }
        }
    }
}
