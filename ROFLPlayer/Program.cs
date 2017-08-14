using System;
using System.Windows.Forms;
using System.Threading;

namespace ROFLPlayer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static Mutex mutex = new Mutex(true, "{f847ab42-e13e-43ba-990a-1f781d5966e4}");

        [STAThread]
        static void Main(string[] args)
        {
            if(mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(args));
                mutex.ReleaseMutex();
            }
            else
            {
                WinMethods.PostMessage(
                    (IntPtr)WinMethods.HWND_BROADCAST,
                    WinMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
        }
    }
}
