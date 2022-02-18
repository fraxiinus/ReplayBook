using ModernWpf.Controls;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for GenericMessageDialog.xaml
    /// </summary>
    public partial class GenericMessageDialog : ContentDialog
    {
        public GenericMessageDialog()
        {
            InitializeComponent();
        }

        public void SetMessage(string message)
        {
            MessageTextBlock.Text = message;
        }
    }
}
