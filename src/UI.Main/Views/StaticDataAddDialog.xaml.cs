using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.StaticData.Extensions;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using ModernWpf.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for StaticDataAddDialog.xaml
    /// </summary>
    public partial class StaticDataAddDialog : ContentDialog
    {
        public StaticDataAddDialog()
        {
            InitializeComponent();
        }

        private StaticDataManager Context
        {
            get => (DataContext is StaticDataManager context)
                ? context
                : throw new Exception("Invalid data context");
        }

        private async void RefreshPatchesButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPatchesButton.IsEnabled = false;
            PatchComboBox.IsEnabled = false;

            await Context.RefreshPatches();

            RefreshPatchesButton.IsEnabled = true;
            PatchComboBox.IsEnabled = true;
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            PatchComboBox.IsEnabled = false;
            RefreshPatchesButton.IsEnabled = false;

            ProgressBarPanel.Visibility = Visibility.Visible;

            var patch = PatchComboBox.SelectedItem.ToString();
            var progressBar = (ProgressBarPanel.Children[0] as ProgressBar);

            await Context.DownloadProperties(patch,
                    StaticDataType.Rune | StaticDataType.Item | StaticDataType.Champion,
                    LanguageHelper.CurrentLanguage.GetRiotRegionCode());
            
            progressBar.IsIndeterminate = false;
            progressBar.Value = 33;

            await Context.DownloadImages(patch, StaticDataType.Item | StaticDataType.Champion);

            progressBar.Value = 66;

            await Context.DownloadRuneImages(patch, LanguageHelper.CurrentLanguage.GetRiotRegionCode());

            progressBar.Value = 100;

            await Task.Delay(1000);

            Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
