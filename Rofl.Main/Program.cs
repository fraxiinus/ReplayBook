using Rofl.Executables;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Reader;
using Rofl.Reader.Models;
using Rofl.Requests;
using Rofl.Logger;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rofl.Main
{
    static class Program
    {
        private static string _className = "Program";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread] // This is required for system dialogs
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Scribe logger = new Scribe();

            //*/
            try
            {
                ExeManager exeManager = null;
                try
                {
                    exeManager = new ExeManager();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ROFLPlayer was not able to find League of Legends. Please add it now.", "First time setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    logger.Info(_className, $"First time setup kicked off by exception:\n{ex.ToString()}");
                    // Start add exec form
                    var addForm = new ExecAddForm(new ExeTools());
                    var formResult = addForm.ShowDialog();

                    // If form exited with ok
                    if (formResult == DialogResult.OK)
                    {

                        // Get new exec
                        LeagueExecutable newExec = addForm.NewLeagueExec;
                        newExec.IsDefault = true;

                        // Save execinfo file
                        exeManager = new ExeManager(newExec);
                        exeManager.Save();
                    }
                    else
                    {
                        // Exit if form exited any other way
                        Environment.Exit(1);
                    }

                    addForm.Dispose();
                }

                ReplayPlayer replayPlayer = new ReplayPlayer(exeManager);

                if (args.Length == 0)
                {
                    // Update default exec
                    Application.Run(new UpdateSplashForm(exeManager));
                    Application.Run(new SettingsForm(exeManager));
                }
                else
                {

                    // StartupMode, 1  = show detailed information, 0 = launch replay immediately
                    if (RoflSettings.Default.StartupMode == 0)
                    {
                        StartReplay(args[0], exeManager, replayPlayer);
                    }
                    else
                    {
                        var replayFile = Task.Run(() => SetupReplayFileAsync(args[0]));

                        replayFile.Wait();

                        RequestManager requestManager = new RequestManager();

                        Application.Run(new DetailForm(replayFile.Result, requestManager, exeManager, replayPlayer));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"ROFLPlayer encountered an unhandled exception, please record this message and report it here https://github.com/andrew1421lee/ROFL-Player/issues" + "\n\n" + ex.ToString(), "Critical Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(_className, "Unhandled exception: " + ex.ToString());
                Environment.Exit(1);
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
                    MessageBox.Show($"{fileInfo.Name} is a old LoLReplay file.\nROFLPlayer will try to open this file in compatibility mode, some data or features may be missing.", "Compatibility Mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fileInfo.Type = REPLAYTYPES.LRF;
                    break;
                case ".lpr":
                    MessageBox.Show($"{fileInfo.Name} is a old BaronReplay file. ROFLPlayer does not support opening this file.", "Compatibility Mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fileInfo.Type = REPLAYTYPES.LPR;
                    Environment.Exit(1);
                    break;
                default:
                    MessageBox.Show($"{fileInfo.Name} is not a supported file type", "Unsupported File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
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
                Environment.Exit(1);
            }

            return fileInfo;
        }

        private static void StartReplay(string replayPath, ExeManager exeManager, ReplayPlayer replayPlayer, string execName = "default")
        {
            LeagueExecutable exec = null;

            // Get default exec or specified exec
            if (execName.Equals("default"))
            {
                // Start update form with default
                var result = new UpdateSplashForm(exeManager).ShowDialog();

                if (result == DialogResult.OK)
                {
                    exec = exeManager.GetDefaultExecutable();
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
                    exec = exeManager.GetExecutable(execName);
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

            replayPlayer.Play(exec, replayPath);
        }
    }
}
