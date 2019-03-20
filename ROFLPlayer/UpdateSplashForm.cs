using ROFLPlayer.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROFLPlayer
{
    public partial class UpdateSplashForm : Form
    {
        private string TargetExecToUpdate = null;

        public UpdateSplashForm()
        {
            InitializeComponent();
        }

        public UpdateSplashForm(string targetExec)
        {
            TargetExecToUpdate = targetExec;
            InitializeComponent();
        }

        private async void UpdateSplashForm_Load(object sender, EventArgs e)
        {
            this.TitleLabel.Text = "Getting League of Legends executable...";
            await WaitDelay(100);

            // Get default exec, as default exec
            var targetExec = ExecsManager.GetExec(ExecsManager.GetDefaultExecName());

            // Choose exec to update if given
            if (!string.IsNullOrEmpty(TargetExecToUpdate))
            {
                var tempExec = ExecsManager.GetExec(TargetExecToUpdate);

                // target by that name does not exist, do not do anything and close the form
                if(tempExec == null)
                {
                    MessageBox.Show("Could not find executable with name: " + targetExec.Name + ". Please try again", "No Exec Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Abort;
                    this.Close();
                }
                else // set target to new exec
                {
                    targetExec = tempExec;
                }
            }

            this.LoadingProgressBar.Value = 20;
            this.TitleLabel.Text = "Checking executable...";
            await WaitDelay(100);

            // This should only happen if there is NO default exec, will be skipped on target exec
            if(targetExec == null)
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
                            this.DialogResult = DialogResult.Abort;
                            this.Close();
                        }

                        // Save using given path
                        ExecsManager.FindAndAddLeagueExec(installPath);
                    }
                    else // Some other error, besides finding install path, happened when trying to find exec
                    {
                        MessageBox.Show(result.Substring(result.IndexOf(':') + 1), "Exception occured", MessageBoxButtons.OK, MessageBoxIcon.Error); Environment.Exit(0);
                        this.DialogResult = DialogResult.Abort;
                        this.Close();
                    }
                }

                // Set new default exec we just found
                targetExec = ExecsManager.GetExec(ExecsManager.GetDefaultExecName());
            }

            this.LoadingProgressBar.Value = 40;
            this.TitleLabel.Text = "Checking if executable needs updating...";
            await WaitDelay(100);

            // Double check if target exists
            if (!File.Exists(targetExec.TargetPath))
            {
                // Check if entry allows updates if it doesn't exist
                if(targetExec.AllowUpdates)
                {
                    // Update exec path
                    var result = ExecsManager.UpdateLeaguePath(targetExec.Name);

                    // If update failed
                    if(result.StartsWith("FALSE"))
                    {
                        MessageBox.Show(result.Substring(result.IndexOf(':') + 1), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.Abort;
                        this.Close();
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
                    this.DialogResult = DialogResult.Abort;
                    this.Close();
                }
            }
            else
            {
                this.TitleLabel.Text = "Found League executable";
                this.LoadingProgressBar.Value = 100;
            }

            await WaitDelay(200);
            this.DialogResult = DialogResult.OK;
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
