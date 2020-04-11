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
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;

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
        }

        private void FinishButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) return;

            var parentWindow = Window.GetWindow(this);
            if (!(parentWindow is WelcomeSetupWindow parent)) throw new ArgumentException("Parent window is not WelcomeSetupWindow type");

            context.ApplyInitialSettings(parent.SetupSettings);
            context.WriteSkipWelcome();

            parentWindow.Close();
        }
    }
}
