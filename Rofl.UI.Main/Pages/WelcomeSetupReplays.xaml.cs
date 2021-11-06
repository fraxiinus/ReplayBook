using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.UI.Main.Models;
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
    public partial class WelcomeSetupReplays : ModernWpf.Controls.Page, IWelcomePage
    {
        public WelcomeSetupReplays()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            context.DisableNextButton = string.IsNullOrEmpty(context.ReplayPath);
        }

        public string GetTitle()
        {
            return (string)TryFindResource("WswReplaysFrameTitle");
        }

        public Type GetNextPage()
        {
            return typeof(WelcomeSetupDownload);
        }

        public Type GetPreviousPage()
        {
            return typeof(WelcomeSetupExecutables);
        }

        private void BrowseReplayFolderButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

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

                context.ReplayPath = folderDialog.FileName;
                context.DisableNextButton = false;
            }
        }
    }
}
