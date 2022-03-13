﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;
using Fraxiinus.ReplayBook.Requests.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;

namespace Fraxiinus.ReplayBook.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupDownload.xaml
    /// </summary>
    public partial class WelcomeSetupDownload : ModernWpf.Controls.Page, IWelcomePage
    {
        public WelcomeSetupDownload()
        {
            InitializeComponent();

            //NextButton.IsEnabled = false;
        }

        public string GetTitle()
        {
            return (string)TryFindResource("WswDownloadFrameTitle");
        }

        public Type GetNextPage()
        {
            return typeof(WelcomeSetupFinish);
        }

        public Type GetPreviousPage()
        {
            return typeof(WelcomeSetupReplays);
        }

        private async void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not WelcomeSetupDataContext context) { return; }
            if (Application.Current.MainWindow is not MainWindow mainWindow) { return; }
            if (mainWindow.DataContext is not MainWindowViewModel mainViewModel) { return; }

            // Clear the error text box
            ErrorText.Text = string.Empty;

            // What do we download?
            bool downloadRunes = RunesCheckBox.IsChecked ?? false;

            // Nothing was selected, do nothing
            if (downloadRunes == false)
            {
                ErrorText.Text = (string)TryFindResource("WswDownloadNoSelectionError");
                return;
            }

            // test internet by requesting latest version
            try
            {
                _ = await mainViewModel.RequestManager.GetLatestDataDragonVersionAsync().ConfigureAwait(true);
            }
            catch (HttpRequestException)
            {
                // tell user
                ContentDialog dialog = ContentDialogHelper.CreateContentDialog();
                dialog.SetLabelText((string)TryFindResource("WswDownloadMissingError"));
                dialog.Title = (string)TryFindResource("ErrorTitle");
                dialog.PrimaryButtonText = TryFindResource("OKButtonText") as string;
                dialog.DefaultButton = ContentDialogButton.Primary;
                _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);

                return;
            }

            // Create all the requests we need
            var requests = new List<RequestBase>();
            if (downloadRunes)
            {
                requests.AddRange(await mainViewModel.RequestManager.GetAllRuneRequests(mainViewModel.StaticDataProvider.GetAllRunes())
                    .ConfigureAwait(true));
            }

            // No requests? nothing to do
            if (requests.Count < 1)
            {
                ErrorText.Text = (string)TryFindResource("WswDownloadMissingError");
                return;
            }

            // Disable buttons while download happens
            DownloadButton.IsEnabled = false;
            RunesCheckBox.IsEnabled = false;

            context.DisableNextButton = true;
            context.DisableBackButton = true;
            context.DisableSkipButton = true;

            // Make progress elements visible
            DownloadProgressGrid.Visibility = Visibility.Visible;

            DownloadProgressBar.Value = 0;
            DownloadProgressBar.Minimum = 0;
            DownloadProgressBar.Maximum = requests.Count;

            foreach (RequestBase request in requests)
            {
                ResponseBase response = await mainViewModel.RequestManager.MakeRequestAsync(request)
                    .ConfigureAwait(true);

                DownloadProgressText.Text = response.ResponsePath;

                DownloadProgressBar.Value++;
            }
        }

        private void DownloadProgressBar_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DataContext is not WelcomeSetupDataContext context) { return; }

            if (Math.Abs(DownloadProgressBar.Value) < 0.1) { return; }

            if (Math.Abs(DownloadProgressBar.Value - DownloadProgressBar.Maximum) < 0.1)
            {
                DownloadProgressText.Text = (string)TryFindResource("WswDownloadFinished");

                context.DisableNextButton = false;
                context.DisableBackButton = false;
                context.DisableSkipButton = false;
            }
        }
    }
}
