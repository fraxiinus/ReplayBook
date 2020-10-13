using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf.Controls;
using Rofl.Executables.Models;
using Rofl.Settings;
using Rofl.Settings.Models;
using Rofl.UI.Main.Utilities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            // This window should open as a dialog, so set owner
            this.Owner = App.Current.MainWindow;
            SettingsMenuListBox.SelectedIndex = 0;
        }

        private void SettingsWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager)) { return; }

            // Change window style
            var GWL_STYLE = -16;
            // Maximize box flag
            var WS_MAXIMIZEBOX = 0x10000;

            var windowHandle = new WindowInteropHelper((Window)sender).Handle;
            var value = NativeMethods.GetWindowLong(windowHandle, GWL_STYLE);

            // Flip maximize box flag
            _ = NativeMethods.SetWindowLong(windowHandle, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            context.SaveConfigFile();
            this.DialogResult = true;
        }

        private void SettingsMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((sender as ListBox).SelectedItem as ListBoxItem);

            switch (selectedItem.Name)
            {
                case "GeneralSettingsListItem":
                    SettingsTabControl.SelectedIndex = 0;
                    break;
                case "ExecutablesSettingsListItem":
                    SettingsTabControl.SelectedIndex = 1;
                    break;
                case "ReplaySettingsListItem":
                    SettingsTabControl.SelectedIndex = 2;
                    break;
                case "RequestSettingsListItem":
                    SettingsTabControl.SelectedIndex = 3;
                    break;
                case "AboutSettingsListItem":
                    SettingsTabControl.SelectedIndex = 4;
                    break;
                default:
                    break;
            }
        }

        private void AddKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            var addDialog = new PlayerMarkerDialog
            {
                Owner = this,
                DataContext = context.Settings.KnownPlayers
            };

            addDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void EditKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            var editDialog = new PlayerMarkerDialog(selectedPlayer)
            {
                Owner = this,
                DataContext = context.Settings.KnownPlayers
            };

            editDialog.ShowAsync(ContentDialogPlacement.Popup);
        }
        
        private void RemoveKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            // Create button that does the actual delete
            var confirmButton = new Button
            {
                Content = TryFindResource("YesText") as String
            };

            // Add items to a panel
            var contentPanel = new SimpleStackPanel
            {
                Spacing = 12
            };
            contentPanel.Children.Add(new TextBlock
            {
                Text = TryFindResource("ConfirmText") as String
            });
            contentPanel.Children.Add(confirmButton);

            // Add the grid into a flyout
            var confirmFlyout = new Flyout
            {
                Content = contentPanel
            };

            // Assign button behavior
            confirmButton.Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                context.Settings.KnownPlayers.Remove(selectedPlayer);

                EditKnownPlayerButton.IsEnabled = false;
                RemoveKnownPlayerButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveKnownPlayerButton);
        }

        private void KnownPlayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem as PlayerMarker == null) { return; };

            EditKnownPlayerButton.IsEnabled = true;
            RemoveKnownPlayerButton.IsEnabled = true;
        }

        private void SourceFoldersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem as String == null) { return; };

            EditSourceFolderButton.IsEnabled = true;
            RemoveSourceFolderButton.IsEnabled = true;
        }

        private void AddSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("SourceFoldersWindowText") as String;
                folderDialog.IsFolderPicker = true;
                folderDialog.AddToMostRecentlyUsedList = false;
                folderDialog.AllowNonFileSystemItems = false;
                folderDialog.EnsureFileExists = true;
                folderDialog.EnsurePathExists = true;
                folderDialog.EnsureReadOnly = false;
                folderDialog.EnsureValidNames = true;
                folderDialog.Multiselect = false;
                folderDialog.ShowPlacesList = true;

                folderDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                folderDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if(folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var selectedFolder = folderDialog.FileName;

                    if (context.Settings.SourceFolders.Contains(selectedFolder))
                    {
                        var msgBoxResult = MessageBox.Show
                            (
                                TryFindResource("SourceFoldersAlreadyExistsErrorText") as String,
                                TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation
                            );

                        if(msgBoxResult == MessageBoxResult.OK)
                        {
                            AddSourceFolderButton_Click(null, null);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        context.Settings.SourceFolders.Add(selectedFolder);
                    }
                }
            }

        }

        private void EditSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(SourceFoldersListBox.SelectedItem is String selectedFolder)) { return; }

            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("SourceFoldersWindowText") as String;
                folderDialog.IsFolderPicker = true;
                folderDialog.AddToMostRecentlyUsedList = false;
                folderDialog.AllowNonFileSystemItems = false;
                folderDialog.EnsureFileExists = true;
                folderDialog.EnsurePathExists = true;
                folderDialog.EnsureReadOnly = false;
                folderDialog.EnsureValidNames = true;
                folderDialog.Multiselect = false;
                folderDialog.ShowPlacesList = true;

                folderDialog.InitialDirectory = selectedFolder;
                folderDialog.DefaultDirectory = selectedFolder;

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var newSelectedFolder = folderDialog.FileName;

                    if (context.Settings.SourceFolders.Contains(newSelectedFolder))
                    {
                        var msgBoxResult = MessageBox.Show
                            (
                                TryFindResource("SourceFoldersAlreadyExistsErrorText") as String,
                                TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation
                            );

                        if (msgBoxResult == MessageBoxResult.OK)
                        {
                            EditSourceFolderButton_Click(null, null);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        context.Settings.SourceFolders.Remove(selectedFolder);
                        context.Settings.SourceFolders.Add(newSelectedFolder);
                    }
                }
            }
        }

        private void RemoveSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(SourceFoldersListBox.SelectedItem is String selectedFolder)) { return; }

            // Create button that does the actual delete
            var confirmButton = new Button
            {
                Content = TryFindResource("YesText") as String
            };

            // Add items to a panel
            var contentPanel = new SimpleStackPanel
            {
                Spacing = 12
            };
            contentPanel.Children.Add(new TextBlock
            {
                Text = TryFindResource("ConfirmText") as String
            });
            contentPanel.Children.Add(confirmButton);

            // Add the grid into a flyout
            var confirmFlyout = new Flyout
            {
                Content = contentPanel
            };

            // Assign button behavior
            confirmButton.Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                context.Settings.SourceFolders.Remove(selectedFolder);

                EditSourceFolderButton.IsEnabled = false;
                RemoveSourceFolderButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveSourceFolderButton);
        }

        private void ExecutableFoldersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem as string == null) { return; };

            EditExecutableFolderButton.IsEnabled = true;
            RemoveExecutableFolderButton.IsEnabled = true;
        }

        private void AddExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("ExecutableSelectFolderDialogText") as String;
                folderDialog.IsFolderPicker = true;
                folderDialog.AddToMostRecentlyUsedList = false;
                folderDialog.AllowNonFileSystemItems = false;
                folderDialog.EnsureFileExists = true;
                folderDialog.EnsurePathExists = true;
                folderDialog.EnsureReadOnly = false;
                folderDialog.EnsureValidNames = true;
                folderDialog.Multiselect = false;
                folderDialog.ShowPlacesList = true;

                folderDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                folderDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var selectedFolder = folderDialog.FileName;

                    if (context.Executables.Settings.SourceFolders.Contains(selectedFolder))
                    {
                        var msgBoxResult = MessageBox.Show
                            (
                                TryFindResource("SourceFoldersAlreadyExistsErrorText") as String,
                                TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation
                            );
                        if (msgBoxResult == MessageBoxResult.OK)
                        {
                            AddExecutableFolderButton_Click(null, null);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        context.Executables.Settings.SourceFolders.Add(selectedFolder);
                    }
                }
            }
        }

        private void EditExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(ExecutableFoldersListBox.SelectedItem is String selectedFolder)) { return; }

            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("ExecutableSelectFolderDialogText") as String;
                folderDialog.IsFolderPicker = true;
                folderDialog.AddToMostRecentlyUsedList = false;
                folderDialog.AllowNonFileSystemItems = false;
                folderDialog.EnsureFileExists = true;
                folderDialog.EnsurePathExists = true;
                folderDialog.EnsureReadOnly = false;
                folderDialog.EnsureValidNames = true;
                folderDialog.Multiselect = false;
                folderDialog.ShowPlacesList = true;

                folderDialog.InitialDirectory = selectedFolder;
                folderDialog.DefaultDirectory = selectedFolder;

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var newSelectedFolder = folderDialog.FileName;

                    if (context.Executables.Settings.SourceFolders.Contains(newSelectedFolder))
                    {
                        var msgBoxResult = MessageBox.Show
                            (
                                TryFindResource("SourceFoldersAlreadyExistsErrorText") as String,
                                TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation
                            );

                        if (msgBoxResult == MessageBoxResult.OK)
                        {
                            EditExecutableFolderButton_Click(null, null);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        context.Executables.Settings.SourceFolders.Remove(selectedFolder);
                        context.Executables.Settings.SourceFolders.Add(newSelectedFolder);
                    }
                }
            }
        }

        private void RemoveExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(ExecutableFoldersListBox.SelectedItem is String selectedFolder)) { return; }

            // Create button that does the actual delete
            var confirmButton = new Button
            {
                Content = TryFindResource("YesText") as String
            };

            // Add items to a panel
            var contentPanel = new SimpleStackPanel
            {
                Spacing = 12
            };
            contentPanel.Children.Add(new TextBlock
            {
                Text = TryFindResource("ConfirmText") as String
            });
            contentPanel.Children.Add(confirmButton);

            // Add the grid into a flyout
            var confirmFlyout = new Flyout
            {
                Content = contentPanel
            };

            // Assign button behavior
            confirmButton.Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                context.Executables.Settings.SourceFolders.Remove(selectedFolder);

                EditExecutableFolderButton.IsEnabled = false;
                RemoveExecutableFolderButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveExecutableFolderButton);
        }

        private void SourceFoldersSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            var addedCount = context.Executables.SearchAllFoldersForExecutablesAndAddThemAll();

            var labelText = TryFindResource("ExecutableFoldersSearchResultLabelText") as String;
            labelText = labelText.Replace("$", addedCount.ToString(CultureInfo.InvariantCulture));

            MessageBox.Show
            (
                labelText,
                TryFindResource("ExecutableFoldersSearchResultTitleText") as String,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void ExecutablesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem as LeagueExecutable == null) { return; };

            EditExecutableButton.IsEnabled = true;
            RemoveExecutableButton.IsEnabled = true;
        }

        private void AddExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            var addDialog = new ExecutableDetailDialog
            {
                Owner = this,
                DataContext = context.Executables
            };

            addDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void EditExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            var editDialog = new ExecutableDetailDialog(selectedExecutable)
            {
                Owner = this,
                DataContext = context.Executables
            };

            editDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void RemoveExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            // Create button that does the actual delete
            var confirmButton = new Button
            {
                Content = TryFindResource("YesText") as String
            };

            // Add items to a panel
            var contentPanel = new SimpleStackPanel
            {
                Spacing = 12
            };
            contentPanel.Children.Add(new TextBlock
            {
                Text = TryFindResource("ConfirmText") as String
            });
            contentPanel.Children.Add(confirmButton);

            // Add the grid into a flyout
            var confirmFlyout = new Flyout
            {
                Content = contentPanel
            };

            // Assign button behavior
            confirmButton.Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                context.Executables.DeleteExecutable(selectedExecutable.Name);

                EditExecutableButton.IsEnabled = false;
                RemoveExecutableButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveExecutableButton);
        }

        private void SetFileAssocButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            FileAssociations.SetRoflToSelf();
        }

        private async void UpdateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            string latestVersion;
            try
            {
                latestVersion = await GithubConnection.GetLatestVersion().ConfigureAwait(true);
            }
            catch (HttpRequestException ex)
            {
                // Http request failed, show error and stop
                MessageBox.Show
                (
                    TryFindResource("UpdateHTTPExceptionBodyText") as String,
                    TryFindResource("UpdateExceptionTitleText") as String,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
                return;
            }
            
            if (String.IsNullOrEmpty(latestVersion))
            {
                // Either github returned nothing or got an http error code
                MessageBox.Show
                (
                    TryFindResource("UpdateGitHubErrorBodyText") as String,
                    TryFindResource("UpdateExceptionTitleText") as String,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
                return;
            }

            var assemblyName = Assembly.GetEntryAssembly()?.GetName();
            var assemblyVersion = assemblyName.Version.ToString(3);

            if (latestVersion.Equals(assemblyVersion, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show
                (
                    TryFindResource("UpdateMostRecentBodyText") as String,
                    TryFindResource("UpdateMostRecentTitleText") as String,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                var response = MessageBox.Show
                (
                    TryFindResource("UpdateNewerBodyText") as String + $"\n{assemblyVersion} -> {latestVersion}",
                    TryFindResource("UpdateNewerTitleText") as String,
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information
                );

                if(response == MessageBoxResult.OK)
                {
                    Process.Start($"https://github.com/leeanchu/ReplayBook/releases");
                }
            }

            return;
        }

        private void AcknowledgementsButton_Click(object sender, RoutedEventArgs e)
        {
            var ackDialog = new AcknowledgementsWindow
            {
                Top = App.Current.MainWindow.Top + 50,
                Left = App.Current.MainWindow.Left + 50
            };

            ackDialog.ShowDialog();
        }
    }
}
