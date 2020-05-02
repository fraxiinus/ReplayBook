using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Controls
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
            var headerWidthSetter = StatsDataGrid.ColumnHeaderStyle.Setters.First(x => (x as Setter).Property.Name == "Width") as Setter;

            var currentHeaderWidth = (double)headerWidthSetter.Value;
            var currentControlWidth = StatsDataGrid.ActualHeight;

            return currentControlWidth - currentHeaderWidth;
        }

        private void StatsDataGrid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;

            var avaliableColumnSpace = GetCombinedColumnWidth();
            var rowCount = dataGrid.Items.Count;

            // Calculate how wide the columns should be, rounding down
            var targetColumnWidth = (int) (avaliableColumnSpace / rowCount);

            dataGrid.RowHeight = targetColumnWidth;
        }
    }
}
