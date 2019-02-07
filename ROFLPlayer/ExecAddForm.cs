using ROFLPlayer.Models;
using ROFLPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROFLPlayer
{
    public partial class ExecAddForm : Form
    {

        private LeagueExecutable leagueExecutable;

        public ExecAddForm()
        {
            InitializeComponent();
            InitForm();
            leagueExecutable = new LeagueExecutable();
        }

        private void InitForm()
        {
            this.ExecNameTextBox.AutoSize = false;
            this.ExecNameTextBox.Size = new Size(245, 23);

            this.ExecTargetTextBox.AutoSize = false;
            this.ExecTargetTextBox.Size = new Size(245, 23);

            this.ExecStartTextBox.AutoSize = false;
            this.ExecStartTextBox.Size = new Size(245, 23);
        }

        private void ExecBrowseButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "LeagueClient.exe (*.exe)|*.exe",
                Multiselect = false,
                Title = "Select League of Legends client"
            };
            while (dialog.ShowDialog() == DialogResult.OK)
            {
                var filepath = dialog.FileName;
                if (string.IsNullOrEmpty(filepath))
                {
                    return;
                }

                try
                {
                    var path = GameLocator.FindLeagueExecutable(Path.GetDirectoryName(filepath));
                    //RoflSettings.Default.LoLExecLocation = path;
                    this.ExecTargetTextBox.Text = path;
                    this.ExecStartTextBox.Text = Path.GetDirectoryName(filepath);
                    leagueExecutable.TargetPath = path;
                    leagueExecutable.StartFolder = Path.GetDirectoryName(filepath);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not find League of Legends executable, please try again\n\n" + ex.Message, "Error finding game executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
