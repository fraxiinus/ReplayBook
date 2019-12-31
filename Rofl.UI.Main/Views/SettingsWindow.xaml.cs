using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.Executables.Models;
using Rofl.Settings;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                default:
                    break;
            }
        }

        private void AddKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            var addDialog = new PlayerMarkerWindow
            {
                Top = this.Top + 50,
                Left = this.Left + 50,
                Owner = this,
                DataContext = context.Settings.KnownPlayers
            };

            addDialog.ShowDialog();

        }

        private void EditKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            var editDialog = new PlayerMarkerWindow(selectedPlayer)
            {
                Top = this.Top + 50,
                Left = this.Left + 50,
                Owner = this,
                DataContext = context.Settings.KnownPlayers
            };

            editDialog.ShowDialog();

        }
        
        private void RemoveKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            context.Settings.KnownPlayers.Remove(selectedPlayer);

            EditKnownPlayerButton.IsEnabled = false;
            RemoveKnownPlayerButton.IsEnabled = false;
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
                            AddSourceFolderButton_Click(null, null);
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

            context.Settings.SourceFolders.Remove(selectedFolder);

            EditSourceFolderButton.IsEnabled = false;
            RemoveSourceFolderButton.IsEnabled = false;
        }

        private void ExecutableFoldersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem as string == null) { return; };

            EditExecutableFolderButton.IsEnabled = true;
            RemoveExecutableFolderButton.IsEnabled = true;
        }

        private void AddExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveExecutableFolderButton_Click(object sender, RoutedEventArgs e)
        {

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

            var addDialog = new ExecutableDetailWindow
            {
                Top = this.Top + 50,
                Left = this.Left + 50,
                Owner = this,
                DataContext = context.Executables
            };

            addDialog.ShowDialog();
        }

        private void EditExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            var editDialog = new ExecutableDetailWindow(selectedExecutable)
            {
                Top = this.Top + 50,
                Left = this.Left + 50,
                Owner = this,
                DataContext = context.Executables
            };

            editDialog.ShowDialog();
        }

        private void RemoveExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            context.Executables.DeleteExecutable(selectedExecutable.Name);

            EditExecutableButton.IsEnabled = false;
            RemoveExecutableButton.IsEnabled = false;
        }
    }
}
