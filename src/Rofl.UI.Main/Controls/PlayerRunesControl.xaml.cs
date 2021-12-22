using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for PlayerRunesControl.xaml
    /// </summary>
    public partial class PlayerRunesControl : UserControl
    {
        public PlayerRunesControl()
        {
            InitializeComponent();
        }

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as MenuItem).Parent as ContextMenu).PlacementTarget is TextBlock targetTextBlock)
            {
                Clipboard.SetText(targetTextBlock.Text, TextDataFormat.UnicodeText);
            }
        }
    }
}
