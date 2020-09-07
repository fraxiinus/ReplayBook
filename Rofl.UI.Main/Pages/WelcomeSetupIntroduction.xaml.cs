using Rofl.UI.Main.Views;
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
