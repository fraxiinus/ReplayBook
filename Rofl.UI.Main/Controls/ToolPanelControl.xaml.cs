using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ToolPanel.xaml
    /// </summary>
    public partial class ToolPanelControl : UserControl
    {
        private DispatcherTimer _typingTimer;

        public ToolPanelControl()
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

            context.SortPreviewReplays(this.FindResource("PreviewReplaysView") as CollectionViewSource);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            if (_typingTimer == null)
            {
                _typingTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000)
                };

                _typingTimer.Tick += new EventHandler(this.TypingTimer_Timeout);
            }

            _typingTimer.Stop();
            _typingTimer.Tag = (sender as TextBox).Text;
            _typingTimer.Start();
        }

        private void TypingTimer_Timeout(object sender, EventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }
            if (!(sender is DispatcherTimer timer)) { return; }

            string searchText = timer.Tag.ToString();

            context.SortParameters.SearchTerm = searchText;

            (this.FindResource("PreviewReplaysView") as CollectionViewSource).View.Refresh();

            timer.Stop();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is MainWindowViewModel context)) { return; }

            context.ShowSettingsDialog();
        }
    }
}
