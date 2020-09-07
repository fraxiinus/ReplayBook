using System;
using System.Windows;
using System.Windows.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Views;
using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.UI.Main.ViewModels;

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

            this.NextButton.IsEnabled = false;
        }

        private void BrowseReplayFolderButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;
            if (!(parent.DataContext is MainWindowViewModel context)) return;

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

                // Only continue if user presses "OK"
                if (folderDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

                _replayFolder = folderDialog.FileName;

                // display path
                BrowseReplayFolderBox.Text = _replayFolder;
                NextButton.IsEnabled = true;
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToPreviousPage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.SetupSettings.ReplayPath = _replayFolder;

            parent.MoveToNextPage();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToNextPage();
        }
    }
}
