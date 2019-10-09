using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ToolPanel.xaml
    /// </summary>
    public partial class ToolPanel : UserControl
    {
        public ToolPanel()
        {
            InitializeComponent();
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            // Get the button and menu
            ToggleButton sortButton = sender as ToggleButton;
            ContextMenu contextMenu = sortButton.ContextMenu;
            // Set placement and open
            contextMenu.PlacementTarget = sortButton;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;

            var name = Enum.GetName(context.SortParameters.SortMethod.GetType(), context.SortParameters.SortMethod);
            if (this.FindName(name) is MenuItem selectItem)
            {
                // Select our item
                selectItem.BorderBrush = SystemColors.HighlightBrush;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            MenuItem selectedItem = sender as MenuItem;
            if (Enum.TryParse(selectedItem.Name, out SortMethod selectSort))
            {
                context.SortParameters.SortMethod = selectSort;
            }

            // Clear all selections
            foreach (Object item in (this.FindName("SortMenu") as ContextMenu).Items)
            {
                if(item is MenuItem menuItem)
                {
                    menuItem.BorderBrush = Brushes.Transparent;
                }
            }
        }
    }
}
