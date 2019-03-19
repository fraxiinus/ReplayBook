using ROFLPlayer.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROFLPlayer
{
    public partial class UpdateSplashForm : Form
    {
        public UpdateSplashForm()
        {
            InitializeComponent();
        }

        private async void UpdateSplashForm_Load(object sender, EventArgs e)
        {
            // Get default exec
            var defaultExec = ExecsManager.GetExec(ExecsManager.GetDefaultExecName());
            this.LoadingProgressBar.Value = 20;

            await WaitDelay(300);

            // No default exec??? Find one
            if(defaultExec == null)
            {
                MessageBox.Show("Could not find any executables saved, ROFL Player will try to automatically locate League of Legends", "No Default Exec Found", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Result should be the name of the new exec, will be "false" if something happens
                var result = ExecsManager.FindAndAddLeagueExec();

                // Ok, failed to find exec
                if(result.StartsWith("FALSE"))
                {
                    // Failed to find install path, have user find it!
                    if(result.Contains("Could not find install path"))
                    {
                        // show browse dialog
                        MessageBox.Show("ROFL Player could not find League of Legends install folder, please select it", "Could not find install folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        var installPath = BrowseDialog();

                        // Check if result is empty
                        if(string.IsNullOrEmpty(installPath))
                        {
                            MessageBox.Show("Invalid install folder", "Could not find install folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }

                        // Save using given path
                        ExecsManager.FindAndAddLeagueExec(installPath);
                    }
                    else
                    {
                        MessageBox.Show("Exception occured", result.Substring(result.IndexOf(':') + 1), MessageBoxButtons.OK, MessageBoxIcon.Error); Environment.Exit(0);
                        Environment.Exit(0);
                    }
                }

                // Set new default exec
                defaultExec = ExecsManager.GetExec(ExecsManager.GetDefaultExecName());
            }

            // Double check if target exists
            if (!File.Exists(defaultExec.TargetPath))
            {
                // Check if entry allows updates
                if(defaultExec.AllowUpdates)
                {
                    // Update exec path
                    var result = ExecsManager.UpdateLeaguePath(defaultExec.Name);

                    // If update failed
                    if(result.StartsWith("FALSE"))
                    {
                        MessageBox.Show("Error", result.Substring(result.IndexOf(':') + 1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // If update worked
                    else
                    {
                        this.TitleLabel.Text = "Successfully updated League executable";
                        this.LoadingProgressBar.Value = 100;
                    }
                }
                // Entry does not allow updates
                else
                {
                    MessageBox.Show("League executable could not be found, entry does not allow updating", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.TitleLabel.Text = "Found League executable";
                this.LoadingProgressBar.Value = 100;
            }

            await WaitDelay(300);
            this.Close();
        }

        private async Task WaitDelay(int millis)
        {
            await Task.Delay(millis);
        }

        private string BrowseDialog()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "LeagueClient.exe (*.exe)|*.exe",
                Multiselect = false,
                Title = "Select League of Legends client",
                FileName = "LeagueClient.exe"
            };

            while (dialog.ShowDialog() == DialogResult.OK)
            {
                var filepath = dialog.FileName;
                if (string.IsNullOrEmpty(filepath))
                {
                    return null;
                }

                try
                {
                    var path = GameLocator.FindLeagueExecutable(Path.GetDirectoryName(filepath));
                    //RoflSettings.Default.LoLExecLocation = path;
                    if(!string.IsNullOrEmpty(path))
                    {
                        return Path.GetDirectoryName(filepath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not find League of Legends executable, please try again\n\n" + ex.Message, "Error finding game executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return null;
        }
    }
}
