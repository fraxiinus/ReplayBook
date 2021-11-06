using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;
using System;
using System.Windows;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupFinish.xaml
    /// </summary>
    public partial class WelcomeSetupFinish : ModernWpf.Controls.Page, IWelcomePage
    {
        public WelcomeSetupFinish()
        {
            InitializeComponent();

            //SkipButton.IsEnabled = false;
        }

        public string GetTitle()
        {
            return (string)TryFindResource("WswFinishedFrameTitle");
        }

        public Type GetNextPage()
        {
            throw new NotSupportedException();
        }

        public Type GetPreviousPage()
        {
            return typeof(WelcomeSetupDownload);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            context.DisableNextButton = false;
            context.DisableSkipButton = true;
            context.SwapFinishButton = true;
        }
    }
}
