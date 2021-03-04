using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Rofl.Executables.Utilities;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Views;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupRegion.xaml
    /// </summary>
    public partial class WelcomeSetupRegion : Page
    {

        public WelcomeSetupRegion()
        {
            InitializeComponent();

            this.SkipButton.IsEnabled = false;
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            // Load locales into combo box, set default to English
            var allLocales = Enum.GetNames(typeof(LeagueLocale))
                .Where(x => !string.Equals(x, LeagueLocale.Custom.ToString(), StringComparison.OrdinalIgnoreCase))
                .Select(x => x + " (" + ExeTools.GetLocaleCode(x) + ")");

            this.LocaleComboBox.ItemsSource = allLocales;

            this.LocaleComboBox.SelectedIndex = (int)LeagueLocale.EnglishUS;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.SetupSettings.DefaultRegionLocale = (LeagueLocale) this.LocaleComboBox.SelectedIndex;

            parent.MoveToNextPage();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToPreviousPage();
        }
    }
}
