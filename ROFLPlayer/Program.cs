using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rofl.Parsers;
using Rofl.Parsers.Models;
using ROFLPlayer.Models;
using ROFLPlayer.Utilities;

namespace ROFLPlayer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread] // This is required for system dialogs
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
                        var replayFile = Task.Run(() => SetupReplayFileAsync(args[0]));

                        replayFile.Wait();

                        Application.Run(new DetailForm(replayFile.Result));
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

        /// <summary>
        /// Given replay path, construct ReplayFile with all properties initialized
        /// </summary>
        /// <param name="replayPath"></param>
        /// <returns></returns>
        private static async Task<ReplayFile> SetupReplayFileAsync(string replayPath)
        {
            var fileInfo = new ReplayFile
            {
                Location = replayPath,
                Name = Path.GetFileName(replayPath),
            };

            switch (Path.GetExtension(replayPath))
            {
                case ".rofl":
                    fileInfo.Type = REPLAYTYPES.ROFL;
                    break;
                case ".lrf":
                    MessageBox.Show($"{fileInfo.Name} is a old LoLReplay file. ROFLPlayer will only be able to show you basic match metadata.", "Compatibility Mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fileInfo.Type = REPLAYTYPES.LRF;
                    break;
                case ".lpr":
                    MessageBox.Show($"{fileInfo.Name} is a old BaronReplay file. ROFLPlayer will only be able to show you basic match metadata.", "Compatibility Mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fileInfo.Type = REPLAYTYPES.LPR;
                    break;
                default:
                    MessageBox.Show($"{fileInfo.Name} is not a supported file type", "Unsupported File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    break;
            }

            var replayReader = new ReplayReader();
            
            try
            {
                fileInfo = await replayReader.ReadFile(fileInfo);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Exception occured when parsing file:\n{ex.Message}", "Parsing Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            return fileInfo;
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
