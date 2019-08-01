using Rofl.Executables;
using Rofl.Executables.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rofl.Main
{
    public partial class UpdateSplashForm : Form
    {
        private string TargetExecToUpdate = null;
        private ExeManager _exeManager;

        public UpdateSplashForm(ExeManager exeManager)
        {
            _exeManager = exeManager;
            InitializeComponent();
        }

        public UpdateSplashForm(ExeManager exeManager, string targetExec)
        {
            _exeManager = exeManager;
            TargetExecToUpdate = targetExec;
            InitializeComponent();
        }

        private async void UpdateSplashForm_Load(object sender, EventArgs e)
        {
            this.TitleLabel.Text = "Getting League of Legends executable...";
            await WaitDelay(100);

            // Get default exec, as default exec
            LeagueExecutable targetExec = _exeManager.GetDefaultExecutable();

            // Choose exec to update if given
            if (!string.IsNullOrEmpty(TargetExecToUpdate))
            {
                //var tempExec = ExecsManager.GetExec(TargetExecToUpdate);
                var tempExec = _exeManager.GetExecutable(TargetExecToUpdate);

                // target by that name does not exist, do not do anything and close the form
                if(tempExec == null)
                {
                    MessageBox.Show("Could not find executable with name: " + targetExec.Name + ". Please try again", "No Exec Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Abort;
                    this.Close();
                    return;
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
            // Should pretty much NEVER happen now with new ExeManager
            if(targetExec == null)
            {
                MessageBox.Show("Could not find any executables saved, ROFL Player was not able to find League of Legends", "No Default Exec Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
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
                    try
                    {
                        _exeManager.UpdateExecutableTarget(targetExec.Name);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.GetType().ToString()} - {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }

                    this.TitleLabel.Text = "Successfully updated League executable";
                    this.LoadingProgressBar.Value = 100;

                }
                // Entry does not allow updates
                else
                {
                    MessageBox.Show("League executable could not be found, entry does not allow updating", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Abort;
                    this.Close();
                    return;
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
    }
}
