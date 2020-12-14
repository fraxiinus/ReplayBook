using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for PlayerSummary.xaml
    /// </summary>
    public partial class PlayerSummaryControl : UserControl
    {
        public PlayerSummaryControl()
        {
            InitializeComponent();
        }

        private void CopyTextBlock_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is MenuItem context)) return;
            
            // Navigate upward to get the textblock that was right clicked
            // MenuItem -> ContextMenu -> TextBlock
            var textBlock = (TextBlock)((ContextMenu)context.Parent).PlacementTarget;

            Clipboard.SetText(textBlock.Text, TextDataFormat.UnicodeText);
        }
    }
}
