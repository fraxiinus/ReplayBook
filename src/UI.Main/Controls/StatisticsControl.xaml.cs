using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fraxiinus.ReplayBook.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for StatisticsControl.xaml
    /// </summary>
    public partial class StatisticsControl : UserControl
    {
        public StatisticsControl()
        {
            InitializeComponent();
        }

        // NOTE: the data grid is rotated on its side! width and height are swapped! very cool

        public double GetCombinedColumnWidth()
        {
            if (!(StatsDataGrid.ColumnHeaderStyle.Setters.First(x => (x as Setter)?.Property.Name == "Width") is Setter headerWidthSetter)) { return 0; }

            double currentHeaderWidth = (double)headerWidthSetter.Value;
            double currentControlWidth = StatsDataGrid.ActualHeight;

            return currentControlWidth - currentHeaderWidth;
        }

        private void StatsDataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) { return; }

            double availableColumnSpace = GetCombinedColumnWidth();
            int rowCount = dataGrid.Items.Count;

            // Calculate how wide the columns should be, rounding down
            int targetColumnWidth = (int)(availableColumnSpace / rowCount);

            dataGrid.RowHeight = targetColumnWidth;
        }

        // Disable auto-scroll to cell
        // Bandaid fix for drag scroll-up bug
        private void DataGrid_Row_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
