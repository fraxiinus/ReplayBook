namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using Fraxiinus.ReplayBook.UI.Main.Models;
using System;
using System.Windows;

/// <summary>
/// Interaction logic for WelcomeSetupFinish.xaml
/// </summary>
public partial class WelcomeSetupFinish : ModernWpf.Controls.Page, IWelcomePage
{
    private WelcomeSetupDataContext Context
    {
        get => (DataContext is WelcomeSetupDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

    public WelcomeSetupFinish()
    {
        InitializeComponent();
    }

    public string GetTitle()
    {
        return (string)TryFindResource("WswFinishedFrameTitle");
    }

    public Type GetNextPage()
    {
        throw new NotSupportedException();
    }

    public Type GetPreviousPage()
    {
        Context.SwapFinishButton = false;
        Context.DisableSkipButton = false;
        return typeof(WelcomeSetupReplays);
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        // prevent users from ending up in the download page again!
        Context.DisableBackButton = true;
        Context.DisableNextButton = false;
        Context.DisableSkipButton = true;
        Context.SwapFinishButton = true;
    }
}
