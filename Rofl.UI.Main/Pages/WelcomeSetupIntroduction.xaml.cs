using Rofl.UI.Main.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        }

        private void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToNextPage();
        }
    }
}
