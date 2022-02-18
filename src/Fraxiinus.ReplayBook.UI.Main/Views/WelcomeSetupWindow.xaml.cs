using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for WelcomeSetupWindow.xaml
    /// </summary>
    public partial class WelcomeSetupWindow : Window
    {
        private WelcomeSetupDataContext Context
        {
            get => (DataContext is WelcomeSetupDataContext context)
                ? context
                : throw new Exception("Invalid data context");
        }

        public WelcomeSetupWindow()
        {
            InitializeComponent();
        }

        private void WelcomeSetupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Context.ContentFrame.ContentRendered += ContentFrame_ContentRendered;

            _ = Context.ContentFrame.Navigate(typeof(Pages.WelcomeSetupIntroduction));

            Context.PageTitle = (string)TryFindResource("WswIntroFrameTitle");

            InitializeNavigationDots();
        }

        private void ContentFrame_ContentRendered(object sender, EventArgs e)
        {
            // Content doesn't seem to be immediately updated after calling a navigate function,
            // so we update the page title here
            Context.PageTitle = ((Pages.IWelcomePage)(sender as ModernWpf.Controls.Frame).Content).GetTitle();
        }

        private void WelcomeSetupWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow mainWindow) { return; }
            if (mainWindow.DataContext is not MainWindowViewModel mainViewModel) { return; }

            mainViewModel.WriteSkipWelcome();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            GoToPreviousPage();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is not MainWindow mainWindow) { return; }
            if (mainWindow.DataContext is not MainWindowViewModel mainViewModel) { return; }

            Type currentPage = Context.ContentFrame.Content.GetType();

            // we are on the last page
            if (currentPage == typeof(Pages.WelcomeSetupFinish))
            {
                mainViewModel.WriteSkipWelcome();
                mainViewModel.ApplyInitialSettings(Context);
                Close();
            }

            GoToNextPage();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            GoToNextPage();
        }

        private void InitializeNavigationDots()
        {
            // dots are contained in a grid
            Grid markerPanel = NavigationDotsPanel;

            // add as many dots as there are pages
            for (int i = 0; i < Context.PageCount; i++)
            {
                markerPanel.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(20)
                });

                var dotIcon = new ModernWpf.Controls.PathIcon()
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
            // update progress dots, buttons based on page index
            if (Context.PageIndex > 0)
            {
                // In case the page needed to do anything
                Pages.IWelcomePage currentPage = (Pages.IWelcomePage)Context.ContentFrame.Content;
                _ = currentPage.GetPreviousPage();

                // go up the stack (previous page)
                Context.ContentFrame.GoBack();

                // Cannot update title here since content value is still outdated
                // context.PageTitle = ((Pages.IWelcomePage)context.ContentFrame.Content).GetTitle();

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[Context.PageIndex]).Width = 5;

                Context.PageIndex -= 1;

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[Context.PageIndex]).Width = 8;

                Context.DisableBackButton = false;
            }

            if (Context.PageIndex == 0)
            {
                Context.DisableBackButton = true;
            }

            if (Context.PageIndex < Context.PageCount)
            {
                Context.DisableNextButton = false;
            }
        }

        private void GoToNextPage()
        {
            Pages.IWelcomePage currentPage = (Pages.IWelcomePage)Context.ContentFrame.Content;

            // update progress dots, buttons based on page index
            if (Context.PageIndex + 1 < Context.PageCount)
            {
                _ = Context.ContentFrame.Navigate(currentPage.GetNextPage());

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[Context.PageIndex]).Width = 5;

                Context.PageIndex++;

                ((ModernWpf.Controls.PathIcon)NavigationDotsPanel.Children[Context.PageIndex]).Width = 8;

            }

            if (Context.PageIndex > 0)
            {
                Context.DisableBackButton = false;
            }
        }
    }
}