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
        private WelcomeSetupSettings _setupSettings;

        public WelcomeSetupRegion()
        {
            InitializeComponent();
        }

        private void WelcomeSetupRegion_OnLoaded(object sender, RoutedEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (!(parentWindow is WelcomeSetupWindow parent)) throw new ArgumentException("Parent window is not WelcomeSetupWindow type");

            _setupSettings = parent.SetupSettings;

            // Load locales into combo box, set default to English
            var allLocales = Enum.GetNames(typeof(LeagueLocale)).Select(x => x + " (" + ExeTools.GetLocaleCode(x) + ")");

            this.LocaleComboBox.ItemsSource = allLocales;

            this.LocaleComboBox.SelectedIndex = (int) LeagueLocale.EnglishUS;
        }

        private void LocaleComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _setupSettings.RegionLocale = (LeagueLocale) this.LocaleComboBox.SelectedIndex;
        }
    }
}
