using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupFinish.xaml
    /// </summary>
    public partial class WelcomeSetupFinish : Page
    {
        public WelcomeSetupFinish()
        {
            InitializeComponent();

            SkipButton.IsEnabled = false;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }

            parent.MoveToPreviousPage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow parent)) { return; }
            if (!(parent.DataContext is MainWindowViewModel context)) { return; }

            context.ApplyInitialSettings(parent.SetupSettings);

            parent.Close();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupWindow)) { return; }
        }
    }
}
