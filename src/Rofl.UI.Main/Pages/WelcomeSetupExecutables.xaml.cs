using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.Executables.Models;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupExecutables.xaml
    /// </summary>
    public partial class WelcomeSetupExecutables : ModernWpf.Controls.Page, IWelcomePage
    {
        public WelcomeSetupExecutables()
        {
            InitializeComponent();
        }

        public string GetTitle()
        {
            return (string)TryFindResource("WswExecutablesFrameTitle");
        }

        public Type GetNextPage()
        {
            return typeof(WelcomeSetupReplays);
        }

        public Type GetPreviousPage()
        {
            return typeof(WelcomeSetupIntroduction);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            // check if anything is already loaded
            if (context.Executables?.Count > 0)
            {
                ExecutablesPreviewListBox.ItemsSource = context.Executables;
                ExecutablesEmptyTextBlock.Visibility = Visibility.Collapsed;
                context.DisableNextButton = false;
            }
            else
            {
                context.DisableNextButton = true;
            }
        }

        private async void BrowseExecutablesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }
            if (!(Application.Current.MainWindow is MainWindow mainWindow)) { return; }
            if (!(mainWindow.DataContext is MainWindowViewModel mainViewModel)) { return; }

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

                // reset previews
                string selectedFolder = folderDialog.FileName;
                ExecutablesPreviewListBox.ItemsSource = null;
                ExecutablesEmptyTextBlock.Visibility = Visibility.Visible;

                // Show updating text
                ExecutablesEmptyTextBlock.Text = (string)TryFindResource("LoadingMessageExecutables");
                SourceFolderSearchProgress.IsActive = true;
                context.RiotGamesPath = selectedFolder;

                // Search for executables
                IList<LeagueExecutable> results = await Task.Run(() => mainViewModel.SettingsManager.Executables.SearchFolderForExecutables(selectedFolder));

                // Reset text
                ExecutablesEmptyTextBlock.Text = (string)TryFindResource("WswExecutablesRegisterListEmpty");
                SourceFolderSearchProgress.IsActive = false;

                // only allow next button if executable found
                if (results.Count > 0)
                {
                    ExecutablesPreviewListBox.ItemsSource = results;
                    ExecutablesEmptyTextBlock.Visibility = Visibility.Collapsed;

                    context.Executables = results as List<LeagueExecutable>;

                    context.DisableNextButton = false;
                }
                else
                {
                    ExecutablesEmptyTextBlock.Visibility = Visibility.Visible;
                    context.DisableNextButton = true;
                }
            }
        }
    }
}
