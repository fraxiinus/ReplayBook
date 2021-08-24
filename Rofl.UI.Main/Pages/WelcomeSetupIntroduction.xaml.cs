using Rofl.Settings.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupIntroduction.xaml
    /// </summary>
    public partial class WelcomeSetupIntroduction : Page
    {
        public WelcomeSetupIntroduction()
        {
            InitializeComponent();

            // Disable buttons
            PreviousButton.IsEnabled = false;
            SkipButton.IsEnabled = false;

            // Load radio buttons
            LanguageRadioButtons.ItemsSource = LanguageHelper.GetFriendlyLanguageNames();
            LanguageRadioButtons.SelectedIndex = 0;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }

            parent.MoveToNextPage();
        }

        private void LanguageRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }
            parent.SetupSettings.SetupLanguage = (Language)LanguageRadioButtons.SelectedIndex;

            LanguageHelper.SetProgramLanguage(parent.SetupSettings.SetupLanguage);
        }
    }
}
