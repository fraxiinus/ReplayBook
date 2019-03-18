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
            this.LoadingProgressBar.Value = 50;

            await WaitDelay(300);

            // Double check if target exists
            if(!File.Exists(defaultExec.TargetPath))
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
                    MessageBox.Show("Error", "League executable could not be found, entry does not allow updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
