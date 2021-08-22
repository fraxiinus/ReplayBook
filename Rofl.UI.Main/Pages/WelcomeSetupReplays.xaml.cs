using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupReplays.xaml
    /// </summary>
    public partial class WelcomeSetupReplays : Page
    {
        private string _replayFolder;

        public WelcomeSetupReplays()
        {
            InitializeComponent();

            NextButton.IsEnabled = false;
        }

        private void BrowseReplayFolderButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) { return; }
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }
            if (!(parent.DataContext is MainWindowViewModel)) { return; }

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

                // Only continue if user presses "OK"
                if (folderDialog.ShowDialog() != CommonFileDialogResult.Ok) { return; }

                _replayFolder = folderDialog.FileName;

                // display path
                BrowseReplayFolderBox.Text = _replayFolder;
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

            parent.SetupSettings.ReplayPath = _replayFolder;

            parent.MoveToNextPage();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }

            parent.MoveToNextPage();
        }
    }
}
