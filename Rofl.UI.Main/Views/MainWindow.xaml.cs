using Microsoft.Extensions.Configuration;
using Rofl.Executables;
using Rofl.Files;
using Rofl.Logger;
using Rofl.Reader;
using Rofl.Reader.Models;
using Rofl.Requests;
using Rofl.Settings;
using Rofl.UI.Main.Controls;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
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

namespace Rofl.UI.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileManager _files;
        private readonly RequestManager _requests;
        private readonly SettingsManager _settingsManager;
        private readonly Scribe _log;

        public MainWindow()
        {
            InitializeComponent();

            //_config = new ConfigurationBuilder()
            //    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .Build();
            _log = new Scribe();

            _settingsManager = new SettingsManager(_log);

            _files = new FileManager(_settingsManager.Settings, _log);
            _requests = new RequestManager(_settingsManager.Settings, _log);

            _log.Error("lol", "ALPHA DEBUG");

            this.DataContext = new MainWindowViewModel(_files, _requests, _settingsManager);
        }

        private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
        {
            // configure await it set to true, otherwise we get some errors...
            await (DataContext as MainWindowViewModel).LoadReplays().ConfigureAwait(true);
            await (DataContext as MainWindowViewModel).LoadPreviewPlayerThumbnails().ConfigureAwait(true);
        }

        private void PreviewReplaysView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (this.DataContext as MainWindowViewModel).FilterPreviewReplay(e.Item as ReplayPreviewModel);
        }

        private async void ReplayItemControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ReplayPreviewModel previewModel = (sender as ReplayItemControl).DataContext as ReplayPreviewModel;

            ReplayFile replayModel = (this.DataContext as MainWindowViewModel).FileResults[previewModel.MatchId].ReplayFile;

            ReplayDetailModel replayDetailModel = new ReplayDetailModel(replayModel, previewModel);

            ReplayDetailControl detailControl = this.FindName("DetailView") as ReplayDetailControl;
            detailControl.DataContext = replayDetailModel;

            (detailControl.FindName("BlankContent") as StackPanel).Visibility = Visibility.Hidden;
            (detailControl.FindName("ReplayContent") as StackPanel).Visibility = Visibility.Visible;


            await (this.DataContext as MainWindowViewModel).LoadItemThumbnails(replayDetailModel).ConfigureAwait(true);
        }

    }
}
