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

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ReplayDetailView.xaml
    /// </summary>
    public partial class ReplayDetailControl : UserControl
    {
        public ReplayDetailControl()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this).DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetailModel replay)) { return; }

            context.PlayReplay(replay.PreviewModel);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is TabControl tabControl)) { return; }

            StatsScrollViewer.ScrollToVerticalOffset(0);
        }

        private void StatsScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            StatsScrollViewer.ScrollToVerticalOffset(0);
        }
    }
}
