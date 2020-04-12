using System;
using System.Windows;
using System.Windows.Controls;
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

            parentWindow.Close();
        }
    }
}
