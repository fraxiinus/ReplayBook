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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for RenameFileDialog.xaml
    /// </summary>
    public partial class RenameFileDialog : Window
    {
        public string FileName { get; set; }

        public RenameFileDialog()
        {
            InitializeComponent();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            // Change window style
            var GWL_STYLE = -16;
            // Maximize box flag
            var WS_MAXIMIZEBOX = 0x10000;

            var windowHandle = new WindowInteropHelper((Window)sender).Handle;
            var value = NativeMethods.GetWindowLong(windowHandle, GWL_STYLE);

            // Flip maximize box flag
            _ = NativeMethods.SetWindowLong(windowHandle, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.ActualWidth;
            this.MinHeight = this.ActualHeight;
            this.MaxHeight = this.ActualHeight;

            NameTextBox.Text = FileName;
            NameTextBox.ToolTip = new ToolTip 
            { 
                Content = ((string)TryFindResource("RenameDialogInvalidCharacters"))
                    .Replace("{n}", "\n")
                    .Replace("{t}", "\t") 
            };

            NameTextBox.Focus();
            NameTextBox.SelectAll();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            FileName = NameTextBox.Text;
            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var invalidCharsExist = NameTextBox.Text.IndexOfAny("\\/:*?\"<>|".ToCharArray()) != -1;
            if (invalidCharsExist)
            {
                // Disable submit button
                OKButton.IsEnabled = false;

                // Show error message
                if (NameTextBox.ToolTip as ToolTip != null)
                {
                    (NameTextBox.ToolTip as ToolTip).IsOpen = true;
                }
            }
            else
            {
                if (NameTextBox.ToolTip as ToolTip != null)
                {
                    (NameTextBox.ToolTip as ToolTip).IsOpen = false;
                }

                OKButton.IsEnabled = true;
            }
        }
    }
}
