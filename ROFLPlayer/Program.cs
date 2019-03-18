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

            Application.Run(new UpdateSplashForm());

            //*/
            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show(@"ROFLPlayer encountered an unhandled exception, please record this message and report it here https://github.com/andrew1421lee/ROFL-Player/issues" + "\n\n" + ex.ToString() + "\n" + ex.Source, "Critical Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            //*/
        }
    }
}
