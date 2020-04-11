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
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private WelcomeSetupSettings _setupSettings;

        public WelcomeSetupReplays()
        {
            InitializeComponent();
        }

        private void WelcomeSetupReplays_OnLoaded(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (!(parentWindow is WelcomeSetupWindow parent)) throw new ArgumentException("Parent window is not WelcomeSetupWindow type");

            _setupSettings = parent.SetupSettings;
        }

        private void BrowseReplayFolderButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) return;

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

                var selectedFolder = folderDialog.FileName;

                // Set path in parent
                _setupSettings.ReplayPath = selectedFolder;

                BrowseReplayFolderBox.Text = _setupSettings.ReplayPath;
            }
        }
    }
}
