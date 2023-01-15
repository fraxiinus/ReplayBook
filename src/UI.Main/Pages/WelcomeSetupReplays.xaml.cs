namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using Microsoft.WindowsAPICodePack.Dialogs;
using Fraxiinus.ReplayBook.UI.Main.Models;
using System;
using System.Windows;

/// <summary>
/// Interaction logic for WelcomeSetupReplays.xaml
/// </summary>
public partial class WelcomeSetupReplays : ModernWpf.Controls.Page, IWelcomePage
{
    private WelcomeSetupDataContext Context
    {
        get => (DataContext is WelcomeSetupDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

    public WelcomeSetupReplays()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        Context.DisableNextButton = string.IsNullOrEmpty(Context.ReplayPath);
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
        using var folderDialog = new CommonOpenFileDialog();
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

        Context.ReplayPath = folderDialog.FileName;
        Context.DisableNextButton = false;
    }
}
