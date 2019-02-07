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
                // Filter out only exes
                Filter = "LeagueClient.exe or League of Legends.exe (*.exe)|*.exe",
                // Show only files starting with "League"
                FileName = "League*",
                // Only allow one file to be selected
                Multiselect = false,
                // Set title of dialog
                Title = "Select League of Legends client"
            };
            // Wait for user to press ok
            while (dialog.ShowDialog() == DialogResult.OK)
            {
                var filepath = dialog.FileName;
                // They didn't select anything, do nothing
                if (string.IsNullOrEmpty(filepath) || filepath.Equals("League*"))
                {
                    return;
                }

                string gamePath = "";
                // Now did they select leagueclient or league of legends?
                switch (Path.GetFileName(filepath))
                {
                    case "LeagueClient.exe":
                        {
                            ExecUpdateCheckbox.Enabled = true;
                            // Find the league of legends.exe using game locator
                            try
                            {
                                gamePath = GameLocator.FindLeagueExecutable(Path.GetDirectoryName(filepath));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not find League of Legends executable, please try again\n\n" + ex.Message, "Error finding game executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        }
                    case "League of Legends.exe":
                        {
                            ExecUpdateCheckbox.Checked = false;
                            ExecUpdateCheckbox.Enabled = false;
                            gamePath = filepath;
                            break;
                        }
                }

                this.ExecTargetTextBox.Text = gamePath;
                this.ExecStartTextBox.Text = Path.GetDirectoryName(filepath);
                leagueExecutable.TargetPath = gamePath;
                leagueExecutable.StartFolder = Path.GetDirectoryName(filepath);
                return;
            }
        }

        private void ExecCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
