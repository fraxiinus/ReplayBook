using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Rofl.Requests.Models;
using Rofl.UI.Main.ViewModels;
using Rofl.UI.Main.Views;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupDownload.xaml
    /// </summary>
    public partial class WelcomeSetupDownload : Page
    {
        public WelcomeSetupDownload()
        {
            InitializeComponent();

            this.NextButton.IsEnabled = false;
        }

        private async void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;
            if (!(parent.DataContext is MainWindowViewModel context)) return;

            // Clear the error text box
            ErrorText.Text = String.Empty;

            // What do we download?
            var downloadChamps = ChampionCheckBox.IsChecked ?? false;
            var downloadItems = ItemCheckBox.IsChecked ?? false;

            // Nothing was selected, do nothing
            if (downloadChamps == false && downloadItems == false)
            {
                ErrorText.Text = (string) TryFindResource("WswDownloadNoSelectionError");
                return;
            }

            // Create all the requests we need
            var requests = new List<RequestBase>();
            if (downloadChamps)
            {
                requests.AddRange(await context.RequestManager.GetAllChampionRequests()
                    .ConfigureAwait(true));
            }
            if (downloadItems)
            {
                requests.AddRange(await context.RequestManager.GetAllItemRequests()
                    .ConfigureAwait(true));
            }

            // No requests? nothing to do
            if (requests.Count < 1)
            {
                ErrorText.Text = (string) TryFindResource("WswDownloadMissingError");
                return;
            }

            // Disable buttons while download happens
            DownloadButton.IsEnabled = false;
            ItemCheckBox.IsEnabled = false;
            ChampionCheckBox.IsEnabled = false;

            NextButton.IsEnabled = false;
            PreviousButton.IsEnabled = false;
            SkipButton.IsEnabled = false;

            // Make progress elements visible
            DownloadProgressGrid.Visibility = Visibility.Visible;

            DownloadProgressBar.Value = 0;
            DownloadProgressBar.Minimum = 0;
            DownloadProgressBar.Maximum = requests.Count;

            foreach (var request in requests)
            {
                var response = await context.RequestManager.MakeRequestAsync(request)
                    .ConfigureAwait(true);

                DownloadProgressText.Text = response.ResponsePath;

                DownloadProgressBar.Value++;
            }
        }

        private void DownloadProgressBar_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(DownloadProgressBar.Value) < 0.1) return;

            if (Math.Abs(DownloadProgressBar.Value - DownloadProgressBar.Maximum) < 0.1)
            {
                DownloadProgressText.Text = (string) TryFindResource("WswDownloadFinished");

                NextButton.IsEnabled = true;
                PreviousButton.IsEnabled = true;
                SkipButton.IsEnabled = true;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToNextPage();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToPreviousPage();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is WelcomeSetupWindow parent)) return;

            parent.MoveToNextPage();
        }
    }
}
