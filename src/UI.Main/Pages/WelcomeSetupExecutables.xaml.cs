namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using Fraxiinus.ReplayBook.Executables.Old.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

/// <summary>
/// Interaction logic for WelcomeSetupExecutables.xaml
/// </summary>
public partial class WelcomeSetupExecutables : ModernWpf.Controls.Page, IWelcomePage
{
    private WelcomeSetupDataContext Context
    {
        get => (DataContext is WelcomeSetupDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

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
        if (Application.Current.MainWindow is not MainWindow mainWindow) { return; }
        if (mainWindow.DataContext is not MainWindowViewModel mainViewModel) { return; }
        BrowseExecutables(mainViewModel.ExecutableManager.GetInstallationFolderFromRunningProcess());

        // check if anything is already loaded
        if (Context.Executables?.Count > 0)
        {
            ExecutablesPreviewListBox.ItemsSource = Context.Executables;
            ExecutablesEmptyTextBlock.Visibility = Visibility.Collapsed;
            Context.DisableNextButton = false;
        }
        else
        {
            Context.DisableNextButton = true;
        }
    }

    private async void BrowseExecutables(string selectedFolder)
    {
        if (string.IsNullOrEmpty(selectedFolder)) { return; }
        if (!Directory.Exists(selectedFolder)) { return; }
        if (Application.Current.MainWindow is not MainWindow mainWindow) { return; }
        if (mainWindow.DataContext is not MainWindowViewModel mainViewModel) { return; }

        // Reset previews
        ExecutablesPreviewListBox.ItemsSource = null;
        ExecutablesEmptyTextBlock.Visibility = Visibility.Visible;

        // Show updating text
        ExecutablesEmptyTextBlock.Text = (string)TryFindResource("LoadingMessageExecutables");
        SourceFolderSearchProgress.IsActive = true;
        Context.RiotGamesPath = selectedFolder;

        // Search for executables
        IList<LeagueExecutable> results = await Task.Run(() => mainViewModel.ExecutableManager.SearchFolderForExecutables(selectedFolder));

        // Reset text
        ExecutablesEmptyTextBlock.Text = (string)TryFindResource("WswExecutablesRegisterListEmpty");
        SourceFolderSearchProgress.IsActive = false;

        // only allow next button if executable found
        if (results.Count > 0)
        {
            ExecutablesPreviewListBox.ItemsSource = results;
            ExecutablesEmptyTextBlock.Visibility = Visibility.Collapsed;

            Context.Executables = results as List<LeagueExecutable>;

            Context.DisableNextButton = false;
        }
        else
        {
            ExecutablesEmptyTextBlock.Visibility = Visibility.Visible;
            Context.DisableNextButton = true;
        }
    }

    private void BrowseExecutablesButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is not MainWindow mainWindow) { return; }
        if (mainWindow.DataContext is not MainWindowViewModel) { return; }

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

        // Only continue if user presses "OK"
        if (folderDialog.ShowDialog() != CommonFileDialogResult.Ok) { return; }

        BrowseExecutables(folderDialog.FileName);
    }
}