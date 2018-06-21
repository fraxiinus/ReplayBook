using System;
using System.Windows.Forms;
using ROFLPlayer.Lib;

namespace ROFLPlayer
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            this.GeneralGameTextBox.AutoSize = false;
            this.GeneralGameTextBox.Size = new System.Drawing.Size(234, 23);
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

        private void DeleteMe_Click(object sender, EventArgs e)
        {
            var result = ReplayManager.StartReplay(@"C:\Users\Anchu\Documents\League of Legends\Replays\NA1-2808559688.rofl");
            if(!result.Success)
            {
                MessageBox.Show(result.Message, "Error Starting Replay", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
