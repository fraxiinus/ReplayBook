using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for WelcomeSetupWindow.xaml
    /// </summary>
    public partial class WelcomeSetupWindow : Window
    {
        public WelcomeSetupWindow()
        {
            InitializeComponent();
        }

        private void WelcomeSetupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            context.ContentFrame.ContentRendered += ContentFrame_ContentRendered;

            _ = context.ContentFrame.Navigate(typeof(Pages.WelcomeSetupIntroduction));

            context.PageTitle = (string)TryFindResource("WswIntroFrameTitle");

            InitializeNavigationDots();
        }

        private void ContentFrame_ContentRendered(object sender, EventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            // Content doesn't seem to be immediately updated after calling a navigate function,
            // so we update the page title here
            context.PageTitle = ((Pages.IWelcomePage)(sender as ModernWpf.Controls.Frame).Content).GetTitle();
        }

        private void WelcomeSetupWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!(DataContext is MainWindowViewModel context)) { return; }

            context.WriteSkipWelcome();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            Type currentPage = context.ContentFrame.Content.GetType();

            GoToPreviousPage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }
            if (!(Application.Current.MainWindow is MainWindow mainWindow)) { return; }
            if (!(mainWindow.DataContext is MainWindowViewModel mainViewModel)) { return; }

            Type currentPage = context.ContentFrame.Content.GetType();

            // we are on the last page
            if (currentPage == typeof(Pages.WelcomeSetupFinish))
            {
                mainViewModel.WriteSkipWelcome();
                mainViewModel.ApplyInitialSettings(context);
                Close();
            }

            GoToNextPage();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            GoToNextPage();
        }

        private void InitializeNavigationDots()
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            // dots are contained in a grid
            Grid markerPanel = NavigationDotsPanel;

            // add as many dots as there are pages
            for (int i = 0; i < context.PageCount; i++)
            {
                markerPanel.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(20)
                });

                ModernWpf.Controls.PathIcon dotIcon = new ModernWpf.Controls.PathIcon()
                {
                    Data = (Geometry)TryFindResource("CirclePathIcon"),
                    Width = 5,
                    Margin = new Thickness(5, 0, 5, 0)
                };

                if (i == 0) { dotIcon.Width = 8; }

                Grid.SetColumn(dotIcon, i);

                _ = markerPanel.Children.Add(dotIcon);
            }
        }

        private void GoToPreviousPage()
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            // update progress dots, buttons based on page index
            if (context.PageIndex > 0)
            {
                // go up the stack (previous page)
                context.ContentFrame.GoBack();

                // Cannot update title here since content value is still outdated
                // context.PageTitle = ((Pages.IWelcomePage)context.ContentFrame.Content).GetTitle();

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[context.PageIndex]).Width = 5;

                context.PageIndex -= 1;

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[context.PageIndex]).Width = 8;

                context.DisableBackButton = false;
            }

            if (context.PageIndex == 0)
            {
                context.DisableBackButton = true;
            }

            if (context.PageIndex < context.PageCount)
            {
                context.DisableNextButton = false;
            }
        }

        private void GoToNextPage()
        {
            if (!(DataContext is WelcomeSetupDataContext context)) { return; }

            Pages.IWelcomePage currentPage = (Pages.IWelcomePage)context.ContentFrame.Content;

            // update progress dots, buttons based on page index
            if (context.PageIndex + 1 < context.PageCount)
            {
                _ = context.ContentFrame.Navigate(currentPage.GetNextPage());

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[context.PageIndex]).Width = 5;

                context.PageIndex++;

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[context.PageIndex]).Width = 8;

            }

            if (context.PageIndex > 0)
            {
                context.DisableBackButton = false;
            }
        }
    }
}