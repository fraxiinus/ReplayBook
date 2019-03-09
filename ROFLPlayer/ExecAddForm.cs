using ROFLPlayer.Models;
using ROFLPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        public LeagueExecutable NewLeagueExec { get; }

        private ToolTip toolTip;

        public ExecAddForm()
        {
            InitializeComponent();
            InitForm();
            NewLeagueExec = new LeagueExecutable();
            toolTip = new ToolTip();
        }

        public ExecAddForm(LeagueExecutable leagueExecutable)
        {
            InitializeComponent();
            InitForm();
            toolTip = new ToolTip();

            NewLeagueExec = leagueExecutable;
            this.ExecTargetTextBox.Text = NewLeagueExec.TargetPath;
            this.ExecStartTextBox.Text = NewLeagueExec.StartFolder;
            this.GBoxExecNameTextBox.Text = Path.GetFileName(NewLeagueExec.TargetPath);
            this.GBoxPatchVersTextBox.Text = NewLeagueExec.PatchVersion;
            this.GBoxFileDescTextBox.Text = "League of Legends(TM) Client";
            this.GBoxLastModifTextBox.Text = NewLeagueExec.ModifiedDate.ToString("yyyy/dd/MM");
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

        private bool ValidateForm()
        {
            var toolTip = new ToolTip();
            if (String.IsNullOrEmpty(this.ExecNameTextBox.Text))
            {
                toolTip.Show("Required Field", this.ExecNameTextBox, 0, 20, 3000);
                return false;
            }

            if(String.IsNullOrEmpty(this.ExecTargetTextBox.Text))
            {
                toolTip.Show("Required Field", this.ExecBrowseButton, 0, 20, 3000);
                return false;
            }

            if(this.GBoxFileDescTextBox.Text == "League of Legends(TM) Client")
            {
                toolTip.Show("File does not match League of Legends", this.GBoxFileDescTextBox, 0, 20, 3000);
                return false;
            }

            return true;
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
                            // Enable update checkbox if ever disabled
                            ExecUpdateCheckbox.Enabled = true;
                            try
                            {
                                // Find the league of legends.exe using game locator
                                gamePath = GameLocator.FindLeagueExecutable(Path.GetDirectoryName(filepath));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not find League of Legends executable, please try again\n\n" + ex.Message, "Error finding game executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            break;
                        }
                    case "League of Legends.exe":
                        {
                            // Disable update checkbox, could cause big problems
                            ExecUpdateCheckbox.Checked = false;
                            ExecUpdateCheckbox.Enabled = false;
                            gamePath = filepath;
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("Selected file is not LeagueClient.exe or League of Legends.exe", "Invalid executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                }


                var fileInfo = FileVersionInfo.GetVersionInfo(gamePath);

                this.ExecTargetTextBox.Text = gamePath;
                this.ExecStartTextBox.Text = Path.GetDirectoryName(filepath);
                this.GBoxExecNameTextBox.Text = Path.GetFileName(gamePath);
                this.GBoxPatchVersTextBox.Text = fileInfo.FileVersion;
                this.GBoxFileDescTextBox.Text = fileInfo.FileDescription;
                this.GBoxLastModifTextBox.Text = NewLeagueExec.ModifiedDate.ToString("yyyy/dd/MM");

                NewLeagueExec.TargetPath = gamePath;
                NewLeagueExec.StartFolder = Path.GetDirectoryName(filepath);
                NewLeagueExec.PatchVersion = fileInfo.FileVersion;
                NewLeagueExec.ModifiedDate = File.GetLastWriteTime(gamePath);

                return;
            }
        }

        private void ExecCancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ExecUpdateCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            NewLeagueExec.AllowUpdates = this.ExecUpdateCheckbox.Checked;
        }

        private void ExecUpdateCheckbox_ToolTip(object sender, EventArgs e)
        {
            CheckBox updateBox = (CheckBox)sender;

            var visTime = 3000;

            toolTip.Show("ROFLPlayer can automatically update target path when League of Legends updates", updateBox, 0, 20, visTime);
        }

        private void ExecSaveButton_Click(object sender, EventArgs e)
        {
            if(!ValidateForm())
            {
                return;
            }

            NewLeagueExec.Name = this.ExecNameTextBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
