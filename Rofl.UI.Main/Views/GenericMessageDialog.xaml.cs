using ModernWpf.Controls;

namespace Rofl.UI.Main.Views
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
