using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf;
using ModernWpf.Controls;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
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
using System.IO;
using System.Linq;
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
            if (!(this.DataContext is SettingsManager context)) { return; }

            // Set color picker note
            if(context.Settings.AccentColor != null)
            {
                AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeCustomAccentNote") as String;
            }

            //// Change window style (i dont think this works anymore...2020-11-30)
            var GWL_STYLE = -16;
            // Maximize box flag
            var WS_MAXIMIZEBOX = 0x10000;

            var windowHandle = new WindowInteropHelper((Window)sender).Handle;
            var value = NativeMethods.GetWindowLong(windowHandle, GWL_STYLE);

            // Flip maximize box flag
            _ = NativeMethods.SetWindowLong(windowHandle, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
            //

            // Set player marker style radio
            PlayerMarkerStyleOption1.IsChecked = (context.Settings.PlayerMarkerStyle == MarkerStyle.Border);
            PlayerMarkerStyleOption2.IsChecked = (context.Settings.PlayerMarkerStyle == MarkerStyle.Square);

            // Set file action radio
            FileActionOption1.IsChecked = (context.Settings.FileAction == FileAction.Play);
            FileActionOption2.IsChecked = (context.Settings.FileAction == FileAction.Open);

            // Set theme mode radio
            AppearanceThemeOption1.IsChecked = (context.Settings.ThemeMode == 0);
            AppearanceThemeOption2.IsChecked = (context.Settings.ThemeMode == 1);
            AppearanceThemeOption3.IsChecked = (context.Settings.ThemeMode == 2);

            // Load language drop down
            foreach (var lang in (Language[])Enum.GetValues(typeof(Language)))
            {
                switch (lang)
                {
                    case Settings.Models.Language.En:
                        LanguageComboBox.Items.Add("English");
                        break;
                    case Settings.Models.Language.ZhHans:
                        LanguageComboBox.Items.Add("Chinese (Simplified)");
                        break;
                }
            }
            LanguageComboBox.SelectedIndex = (int) context.Settings.ProgramLanguage;

            // Load locale drop down
            var allLocales = Enum.GetNames(typeof(LeagueLocale))
                .Where(x => !string.Equals(x, LeagueLocale.Custom.ToString(), StringComparison.OrdinalIgnoreCase))
                .Select(x => x + " (" + ExeTools.GetLocaleCode(x) + ")");
            ExecutableLocaleComboBox.ItemsSource = allLocales;
            ExecutableLocaleComboBox.SelectedIndex = (int) context.Executables.Settings.DefaultLocale;

            // See if an update exists
            if (context.TemporaryValues.TryGetBool("UpdateAvailable", out bool update))
            {
                UpdateAvailableButton.Visibility = update ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            context.SaveConfigFile();
            this.DialogResult = true;
        }

        private async void SettingsMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((sender as ListBox).SelectedItem as ListBoxItem);

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

            // Create confirmation flyout
            var confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as String);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as String);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
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

        private async void AddSourceFolderButton_Click(object sender, RoutedEventArgs e)
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
                        // Create Dialog with error message
                        var msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as String);

                        // Show dialog
                        var msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

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
                        // Create Dialog with error message
                        var msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as String);

                        // Show dialog
                        var msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

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

            // Create confirmation flyout
            var confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as String);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as String);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
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

        private async void AddExecutableFolderButton_Click(object sender, RoutedEventArgs e)
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
                        // Create Dialog with error message
                        var msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as String);

                        // Show dialog
                        var msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

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
                        // Create Dialog with error message
                        var msgDialog = new GenericMessageDialog()
                        {
                            Title = TryFindResource("SourceFoldersAlreadyExistsErrorTitle") as String,
                            Owner = this
                        };
                        msgDialog.SetMessage(TryFindResource("SourceFoldersAlreadyExistsErrorText") as String);

                        // Show dialog
                        var msgDialogResult = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

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

            // Create confirmation flyout
            var confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as String);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as String);

            confirmFlyout.GetFlyoutButton().Click += (object eSender, RoutedEventArgs eConfirm) =>
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

            // Create Dialog with message
            var msgDialog = new GenericMessageDialog()
            {
                Title = TryFindResource("ExecutableFoldersSearchResultTitleText") as String,
                Owner = this
            };
            msgDialog.SetMessage(labelText);

            // Show dialog
            msgDialog.ShowAsync(ContentDialogPlacement.Popup);
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

            // Create confirmation flyout
            var confirmFlyout = FlyoutHelper.CreateFlyout();
            confirmFlyout.SetFlyoutLabelText(TryFindResource("ConfirmText") as String);
            confirmFlyout.SetFlyoutButtonText(TryFindResource("YesText") as String);

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
            if (!(this.DataContext is SettingsManager context)) { return; }

            FileAssociations.SetRoflToSelf();

            var msgDialog = new GenericMessageDialog()
            {
                Title = TryFindResource("FileAssociationMessageTitleText") as String,
                Owner = this
            };
            msgDialog.SetMessage(TryFindResource("FileAssociationMessageBodyText") as String);

            // Show dialog
            await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private async void UpdateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            string latestVersion;
            try
            {
                latestVersion = await GithubConnection.GetLatestVersion().ConfigureAwait(true);
            }
            catch (HttpRequestException ex)
            {
                // Http request failed, show error and stop
                // Create Dialog with error message
                var msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateExceptionTitleText") as String,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateHTTPExceptionBodyText") as String);

                // Show dialog
                await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                return;
            }
            
            if (String.IsNullOrEmpty(latestVersion))
            {
                // Either github returned nothing or got an http error code
                // Http request failed, show error and stop
                // Create Dialog with error message
                var msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateExceptionTitleText") as String,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateGitHubErrorBodyText") as String);

                // Show dialog
                await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
                return;
            }

            var assemblyName = Assembly.GetEntryAssembly()?.GetName();
            var assemblyVersion = assemblyName.Version.ToString(3);

            if (latestVersion.Equals(assemblyVersion, StringComparison.OrdinalIgnoreCase))
            {
                context.TemporaryValues["UpdateAvailable"] = false;
                UpdateAvailableButton.Visibility = Visibility.Collapsed;

                var msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateMostRecentTitleText") as String,
                    Owner = this
                };
                msgDialog.SetMessage(TryFindResource("UpdateMostRecentBodyText") as String);
                await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            }
            else
            {
                context.TemporaryValues["UpdateAvailable"] = true;
                UpdateAvailableButton.Visibility = Visibility.Visible;

                var msgDialog = new GenericMessageDialog()
                {
                    Title = TryFindResource("UpdateNewerTitleText") as String,
                    Owner = this,
                    IsSecondaryButtonEnabled = true,
                    SecondaryButtonText = TryFindResource("CancelButtonText") as String
                };
                msgDialog.SetMessage(TryFindResource("UpdateNewerBodyText") as String + $"\n{assemblyVersion} -> {latestVersion}");
                var response = await msgDialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                if(response == ContentDialogResult.Primary)
                {
                    Process.Start((TryFindResource("GitHubReleasesLink") as Uri).ToString());
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

        private void AppearanceThemeOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

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
            }
        }

        private void AccentColorResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            // Setting accent color as null resets the color
            ThemeManager.Current.AccentColor = null;
            
            // Save the null here
            context.Settings.AccentColor = null;

            // Update the button
            AccentColorButton.SelectedColor = ThemeManager.Current.ActualAccentColor;

            // Update the note
            AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeDefaultAccentNote") as String;
        }

        private void AccentColorPickerPopup_Closed(object sender, EventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            // Set accent color to picker color
            ThemeManager.Current.AccentColor = AccentColorButton.SelectedColor;

            // Update the settings value
            context.Settings.AccentColor = AccentColorButton.SelectedColorHex;

            // Update the note
            AccentColorNoteTextBlock.Text = TryFindResource("AppearanceThemeCustomAccentNote") as String;
        }

        private async Task LoadCacheSizes()
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) return;

            var results = await viewModel.CalculateCacheSizes().ConfigureAwait(true);

            var readableSizeConverter = new FormatKbSizeConverter();
            RequestsCacheItemSize.Text = (string) readableSizeConverter.Convert(results.ItemsTotalSize, null, null, CultureInfo.InvariantCulture);
            RequestsCacheChampsSize.Text = (string)readableSizeConverter.Convert(results.ChampsTotalSize, null, null, CultureInfo.InvariantCulture);
        }

        private async void DeleteItemsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) return;

            // Set the delete flag, to be deleted by the main view model on close
            viewModel.ClearItemsCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            var dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
            dialog.Title = TryFindResource("RequestsCacheCloseToDeleteTitle") as string;
            dialog.SetLabelText(TryFindResource("RequestsCacheCloseToDelete") as string);

            await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private async void DeleteChampsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) return;

            // Set the delete flag, to be deleted by the main view model on close
            viewModel.ClearChampsCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            var dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
            dialog.Title = TryFindResource("RequestsCacheCloseToDeleteTitle") as string;
            dialog.SetLabelText(TryFindResource("RequestsCacheCloseToDelete") as string);

            await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private void PlayerMarkerStyleOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

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
            if (!(this.DataContext is SettingsManager context)) { return; }

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
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel context)) return;

            // Clear the error text box
            DownloadImageErrorText.Text = String.Empty;

            // What do we download?
            var downloadChamps = ChampionCheckBox.IsChecked ?? false;
            var downloadItems = ItemCheckBox.IsChecked ?? false;

            // Nothing was selected, do nothing
            if (downloadChamps == false && downloadItems == false)
            {
                DownloadImageErrorText.Text = (string)TryFindResource("WswDownloadNoSelectionError");
                return;
            }

            // Create all the requests we need
            var requests = new List<RequestBase>();
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

            // Make progress elements visible
            DownloadProgressGrid.Visibility = Visibility.Visible;

            DownloadProgressBar.Value = 0;
            DownloadProgressBar.Minimum = 0;
            DownloadProgressBar.Maximum = requests.Count;

            foreach (var request in requests)
            {
                var response = await context.RequestManager.MakeRequestAsync(request)
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
            if (Math.Abs(DownloadProgressBar.Value) < 0.1) return;

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
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) return;

            var results = viewModel.CalculateReplayCacheSize();

            var readableSizeConverter = new FormatKbSizeConverter();
            ReplayCacheSize.Text = (string)readableSizeConverter.Convert(results, null, null, CultureInfo.InvariantCulture);
        }

        private async void DeleteReplayCacheButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.MainWindow.DataContext is MainWindowViewModel viewModel)) return;

            // Set the delete flag, to be deleted by the main view model on close
            viewModel.ClearReplayCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            var dialog = ContentDialogHelper.CreateContentDialog(includeSecondaryButton: false);
            dialog.DefaultButton = ContentDialogButton.Primary;

            dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
            dialog.Title = TryFindResource("RequestsCacheCloseToDeleteTitle") as string;
            dialog.SetLabelText(TryFindResource("RequestsCacheCloseToDelete") as string);

            await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
        }

        private void ExecutableLocaleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            context.Executables.Settings.DefaultLocale = (LeagueLocale) ExecutableLocaleComboBox.SelectedIndex;
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            context.Settings.ProgramLanguage = (Language) LanguageComboBox.SelectedIndex;
        }
    }
}
