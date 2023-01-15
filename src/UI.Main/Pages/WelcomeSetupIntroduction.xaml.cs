namespace Fraxiinus.ReplayBook.UI.Main.Pages;

using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for WelcomeSetupIntroduction.xaml
/// </summary>
public partial class WelcomeSetupIntroduction : ModernWpf.Controls.Page, IWelcomePage
{
    private WelcomeSetupDataContext Context
    {
        get => (DataContext is WelcomeSetupDataContext context)
            ? context
            : throw new Exception("Invalid data context");
    }

    public WelcomeSetupIntroduction()
    {
        InitializeComponent();

        // load combo box
        LanguageComboBox.ItemsSource = ConfigurationDefinitions.LanguageDisplayNames.Keys
            .OrderBy(x => x);
    }

    public string GetTitle()
    {
        return (string)TryFindResource("WswIntroFrameTitle");
    }

    public Type GetNextPage()
    {
        return typeof(WelcomeSetupExecutables);
    }

    public Type GetPreviousPage()
    {
        throw new NotSupportedException();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        // select initial language after page is loaded
        var languageNames = ConfigurationDefinitions.LanguageDisplayNames.Keys.ToArray();
        LanguageComboBox.SelectedItem = languageNames[Context.Language.GetListIndex()];
    }

    private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // selection change might trigger when datacontext is not loaded
        try
        {
            var languageCode = ConfigurationDefinitions.LanguageDisplayNames[(string)LanguageComboBox.SelectedItem];

            LanguageHelper.SetProgramLanguage((ProgramLanguage)languageCode);
        }
        catch (Exception)
        {
            // do nothing
        }
    }
}
