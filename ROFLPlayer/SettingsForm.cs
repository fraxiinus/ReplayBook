using System;
using System.Windows.Forms;
using ROFLPlayer.Lib;
using System.Drawing;
using System.Linq;
using System.Collections;

namespace ROFLPlayer
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            this.GeneralGameTextBox.AutoSize = false;
            this.GeneralGameTextBox.Size = new System.Drawing.Size(218, 23);
            this.GeneralLaunchComboBox.AutoSize = false;
            this.GeneralLaunchComboBox.Size = new System.Drawing.Size(200, 23);
            this.GeneralUsernameTextBox.AutoSize = false;
            this.GeneralUsernameTextBox.Size = new Size(200, 23);
            this.GeneralLaunchComboBox.SelectedItem = this.GeneralLaunchComboBox.Items[RoflSettings.Default.StartupMode];
            this.GeneralUsernameTextBox.Text = RoflSettings.Default.Username;
            MainWindowManager.Load(this);
        }

        private void MainOkButton_Click(object sender, EventArgs e)
        {
            MainWindowManager.CloseWindow(this);
        }

        private void MainCancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void GeneralGameBrowseButton_Click(object sender, EventArgs e)
        {
            var filepath = MainWindowManager.ShowFileDialog();
            if(string.IsNullOrEmpty(filepath))
            {
                return;
            }

            if(LeagueManager.CheckLeagueExecutable(filepath))
            {
                this.GeneralGameTextBox.Text = filepath;
            }
            else
            {
                MessageBox.Show("Invalid League executable", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void GeneralGameClearButton_Click(object sender, EventArgs e)
        {
            this.GeneralGameTextBox.Text = string.Empty;
        }


        // DELETE ME!!!
        private void DeleteMe_Click(object sender, EventArgs e)
        {
            var result = ReplayManager.StartReplay(@"C:\Users\Anchu\Documents\League of Legends\Replays\NA1-2808559688.rofl");
            if(!result.Success)
            {
                MessageBox.Show(result.Message, "Error Starting Replay", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AboutGithubButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/andrew1421lee/ROFL-Player");
        }
    }
}
