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
            // Do sizing on objects
            this.GeneralGameTextBox.AutoSize = false;
            this.GeneralGameTextBox.Size = new System.Drawing.Size(200, 23);
            this.GeneralLaunchComboBox.AutoSize = false;
            this.GeneralLaunchComboBox.Size = new System.Drawing.Size(200, 23);
            this.GeneralUsernameTextBox.AutoSize = false;
            this.GeneralUsernameTextBox.Size = new Size(200, 23);
            
            MainWindowManager.Load(this);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.GeneralLaunchComboBox.SelectedItem = this.GeneralLaunchComboBox.Items[RoflSettings.Default.StartupMode];
            this.GeneralRegionComboBox.SelectedItem = RoflSettings.Default.Region;
            this.GeneralUsernameTextBox.Text = RoflSettings.Default.Username;
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
            var dialog = new OpenFileDialog();
            dialog.Filter = "Executable files (*.exe)|*.exe";
            dialog.Multiselect = false;
            dialog.Title = "Select League of Legends executable";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var filepath = dialog.FileName;
                if (string.IsNullOrEmpty(filepath))
                {
                    return;
                }

                if (LeagueManager.CheckLeagueExecutable(filepath))
                {
                    this.GeneralGameTextBox.Text = filepath;
                }
                else
                {
                    MessageBox.Show("Invalid League executable", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GeneralGameClearButton_Click(object sender, EventArgs e)
        {
            this.GeneralGameTextBox.Text = string.Empty;
        }

        private void AboutGithubButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/andrew1421lee/ROFL-Player");
        }
    }
}
