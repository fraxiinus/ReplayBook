using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

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
