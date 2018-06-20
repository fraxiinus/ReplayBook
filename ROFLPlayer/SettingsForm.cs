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

            this.GeneralGameTextBox.Text = filepath;
        }
    }
}
