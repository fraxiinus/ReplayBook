using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rofl.UI.Main.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Views;


namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupExecutables.xaml
    /// </summary>
    public partial class WelcomeSetupExecutables : Page
    {
        private WelcomeSetupSettings _setupSettings;

        public WelcomeSetupExecutables()
        {
            InitializeComponent();
        }

        private void WelcomeSetupExecutables_OnLoaded(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (!(parentWindow is WelcomeSetupWindow parent)) throw new ArgumentException("Parent window is not WelcomeSetupWindow type");

            _setupSettings = parent.SetupSettings;
        }

        private void BrowseExecutablesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            if (!(this.DataContext is MainWindowViewModel context)) return;

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

                // Only continue if user presses "OK"
                if (folderDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

                var selectedFolder = folderDialog.FileName;

                // Search for executables
                var results = context.SettingsManager.Executables.SearchFolderForExecutables(selectedFolder);

                // Show results in preview box
                ExecutablesEmptyTextBlock.Visibility = results.Count < 1 ? Visibility.Visible : Visibility.Collapsed;
                
                this.ExecutablesPreviewListBox.ItemsSource = results;

                // Set path in parent
                _setupSettings.RiotGamesPath = selectedFolder;

                BrowseButtonHintText.Text = _setupSettings.RiotGamesPath;
            }
        }
    }
}
