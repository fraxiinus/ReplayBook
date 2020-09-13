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

            SkipButton.IsEnabled = false;
        }

        //private void FinishButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    if (!(this.DataContext is MainWindowViewModel context)) return;

        //    var parentWindow = Window.GetWindow(this);
        //    if (!(parentWindow is WelcomeSetupWindow parent)) throw new ArgumentException("Parent window is not WelcomeSetupWindow type");

        //    context.ApplyInitialSettings(parent.SetupSettings);

        //    parentWindow.Close();
        //}

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToPreviousPage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;
            if (!(parent.DataContext is MainWindowViewModel context)) return;

            context.ApplyInitialSettings(parent.SetupSettings);

            parent.Close();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;
        }
    }
}
