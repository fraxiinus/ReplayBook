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
        }

        private async void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) return;

            var downloadChamps = ChampionCheckBox.IsChecked ?? false;
            var downloadItems = ItemCheckBox.IsChecked ?? false;

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
            if (requests.Count < 1) return;

            // Disable buttons while download happens
            DownloadButton.IsEnabled = false;
            ItemCheckBox.IsEnabled = false;
            ChampionCheckBox.IsEnabled = false;

            // Disable parent next/back buttons
            var parentWindow = Window.GetWindow(this);
            if (!(parentWindow is WelcomeSetupWindow parent)) throw new ArgumentException("Parent window is not WelcomeSetupWindow type");
            parent.NextButton.IsEnabled = false;
            parent.PreviousButton.IsEnabled = false;

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

                var parentWindow = Window.GetWindow(this);
                if (!(parentWindow is WelcomeSetupWindow parent)) throw new ArgumentException("Parent window is not WelcomeSetupWindow type");
                parent.NextButton.IsEnabled = true;
                parent.PreviousButton.IsEnabled = true;
            }
        }
    }
}
