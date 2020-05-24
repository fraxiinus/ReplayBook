using System;
using Rofl.Executables.Models;
using System.Windows;
using System.Windows.Interop;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExecutableSelectWindow.xaml
    /// </summary>
    public partial class ExecutableSelectWindow : Window
    {
        public ExecutableSelectWindow()
        {
            InitializeComponent();
        }

        public LeagueExecutable Selection { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.ActualWidth;
            this.MinHeight = this.ActualHeight;
            this.MaxHeight = this.ActualHeight;
        }

        private void ExecutableSelectWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            // Change window style
            var GWL_STYLE = -16;
            // Maximize box flag
            var WS_MAXIMIZEBOX = 0x10000;

            var windowHandle = new WindowInteropHelper((Window)sender).Handle;
            var value = NativeMethods.GetWindowLong(windowHandle, GWL_STYLE);

            // Flip maximize box flag
            _ = NativeMethods.SetWindowLong(windowHandle, GWL_STYLE, (int) (value & ~WS_MAXIMIZEBOX));
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ExecutablesListBox.SelectedItem is LeagueExecutable selectedExecutable)) { return; };

            Selection = selectedExecutable;
            this.DialogResult = true;
            return;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            return;
        }
    }
}
