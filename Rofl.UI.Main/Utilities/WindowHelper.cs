using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rofl.UI.Main.Utilities
{
    public static class WindowHelper
    {
        // https://stackoverflow.com/a/32599760
        public static void MoveWindowToCenter(double windowWidth, double windowHeight)
        {
            //get the current monitor
            System.Windows.Forms.Screen currentMonitor = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle);

            //find out if our app is being scaled by the monitor
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpiScaling = (source != null && source.CompositionTarget != null ? source.CompositionTarget.TransformFromDevice.M11 : 1);

            //get the available area of the monitor
            Rectangle workArea = currentMonitor.WorkingArea;
            var workAreaWidth = (int)Math.Floor(workArea.Width * dpiScaling);
            var workAreaHeight = (int)Math.Floor(workArea.Height * dpiScaling);

            //move to the centre
            Application.Current.MainWindow.Left = (((workAreaWidth - (windowWidth * dpiScaling)) / 2) + (workArea.Left * dpiScaling));
            Application.Current.MainWindow.Top = (((workAreaHeight - (windowHeight * dpiScaling)) / 2) + (workArea.Top * dpiScaling));
        }
    }
}
