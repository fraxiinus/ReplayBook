using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.Executables.Models;
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupExecutables.xaml
    /// </summary>
    public partial class WelcomeSetupExecutables : Page
    {

        private string _riotParentPath;
        private IList<LeagueExecutable> _executables;

        public WelcomeSetupExecutables()
        {
            InitializeComponent();

            NextButton.IsEnabled = false;
        }

        private void BrowseExecutablesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) { return; }
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }
            if (!(parent.DataContext is MainWindowViewModel context)) { return; }

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

                // Only continue if user presses "OK"
                if (folderDialog.ShowDialog() != CommonFileDialogResult.Ok) { return; }

                string selectedFolder = folderDialog.FileName;

                // Search for executables
                IList<LeagueExecutable> results = context.SettingsManager.Executables.SearchFolderForExecutables(selectedFolder);

                // Show results in preview box
                ExecutablesEmptyTextBlock.Visibility = results.Count < 1 ? Visibility.Visible : Visibility.Collapsed;
                ExecutablesPreviewListBox.ItemsSource = results;
                BrowseButtonHintText.Text = selectedFolder;

                // Save settings
                _riotParentPath = selectedFolder;
                _executables = results;

                NextButton.IsEnabled = true;
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }

            parent.MoveToPreviousPage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }

            parent.SetupSettings.RiotGamesPath = _riotParentPath;
            parent.SetupSettings.Executables = _executables;

            parent.MoveToNextPage();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }
            parent.MoveToNextPage();
        }
    }
}
