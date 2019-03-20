using System;
using System.IO;
using System.Windows.Forms;
using ROFLPlayer.Models;
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

            //*/
            try
            {
                if (args.Length == 0)
                {
                    // Update default exec
                    Application.Run(new UpdateSplashForm());
                    Application.Run(new SettingsForm());
                }
                else
                {
                    // StartupMode, 1  = show detailed information, 0 = launch replay immediately

                    if (RoflSettings.Default.StartupMode == 0)
                    {
                        StartReplay(args[0]);
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

        private static void StartReplay(string replayPath, string execName = "default")
        {
            LeagueExecutable exec = null;

            // Get default exec or specified exec
            if (execName.Equals("default"))
            {
                // Start update form with default
                var result = new UpdateSplashForm().ShowDialog();

                if (result == DialogResult.OK)
                {
                    exec = ExecsManager.GetExec(ExecsManager.GetDefaultExecName());
                }
                else
                {
                    // Failed to get exec, stop
                    MessageBox.Show("Failed to start replay", $"Could not find executable data {execName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                // Start update form with target
                var result = new UpdateSplashForm(execName).ShowDialog();

                if (result == DialogResult.OK)
                {
                    exec = ExecsManager.GetExec(execName);
                }
                else
                {
                    // Failed to get exec, stop
                    MessageBox.Show("Failed to start replay", $"Could not find executable data {execName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (exec == null)
            {
                MessageBox.Show("Failed to start replay", $"Could not find executable data {execName}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            ReplayManager.StartReplay(replayPath, exec.TargetPath);
        }
    }
}
