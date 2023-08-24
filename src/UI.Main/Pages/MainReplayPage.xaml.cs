namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Models.Search;
using Fraxiinus.ReplayBook.UI.Main.Controls;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Models.View;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;
using ModernWpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

/// <summary>
/// Interaction logic for MainReplayPage.xaml
/// </summary>
public partial class MainReplayPage : ModernWpf.Controls.Page
{
    private MainWindowViewModel Context
    {
        get => (DataContext is MainWindowViewModel context)
            ? context
            : null;
    }

    private ReplayPreview _lastSelection;

    public MainReplayPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {

    }

    private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
    {
        if (Context == null) { return; }

        await Context.ReloadReplayList(false).ConfigureAwait(true);
    }

    private async void ReplayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Context == null) { return; }
        if (sender is not System.Windows.Controls.ListView replayList) { return; }
        if (replayList.SelectedItem is not ReplayPreview previewModel) { return; }

        // Deselect the last selected item
        if (_lastSelection != null && _lastSelection.IsSelected) { _lastSelection.IsSelected = false; }

        previewModel.IsSelected = true;
        _lastSelection = previewModel;

        FileResult replayFile = Context.FileResults[previewModel.Location];

        var replayDetail = new ReplayDetail(Context.StaticDataManager, replayFile, previewModel);
        await replayDetail.LoadRunes();

        ReplayDetailControl detailControl = FindName("DetailView") as ReplayDetailControl;
        detailControl.DataContext = replayDetail;

        (detailControl.FindName("BlankContent") as Grid).Visibility = Visibility.Hidden;
        (detailControl.FindName("ReplayContent") as Grid).Visibility = Visibility.Visible;

        await (DataContext as MainWindowViewModel).LoadItemThumbnails(replayDetail);

        // See if tab control needs to update runes:
        if ((detailControl.FindName("DetailTabControl") as TabControl).SelectedIndex == 1)
        {
            await Context.LoadRuneThumbnails(replayDetail).ConfigureAwait(true);
        }
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        if (Context == null) { return; }
        if (sender is not Button sortButton) { return; }

        // Get the button and menu
        ContextMenu contextMenu = sortButton.ContextMenu;
        // Set placement and open
        contextMenu.PlacementTarget = sortButton;
        contextMenu.Placement = PlacementMode.Bottom;
        contextMenu.IsOpen = true;

        string name = Enum.GetName(Context.SortParameters.SortMethod.GetType(), Context.SortParameters.SortMethod);
        if (FindName(name) is RadioMenuItem selectItem)
        {
            // Select our item
            selectItem.IsChecked = true;
        }
    }

    /// <summary>
    /// Sort menu item click handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (Context == null) { return; }
        if (sender is not RadioMenuItem selectedItem) { return; }

        if (Enum.TryParse(selectedItem.Name, out SortMethod selectSort))
        {
            Context.SortParameters.SortMethod = selectSort;
        }

        await Context.ReloadReplayList(false).ConfigureAwait(false);
    }

    /// <summary>
    /// Display or hide LoadMoreButton if scrolled to the bottom
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReplayListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (Context == null) { return; }

        // If we scrolled at all...
        if (Math.Abs(e.VerticalChange) > 0)
        {
            // If we reached the end, show the button!!!
            ReplayPageBar.Visibility = e.VerticalOffset + e.ViewportHeight == e.ExtentHeight
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Handler for LoadMoreButton
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (Context == null) { return; }

        var (received, searchResults, _) = Context.LoadReplaysFromDatabase();

        if (received == 0)
        {
            // Create and show flyout above the button
            Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false, includeCustom: false);
            flyout.SetFlyoutLabelText(TryFindResource("NoReplaysFoundTitle") as string);

            flyout.ShowAt(LoadMoreButton);

            return;
        }
        else
        {
            Context.StatusBarModel.StatusMessage = $"{Context.PreviewReplays.Count} / {searchResults}";
        }

        // Hide the button bar once we've loaded more
        ReplayPageBar.Visibility = Visibility.Collapsed;
        await Context.LoadPreviewPlayerThumbnails();
    }

    private async void SearchBox_QuerySubmitted(AutoSuggestBox auto, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (Context == null) { return; }

        Context.SortParameters.QueryString = args.QueryText;

        await Context.ReloadReplayList(false);
    }

    private void ReplayStatusBarDismissButton_Click(object sender, RoutedEventArgs e)
    {
        if (Context == null) { return; }

        Context.StatusBarModel.Visible = false;
    }

    private async void ReplayStatusBar_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (Context == null) { return; }

        // do not show error dialog if there are no errors
        if (Context.StatusBarModel.Errors == null) { return; }

        var errorDialog = new ReplayLoadErrorDialog
        {
            DataContext = Context.StatusBarModel
        };
        var result = await errorDialog.ShowAsync().ConfigureAwait(true);

        if (result == ContentDialogResult.Primary)
        {
            Context.ClearReplayCacheOnClose = true;

            // inform the user that the delete will happen when the window is closed
            var dialog = ContentDialogHelper.CreateContentDialog(
                title: TryFindResource("RequestsCacheCloseToDeleteTitle") as string,
                description: TryFindResource("RequestsCacheCloseToDelete") as string,
                primaryButtonText: TryFindResource("Settings__Replays__ClearCacheRestartNow__Button") as string,
                secondaryButtonText: TryFindResource("CancelButtonText") as string);

            var confirmResult = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
            if (confirmResult == ContentDialogResult.Primary)
            {
                Context.RestartOnClose = true;
                App.Current.MainWindow.Close();
            }
        }
    }
}
