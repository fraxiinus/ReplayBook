using Microsoft.Extensions.Configuration;
using Rofl.Files;
using Rofl.Logger;
using Rofl.Reader;
using Rofl.Requests;
using Rofl.UI.Main.Controls;
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
        private IConfiguration _config;

        private FileManager _files;

        private RequestManager _requests;

        private Scribe _log;

        public MainWindow()
        {
            InitializeComponent();

            _config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _log = new Scribe();

            _files = new FileManager(_config, _log);

            _requests = new RequestManager(_config, _log);

            _log.Error("lol", "lol");

            this.DataContext = new MainWindowViewModel(_files, _requests);
        }

        private async void ReplayListView_Loaded(object sender, RoutedEventArgs e)
        {
            await (this.DataContext as MainWindowViewModel).LoadReplays();
            await (this.DataContext as MainWindowViewModel).LoadPreviewPlayerThumbnails();
        }
    }
}
