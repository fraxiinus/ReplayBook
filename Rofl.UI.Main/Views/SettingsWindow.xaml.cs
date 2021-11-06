using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf;
using ModernWpf.Controls;
using Rofl.Executables.Models;
using Rofl.Requests.Models;
using Rofl.Settings;
using Rofl.Settings.Models;
using Rofl.UI.Main.Converters;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
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
            Owner = Application.Current.MainWindow;

            // Display the first settings page
            SettingsMenuListBox.SelectedIndex = 0;

            // Set accent color button
            AccentColorButton.SelectedColor = ThemeManager.Current.ActualAccentColor;

            // Set event for when color picker closes
            AccentColorButton.ColorPickerPopup.Closed += AccentColorPickerPopup_Closed;
        }

        private void SettingsWindow_OnSourceInitialized(object sender, EventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            // Set color picker note
            if (context.Settings.AccentColor != null)
            {
                AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeCustomAccentNote") as string;
            }

            //// Change window style (i dont think this works anymore...2020-11-30)
            int GWL_STYLE = -16;
            // Maximize box flag
            int WS_MAXIMIZEBOX = 0x10000;

            IntPtr windowHandle = new WindowInteropHelper((Window)sender).Handle;
            int value = NativeMethods.GetWindowLong(windowHandle, GWL_STYLE);

            // Flip maximize box flag
            _ = NativeMethods.SetWindowLong(windowHandle, GWL_STYLE, value & ~WS_MAXIMIZEBOX);
            //

            // Set player marker style radio
            PlayerMarkerStyleOption1.IsChecked = context.Settings.PlayerMarkerStyle == MarkerStyle.Border;
            PlayerMarkerStyleOption2.IsChecked = context.Settings.PlayerMarkerStyle == MarkerStyle.Square;

            // Set file action radio
            FileActionOption1.IsChecked = context.Settings.FileAction == FileAction.Play;
            FileActionOption2.IsChecked = context.Settings.FileAction == FileAction.Open;

            // Set theme mode radio
            AppearanceThemeOption1.IsChecked = context.Settings.ThemeMode == 0;
            AppearanceThemeOption2.IsChecked = context.Settings.ThemeMode == 1;
            AppearanceThemeOption3.IsChecked = context.Settings.ThemeMode == 2;

            // Load language drop down
            LanguageComboBox.ItemsSource = LanguageHelper.GetFriendlyLanguageNames();
            LanguageComboBox.SelectedIndex = (int)context.Settings.ProgramLanguage;

            // See if an update exists
            if (context.TemporaryValues.TryGetBool("UpdateAvailable", out bool update))
            {
                UpdateAvailableButton.Visibility = update ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            context.SaveConfigFile();
            DialogResult = true;
        }

        private async void SettingsMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selectedItem = (sender as ListBox).SelectedItem as ListBoxItem;

            switch (selectedItem.Name)
            {
                case "GeneralSettingsListItem":
                    SettingsTabControl.SelectedIndex = 0;
                    break;
                case "UpdatesSettingsListItem":
                    SettingsTabControl.SelectedIndex = 1;
                    break;
                case "AppearanceSettingsListItem":
                    SettingsTabControl.SelectedIndex = 2;
                    break;
                case "ExecutablesSettingsListItem":
                    SettingsTabControl.SelectedIndex = 3;
                    break;
                case "ReplaySettingsListItem":
                    SettingsTabControl.SelectedIndex = 4;
                    LoadReplayCacheSizes();
                    break;
                case "RequestSettingsListItem":
                    SettingsTabControl.SelectedIndex = 5;
                    await LoadCacheSizes().ConfigureAwait(true);
                    break;
                case "AboutSettingsListItem":
                    SettingsTabControl.SelectedIndex = 6;
                    VersionTextBlock.Text = "Release " + ApplicationHelper.GetVersion();
                    break;
                default:
                    break;
            }
        }

        private void AddKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            PlayerMarkerDialog addDialog = new PlayerMarkerDialog
            {
                Owner = this,
                DataContext = context.Settings.KnownPlayers
            };

            _ = addDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void EditKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            PlayerMarkerDialog editDialog = new PlayerMarkerDialog(selectedPlayer)
            {
                Owner = this,
                DataContext = context.Settings.KnownPlayers
            };

            _ = editDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void RemoveKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            // Create confirmation flyout
            Flyout confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as string);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as string);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                _ = context.Settings.KnownPlayers.Remove(selectedPlayer);

                EditKnownPlayerButton.IsEnabled = false;
                RemoveKnownPlayerButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveKnownPlayerButton);
        }

        private void KnownPlayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ListBox).SelectedItem as PlayerMarker) == null) { return; };

            EditKnownPlayerButton.IsEnabled = true;
            RemoveKnownPlayerButton.IsEnabled = true;
        }

        private void SourceFoldersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ListBox).SelectedItem as string) == null) { return; };

            EditSourceFolderButton.IsEnabled = true;
            RemoveSourceFolderButton.IsEnabled = true;
        }

        private async void AddSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("SourceFoldersWindowText") as string;
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

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string selectedFolder = folderDialog.FileName;

                    if (context.Settings.SourceFolders.Contains(selectedFolder))
                    {
                        // Create Dialog with error message
                        GenericMessageDialog msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as string,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as string);

                        // Show dialog
                        ContentDialogResult msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                        // Repeat action
                        if (msgDialogResult == ContentDialogResult.Primary)
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

        private async void EditSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(SourceFoldersListBox.SelectedItem is string selectedFolder)) { return; }

            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("SourceFoldersWindowText") as string;
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
                    string newSelectedFolder = folderDialog.FileName;

                    if (context.Settings.SourceFolders.Contains(newSelectedFolder))
                    {
                        // Create Dialog with error message
                        GenericMessageDialog msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as string,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as string);

                        // Show dialog
                        ContentDialogResult msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                        // Repeat action
                        if (msgDialogResult == ContentDialogResult.Primary)
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
                        _ = context.Settings.SourceFolders.Remove(selectedFolder);
                        context.Settings.SourceFolders.Add(newSelectedFolder);
                    }
                }
            }
        }

        private void RemoveSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(SourceFoldersListBox.SelectedItem is string selectedFolder)) { return; }

            // Create confirmation flyout
            Flyout confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as string);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as string);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                _ = context.Settings.SourceFolders.Remove(selectedFolder);

                EditSourceFolderButton.IsEnabled = false;
                RemoveSourceFolderButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveSourceFolderButton);
        }

        private void ExecutableFoldersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ListBox).SelectedItem as string) == null) { return; };

            EditExecutableFolderButton.IsEnabled = true;
            RemoveExecutableFolderButton.IsEnabled = true;
        }

        private async void AddExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("ExecutableSelectFolderDialogText") as string;
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
                    string selectedFolder = folderDialog.FileName;

                    if (context.Executables.Settings.SourceFolders.Contains(selectedFolder))
                    {
                        // Create Dialog with error message
                        GenericMessageDialog msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as string,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as string);

                        // Show dialog
                        ContentDialogResult msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                        // Repeat action
                        if (msgDialogResult == ContentDialogResult.Primary)
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

        private async void EditExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(ExecutableFoldersListBox.SelectedItem is string selectedFolder)) { return; }

            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("ExecutableSelectFolderDialogText") as string;
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
                    string newSelectedFolder = folderDialog.FileName;

                    if (context.Executables.Settings.SourceFolders.Contains(newSelectedFolder))
                    {
                        // Create Dialog with error message
                        GenericMessageDialog msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as string,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as string);

                        // Show dialog
                        ContentDialogResult msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                        // Repeat action
                        if (msgDialogResult == ContentDialogResult.Primary)
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
                        _ = context.Executables.Settings.SourceFolders.Remove(selectedFolder);
                        context.Executables.Settings.SourceFolders.Add(newSelectedFolder);
                    }
                }
            }
        }

        private void RemoveExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(ExecutableFoldersListBox.SelectedItem is string selectedFolder)) { return; }

            // Create confirmation flyout
            Flyout confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as string);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as string);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                _ = context.Executables.Settings.SourceFolders.Remove(selectedFolder);

                EditExecutableFolderButton.IsEnabled = false;
                RemoveExecutableFolderButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveExecutableFolderButton);
        }

        private async void SourceFoldersSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            // stop button from being pressed and show progress
            SourceFoldersSearchButton.IsEnabled = false;
            SourceFolderSearchProgress.IsActive = true;

            (int, string[]) results = await Task.Run(() => context.Executables.SearchAllFoldersForExecutablesAndAddThemAll()).ConfigureAwait(true);
            int addedCount = results.Item1;
            string[] skippedDirs = results.Item2;

            string labelText = TryFindResource("ExecutableFoldersSearchResultLabelText") as string;
            labelText = labelText.Replace("$", addedCount.ToString(CultureInfo.InvariantCulture));

            if (skippedDirs.Length > 0)
            {
                labelText += "\n\n" + TryFindResource("ExecutableFoldersSearchResultErrorText");
                foreach (string dir in skippedDirs)
                {
                    labelText += "\n" + dir;
                }
            }

            // Create Dialog with message
            GenericMessageDialog msgDialog = new GenericMessageDialog()
            {
                Title = TryFindResource("ExecutableFoldersSearchResultTitleText") as string,
                Owner = this
            };
            msgDialog.SetMessage(labelText);

            // Show dialog
            _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            // re-enable button and hide progress
            SourceFoldersSearchButton.IsEnabled = true;
            SourceFolderSearchProgress.IsActive = false;
        }

        private void ExecutablesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ListBox).SelectedItem as LeagueExecutable) == null) { return; };

            EditExecutableButton.IsEnabled = true;
            RemoveExecutableButton.IsEnabled = true;
        }

        private void AddExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            ExecutableDetailDialog addDialog = new ExecutableDetailDialog
            {
                Owner = this,
                DataContext = context.Executables
            };

            _ = addDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void EditExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            ExecutableDetailDialog editDialog = new ExecutableDetailDialog(selectedExecutable)
            {
                Owner = this,
                DataContext = context.Executables
            };

            _ = editDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void RemoveExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            // Create confirmation flyout
            Flyout confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as string);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as string);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                context.Executables.DeleteExecutable(selectedExecutable.Name);

                EditExecutableButton.IsEnabled = false;
                RemoveExecutableButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveExecutableButton);
        }

        private async void SetFileAssocButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager)) { return; }

            FileAssociations.SetRoflToSelf();

            GenericMessageDialog msgDialog = new GenericMessageDialog()
            {
                Title = TryFindResource("FileAssociationMessageTitleText") as string,
                Owner = this
            };
            msgDialog.SetMessage(TryFindResource("FileAssociationMessageBodyText") as string);

            // Show dialog
            _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private async void UpdateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            string latestVersion;
            try
            {
                latestVersion = await GithubConnection.GetLatestVersion().ConfigureAwait(true);
            }
            catch (HttpRequestException)
            {
                // Http request failed, show error and stop
                // Create Dialog with error message
                GenericMessageDialog msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateExceptionTitleText") as string,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateHTTPExceptionBodyText") as string);

                // Show dialog
                _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                return;
            }

            if (string.IsNullOrEmpty(latestVersion))
            {
                // Either github returned nothing or got an http error code
                // Http request failed, show error and stop
                // Create Dialog with error message
                GenericMessageDialog msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateExceptionTitleText") as string,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateGitHubErrorBodyText") as string);

                // Show dialog
                _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                return;
            }

            string assemblyVersion = ApplicationHelper.GetVersion();

            if (latestVersion.Equals(assemblyVersion, StringComparison.OrdinalIgnoreCase))
            {
                context.TemporaryValues["UpdateAvailable"] = false;
                UpdateAvailableButton.Visibility = Visibility.Collapsed;

                GenericMessageDialog msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateMostRecentTitleText") as string,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateMostRecentBodyText") + $"\n{assemblyVersion}");
                _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            }
            else
            {
                context.TemporaryValues["UpdateAvailable"] = true;
                UpdateAvailableButton.Visibility = Visibility.Visible;

                GenericMessageDialog msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateNewerTitleText") as string,
                    Owner = this,
                    IsSecondaryButtonEnabled = true,
                    SecondaryButtonText = TryFindResource("CancelButtonText") as string
                };
                msgDialog.SetMessage(TryFindResource("UpdateNewerBodyText") + $"\n{assemblyVersion} -> {latestVersion}");
                ContentDialogResult response = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                if (response == ContentDialogResult.Primary)
                {
                    _ = Process.Start((TryFindResource("GitHubReleasesLink") as Uri).ToString());
                }
            }

            return;
        }

        private void AcknowledgementsButton_Click(object sender, RoutedEventArgs e)
        {
            AcknowledgementsWindow ackDialog = new AcknowledgementsWindow
            {
                Top = Application.Current.MainWindow.Top + 50,
                Left = Application.Current.MainWindow.Left + 50
            };

            _ = ackDialog.ShowDialog();
        }

        private void AppearanceThemeOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            switch (AppearanceThemeOptions.SelectedIndex)
            {
                case 0: // system default
                    ThemeManager.Current.ApplicationTheme = null;
                    context.Settings.ThemeMode = 0;
                    break;
                case 1: // dark
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    context.Settings.ThemeMode = 1;
                    break;
                case 2: // light
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    context.Settings.ThemeMode = 2;
                    break;
                default:
                    break;
            }
        }

        private void AccentColorResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            // Setting accent color as null resets the color
            ThemeManager.Current.AccentColor = null;

            // Save the null here
            context.Settings.AccentColor = null;

            // Update the button
            AccentColorButton.SelectedColor = ThemeManager.Current.ActualAccentColor;

            // Update the note
            AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeDefaultAccentNote") as string;
        }

        private void AccentColorPickerPopup_Closed(object sender, EventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            // Set accent color to picker color
            ThemeManager.Current.AccentColor = AccentColorButton.SelectedColor;

            // Update the settings value
            context.Settings.AccentColor = AccentColorButton.SelectedColorHex;

            // Update the note
            AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeCustomAccentNote") as string;
        }

        private async Task LoadCacheSizes()
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) { return; }

            (long ItemsTotalSize, long ChampsTotalSize, long RunesTotalSize) = await viewModel.CalculateCacheSizes().ConfigureAwait(true);

            FormatKbSizeConverter readableSizeConverter = new FormatKbSizeConverter();
            RequestsCacheItemSize.Text = (string)readableSizeConverter.Convert(ItemsTotalSize, null, null, CultureInfo.InvariantCulture);
            RequestsCacheChampsSize.Text = (string)readableSizeConverter.Convert(ChampsTotalSize, null, null, CultureInfo.InvariantCulture);
            RequestsCacheRunesSize.Text = (string)readableSizeConverter.Convert(RunesTotalSize, null, null, CultureInfo.InvariantCulture);
        }

        private async void DeleteItemsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) { return; }

            // Set the delete flag, to be deleted by the main view model on close
            viewModel.ClearItemsCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            ContentDialog dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
            dialog.Title = TryFindResource("RequestsCacheCloseToDeleteTitle") as string;
            dialog.SetLabelText(TryFindResource("RequestsCacheCloseToDelete") as string);

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private async void DeleteChampsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) { return; }

            // Set the delete flag, to be deleted by the main view model on close
            viewModel.ClearChampsCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            ContentDialog dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
            dialog.Title = TryFindResource("RequestsCacheCloseToDeleteTitle") as string;
            dialog.SetLabelText(TryFindResource("RequestsCacheCloseToDelete") as string);

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private async void DeleteRunesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) { return; }

            // Set the delete flag, to be deleted by the main view model on close
            viewModel.ClearRunesCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            ContentDialog dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
            dialog.Title = TryFindResource("RequestsCacheCloseToDeleteTitle") as string;
            dialog.SetLabelText(TryFindResource("RequestsCacheCloseToDelete") as string);

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private void PlayerMarkerStyleOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            if (PlayerMarkerStyleOptions.SelectedIndex == 0)
            {
                context.Settings.PlayerMarkerStyle = MarkerStyle.Border;
            }
            else if (PlayerMarkerStyleOptions.SelectedIndex == 1)
            {
                context.Settings.PlayerMarkerStyle = MarkerStyle.Square;
            }
        }

        private void FileActionOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            if (FileActionOptions.SelectedIndex == 0)
            {
                context.Settings.FileAction = FileAction.Play;
            }
            else if (FileActionOptions.SelectedIndex == 1)
            {
                context.Settings.FileAction = FileAction.Open;
            }
        }

        private async void DownloadImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel context)) { return; }

            // Clear the error text box
            DownloadImageErrorText.Text = string.Empty;

            // What do we download?
            bool downloadChamps = ChampionCheckBox.IsChecked ?? false;
            bool downloadItems = ItemCheckBox.IsChecked ?? false;
            bool downloadRunes = RunesCheckBox.IsChecked ?? false;

            // Nothing was selected, do nothing
            if (downloadChamps == false && downloadItems == false && downloadRunes == false)
            {
                DownloadImageErrorText.Text = (string)TryFindResource("WswDownloadNoSelectionError");
                return;
            }

            // Create all the requests we need
            List<RequestBase> requests = new List<RequestBase>();
            if (downloadChamps)
            {
                requests.AddRange(await context.RequestManager.GetAllChampionRequests()
                    .ConfigureAwait(true));
            }
            if (downloadItems)
            {
                requests.AddRange(await context.RequestManager.GetAllItemRequests()
                    .ConfigureAwait(true));
            }
            if (downloadRunes)
            {
                requests.AddRange(await context.RequestManager.GetAllRuneRequests(RuneHelper.GetAllRunes())
                    .ConfigureAwait(true));
            }

            // No requests? nothing to do
            if (requests.Count < 1)
            {
                DownloadImageErrorText.Text = (string)TryFindResource("WswDownloadMissingError");
                return;
            }

            // Disable buttons while download happens
            ItemCheckBox.IsEnabled = false;
            ChampionCheckBox.IsEnabled = false;
            DownloadImageButton.IsEnabled = false;
            RunesCheckBox.IsChecked = false;

            // Make progress elements visible
            DownloadProgressGrid.Visibility = Visibility.Visible;

            DownloadProgressBar.Value = 0;
            DownloadProgressBar.Minimum = 0;
            DownloadProgressBar.Maximum = requests.Count;

            foreach (RequestBase request in requests)
            {
                ResponseBase response = await context.RequestManager.MakeRequestAsync(request)
                    .ConfigureAwait(true);

                string splitSubstring = response.ResponsePath;
                if (splitSubstring.Length > 50)
                {
                    splitSubstring = response.ResponsePath.Substring(0, 35) + "..." + response.ResponsePath.Substring(response.ResponsePath.Length - 15);
                }

                DownloadProgressText.Text = splitSubstring;

                DownloadProgressBar.Value++;
            }
        }

        private void DownloadProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(DownloadProgressBar.Value) < 0.1) { return; }

            if (Math.Abs(DownloadProgressBar.Value - DownloadProgressBar.Maximum) < 0.1)
            {
                DownloadProgressText.Text = (string)TryFindResource("WswDownloadFinished");
                ItemCheckBox.IsEnabled = true;
                ChampionCheckBox.IsEnabled = true;
                DownloadImageButton.IsEnabled = true;
            }
        }

        private void LoadReplayCacheSizes()
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) { return; }

            long results = viewModel.CalculateReplayCacheSize();

            FormatKbSizeConverter readableSizeConverter = new FormatKbSizeConverter();
            ReplayCacheSize.Text = (string)readableSizeConverter.Convert(results, null, null, CultureInfo.InvariantCulture);
        }

        private async void DeleteReplayCacheButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) { return; }

            // Set the delete flag, to be deleted by the main view model on close
            viewModel.ClearReplayCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            ContentDialog dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
            dialog.Title = TryFindResource("RequestsCacheCloseToDeleteTitle") as string;
            dialog.SetLabelText(TryFindResource("RequestsCacheCloseToDelete") as string);

            _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is SettingsManager context)) { return; }

            context.Settings.ProgramLanguage = (Language)LanguageComboBox.SelectedIndex;
            LanguageHelper.SetProgramLanguage(context.Settings.ProgramLanguage);
        }
    }
}
