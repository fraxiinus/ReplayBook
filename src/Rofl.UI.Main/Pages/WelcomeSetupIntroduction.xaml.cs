using Rofl.Settings.Models;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupIntroduction.xaml
    /// </summary>
    public partial class WelcomeSetupIntroduction : ModernWpf.Controls.Page, IWelcomePage
    {
        public WelcomeSetupIntroduction()
        {
            InitializeComponent();

            // Load radio buttons
            LanguageRadioButtons.ItemsSource = LanguageHelper.GetFriendlyLanguageNames();
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

        private void LanguageRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not WelcomeSetupDataContext context) { return; }

            LanguageHelper.SetProgramLanguage(context.Language);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not WelcomeSetupDataContext context) { return; }

            // select initial language after page is loaded
            LanguageRadioButtons.SelectedIndex = (int)context.Language;
        }
    }
}
