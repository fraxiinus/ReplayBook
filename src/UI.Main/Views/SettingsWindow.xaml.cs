using Fraxiinus.ReplayBook.Configuration;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old.Models;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Converters;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// We need to update executables if executables were changed
        /// </summary>
        public bool UpdateExecutablesOnClose { get; private set; }

        private SettingsWindowDataContext Context
        {
            get => (DataContext is SettingsWindowDataContext context)
                ? context
                : throw new Exception("Invalid data context");
        }

        public SettingsWindow()
        {
            UpdateExecutablesOnClose = false;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            // Set color picker note
            if (!string.IsNullOrEmpty(context.Configuration.AccentColor))
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
            PlayerMarkerStyleOption1.IsChecked = context.Configuration.MarkerStyle == MarkerStyle.Border;
            PlayerMarkerStyleOption2.IsChecked = context.Configuration.MarkerStyle == MarkerStyle.Square;

            // Set file action radio
            FileActionOption1.IsChecked = context.Configuration.FileAction == FileAction.Play;
            FileActionOption2.IsChecked = context.Configuration.FileAction == FileAction.Open;

            // Set theme mode radio
            AppearanceThemeOption1.IsChecked = context.Configuration.ThemeMode == Theme.SystemAssigned;
            AppearanceThemeOption2.IsChecked = context.Configuration.ThemeMode == Theme.Dark;
            AppearanceThemeOption3.IsChecked = context.Configuration.ThemeMode == Theme.Light;

            // Load language drop down
            LanguageComboBox.ItemsSource = ConfigurationDefinitions.LanguageDisplayNames.Keys
                .OrderBy(x => x);

            // select initial language after page is loaded
            var languageNames = ConfigurationDefinitions.LanguageDisplayNames.Keys.ToArray();
            LanguageComboBox.SelectedItem = languageNames[context.Configuration.Language.GetListIndex()];

            // See if an update exists
            if (context.Configuration.Stash.TryGetBool("UpdateAvailable", out bool update))
            {
                UpdateAvailableButton.Visibility = update ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (Application.Current.MainWindow.DataContext is not MainWindowViewModel viewModel) { return; }

            await viewModel.StaticDataManager.SaveIndexAsync();
            await context.Configuration.ToConfigurationFile().SaveConfigurationFile();
            context.Executables.Save();
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
                case "StaticDataSettingsListItem":
                    SettingsTabControl.SelectedIndex = 4;
                    await StaticDataSizeValue_CalculateTotalValue();
                    break;
                case "ReplaySettingsListItem":
                    SettingsTabControl.SelectedIndex = 5;
                    LoadReplayCacheSizes();
                    break;
                //case "RequestSettingsListItem":
                //    SettingsTabControl.SelectedIndex = 6;
                //    //await LoadCacheSizes().ConfigureAwait(true);
                //    break;
                case "AboutSettingsListItem":
                    SettingsTabControl.SelectedIndex = 6;
                    VersionTextBlock.Text = "Release " + ApplicationProperties.Version;
                    break;
                default:
                    break;
            }
        }

        private void AddKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            var addDialog = new PlayerMarkerDialog
            {
                Owner = this,
                DataContext = context.Configuration.PlayerMarkers
            };

            _ = addDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void EditKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (KnownPlayersListBox.SelectedItem is not PlayerMarkerConfiguration selectedPlayer) { return; }

            var editDialog = new PlayerMarkerDialog(selectedPlayer)
            {
                Owner = this,
                DataContext = context.Configuration.PlayerMarkers
            };

            _ = editDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void RemoveKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (KnownPlayersListBox.SelectedItem is not PlayerMarkerConfiguration selectedPlayer) { return; }

            // Create confirmation flyout
            Flyout confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as string);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as string);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                _ = context.Configuration.PlayerMarkers.Remove(selectedPlayer);

                EditKnownPlayerButton.IsEnabled = false;
                RemoveKnownPlayerButton.IsEnabled = false;
                confirmFlyout.Hide();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveKnownPlayerButton);
        }

        private void KnownPlayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ListBox).SelectedItem as PlayerMarkerConfiguration) == null) { return; };

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
            if (DataContext is not SettingsWindowDataContext context) { return; }

            using var folderDialog = new CommonOpenFileDialog();
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

                if (context.Configuration.ReplayFolders.Contains(selectedFolder))
                {
                    // Create Dialog with error message
                    var msgDialog = new GenericMessageDialog()
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
                    context.Configuration.ReplayFolders.Add(selectedFolder);
                }
            }

        }

        private async void EditSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (SourceFoldersListBox.SelectedItem is not string selectedFolder) { return; }

            using var folderDialog = new CommonOpenFileDialog();
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

                if (context.Configuration.ReplayFolders.Contains(newSelectedFolder))
                {
                    // Create Dialog with error message
                    var msgDialog = new GenericMessageDialog()
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
                    _ = context.Configuration.ReplayFolders.Remove(selectedFolder);
                    context.Configuration.ReplayFolders.Add(newSelectedFolder);
                }
            }
        }

        private void RemoveSourceFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (SourceFoldersListBox.SelectedItem is not string selectedFolder) { return; }

            // Create confirmation flyout
            Flyout confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as string);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as string);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
            {
                _ = context.Configuration.ReplayFolders.Remove(selectedFolder);

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
            if (DataContext is not SettingsWindowDataContext context) { return; }

            using var folderDialog = new CommonOpenFileDialog();
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
                    var msgDialog = new GenericMessageDialog()
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
                    
                    // executables have been changed, need to update
                    UpdateExecutablesOnClose = true;
                }
            }
        }

        private async void EditExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (ExecutableFoldersListBox.SelectedItem is not string selectedFolder) { return; }

            using var folderDialog = new CommonOpenFileDialog();
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
                    var msgDialog = new GenericMessageDialog()
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

                    // executables have been changed, need to update
                    UpdateExecutablesOnClose = true;
                }
            }
        }

        private void RemoveExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (ExecutableFoldersListBox.SelectedItem is not string selectedFolder) { return; }

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

                // executables have been changed, need to update
                UpdateExecutablesOnClose = true;
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveExecutableFolderButton);
        }

        private async void SourceFoldersSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            // stop button from being pressed and show progress
            SourceFoldersSearchButton.IsEnabled = false;
            SourceFolderSearchProgress.IsActive = true;

            (int, string[]) results = await Task.Run(() => context.Executables.SearchAllFoldersForExecutablesAndAddThemAll()).ConfigureAwait(true);
            int addedCount = results.Item1;
            string[] skippedDirs = results.Item2;

            string labelText = TryFindResource("ExecutableFoldersSearchResultLabelText") as string;
            labelText = labelText.Replace("$", addedCount.ToString(CultureInfo.InvariantCulture));

            if (addedCount > 0)
            {
                // executables have been changed, need to update
                UpdateExecutablesOnClose = true;
            }

            if (skippedDirs.Length > 0)
            {
                labelText += "\n\n" + TryFindResource("ExecutableFoldersSearchResultErrorText");
                foreach (string dir in skippedDirs)
                {
                    labelText += "\n" + dir;
                }
            }

            // Create Dialog with message
            var msgDialog = new GenericMessageDialog()
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
            if (DataContext is not SettingsWindowDataContext context) { return; }

            var addDialog = new ExecutableDetailDialog
            {
                Owner = this,
                DataContext = context.Executables
            };

            _ = addDialog.ShowAsync(ContentDialogPlacement.Popup);

            // executables have been changed, need to update
            UpdateExecutablesOnClose = true;
        }

        private void EditExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (ExecutablesListBox.SelectedItem is not LeagueExecutable selectedExecutable) { return; };

            var editDialog = new ExecutableDetailDialog(selectedExecutable)
            {
                Owner = this,
                DataContext = context.Executables
            };

            _ = editDialog.ShowAsync(ContentDialogPlacement.Popup);

            // executables have been changed, need to update
            UpdateExecutablesOnClose = true;
        }

        private void RemoveExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (ExecutablesListBox.SelectedItem is not LeagueExecutable selectedExecutable) { return; };

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

                // executables have been changed, need to update
                UpdateExecutablesOnClose = true;
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveExecutableButton);
        }

        private async void SetFileAssocButton_Click(object sender, RoutedEventArgs e)
        {
            FileAssociations.SetRoflToSelf();

            var msgDialog = new GenericMessageDialog()
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
            if (DataContext is not SettingsWindowDataContext context) { return; }
            UpdateCheckButton.IsEnabled = false;

            string latestVersion;
            try
            {
                latestVersion = await GithubConnection.GetLatestVersion().ConfigureAwait(true);
            }
            catch (HttpRequestException)
            {
                // Http request failed, show error and stop
                // Create Dialog with error message
                var msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateExceptionTitleText") as string,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateHTTPExceptionBodyText") as string);

                // Show dialog
                _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                UpdateCheckButton.IsEnabled = true;
                return;
            }

            if (string.IsNullOrEmpty(latestVersion))
            {
                // Either github returned nothing or got an http error code
                // Http request failed, show error and stop
                // Create Dialog with error message
                var msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateExceptionTitleText") as string,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateGitHubErrorBodyText") as string);

                // Show dialog
                _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                UpdateCheckButton.IsEnabled = true;
                return;
            }

            string assemblyVersion = ApplicationProperties.Version;

            if (latestVersion.Equals(assemblyVersion, StringComparison.OrdinalIgnoreCase))
            {
                context.Configuration.Stash["UpdateAvailable"] = false;
                UpdateAvailableButton.Visibility = Visibility.Collapsed;

                var msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateMostRecentTitleText") as string,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateMostRecentBodyText") + $"\n{assemblyVersion}");
                _ = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            }
            else
            {
                context.Configuration.Stash["UpdateAvailable"] = true;
                UpdateAvailableButton.Visibility = Visibility.Visible;

                var msgDialog = new GenericMessageDialog()
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

            UpdateCheckButton.IsEnabled = true;
            return;
        }

        private void AcknowledgementsButton_Click(object sender, RoutedEventArgs e)
        {
            var ackDialog = new AcknowledgementsWindow
            {
                Top = Application.Current.MainWindow.Top + 50,
                Left = Application.Current.MainWindow.Left + 50
            };

            _ = ackDialog.ShowDialog();
        }

        private void AppearanceThemeOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            switch (AppearanceThemeOptions.SelectedIndex)
            {
                case 0: // system default
                    ThemeManager.Current.ApplicationTheme = null;
                    context.Configuration.ThemeMode = Theme.SystemAssigned;
                    break;
                case 1: // dark
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    context.Configuration.ThemeMode = Theme.Dark;
                    break;
                case 2: // light
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    context.Configuration.ThemeMode = Theme.Light;
                    break;
                default:
                    break;
            }
        }

        private void AccentColorResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            // Setting accent color as null resets the color
            ThemeManager.Current.AccentColor = null;

            // Save the null here
            context.Configuration.AccentColor = null;

            // Update the button
            AccentColorButton.SelectedColor = ThemeManager.Current.ActualAccentColor;

            // Update the note
            AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeDefaultAccentNote") as string;
        }

        private void AccentColorPickerPopup_Closed(object sender, EventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            // Set accent color to picker color
            ThemeManager.Current.AccentColor = AccentColorButton.SelectedColor;

            // Update the settings value
            context.Configuration.AccentColor = AccentColorButton.SelectedColorHex;

            // Update the note
            AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeCustomAccentNote") as string;
        }

        private void PlayerMarkerStyleOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            if (PlayerMarkerStyleOptions.SelectedIndex == 0)
            {
                context.Configuration.MarkerStyle = MarkerStyle.Border;
            }
            else if (PlayerMarkerStyleOptions.SelectedIndex == 1)
            {
                context.Configuration.MarkerStyle = MarkerStyle.Square;
            }
        }

        private void FileActionOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not SettingsWindowDataContext context) { return; }

            if (FileActionOptions.SelectedIndex == 0)
            {
                context.Configuration.FileAction = FileAction.Play;
            }
            else if (FileActionOptions.SelectedIndex == 1)
            {
                context.Configuration.FileAction = FileAction.Open;
            }
        }

        private void LoadReplayCacheSizes()
        {
            if (Application.Current.MainWindow.DataContext is not MainWindowViewModel viewModel) { return; }

            long results = viewModel.CalculateReplayCacheSize();

            var readableSizeConverter = new FormatKbSizeConverter();
            ReplayCacheSize.Text = (string)readableSizeConverter.Convert(results, null, null, CultureInfo.InvariantCulture);
        }

        private async void DeleteReplayCacheButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext is not MainWindowViewModel viewModel) { return; }

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
            if (DataContext is not SettingsWindowDataContext context) { return; }
            if (Application.Current.MainWindow.DataContext is not MainWindowViewModel viewModel) { return; }

            // convert sorted combobox item to actual code
            var languageCode = ConfigurationDefinitions.LanguageDisplayNames[(string)LanguageComboBox.SelectedItem];

            // save language to configuration
            context.Configuration.Language = (ProgramLanguage)languageCode;
            // load language strings in application
            LanguageHelper.SetProgramLanguage(context.Configuration.Language);
            // load static data in new language
            // await viewModel.StaticDataProvider.Reload(context.Configuration.Language);
        }

        private async Task StaticDataSizeValue_CalculateTotalValue()
        {
            var totalSize = await Context.StaticData.CalculateDiskUsage();

            var readableSizeConverter = new FormatKbSizeConverter();
            StaticDataSizeValue.Text = (string)readableSizeConverter.Convert(totalSize, null, null, CultureInfo.InvariantCulture);
        }

        private void StaticDataDownloadedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((sender as ListBox).SelectedItem as ObservableBundle) == null) { return; };

            EditStaticDataButton.IsEnabled = true;
            RemoveStaticDataButton.IsEnabled = true;
        }

        private async void AddStaticDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Update patches if we don't know any
            await Context.StaticData.GetPatchesIfOutdated();

            var addDialog = new StaticDataAddDialog()
            {
                DataContext = Context.StaticData,
                Owner = this
            };
            var dialogResult = await addDialog.ShowAsync();

            if (dialogResult == ContentDialogResult.Primary)
            {
                await StaticDataDownloadDialog.StartDownloadDialog(addDialog.SelectedPatch);

                await StaticDataSizeValue_CalculateTotalValue();
            }
        }

        private async void EditStaticDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (StaticDataDownloadedListBox.SelectedItem is not ObservableBundle targetBundle) { return; }

            var detailDialog = new StaticDataDetailDialog()
            {
                DataContext = targetBundle,
                Owner = this
            };

            await detailDialog.ShowAsync();
        }

        private void RemoveStaticDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (StaticDataDownloadedListBox.SelectedItem is not ObservableBundle targetBundle) { return; }

            // Create confirmation flyout
            Flyout confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as string);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as string);

            confirmFlyout.GetFlyoutButton().Click += async (object eSender, RoutedEventArgs eConfirm) =>
            {
                Context.StaticData.DeleteBundle(targetBundle.Patch);

                EditStaticDataButton.IsEnabled = false;
                RemoveStaticDataButton.IsEnabled = false;
                confirmFlyout.Hide();

                await StaticDataSizeValue_CalculateTotalValue();
            };

            // Show the flyout
            confirmFlyout.ShowAt(RemoveStaticDataButton);
        }

        private void BrowseStaticDataFolder_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("explorer.exe", Context.StaticData.GetDataPath());
        }
    }
}
