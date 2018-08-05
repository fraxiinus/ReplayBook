using System;
using System.Windows.Forms;
using ROFLPlayer.Utilities;

namespace ROFLPlayer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // StartupMode, 0  = show detailed information, 1 = launch replay immediately
            if (args.Length == 0)
            {
                Application.Run(new SettingsForm());
            }
            else
            {
                if (RoflSettings.Default.StartupMode == 0)
                {
                    ReplayManager.StartReplay(args[0]);
                }
                else
                {
                    Application.Run(new DetailForm(args[0]));
                }
            }
        }
    }
}
