using System;
using System.Windows.Forms;
using ROFLPlayer.Utilities;
using ROFLPlayer.Managers;
using System.Drawing;
using System.IO;
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
            this.GeneralRegionComboBox.AutoSize = false;
            this.GeneralRegionComboBox.Size = new Size(200, 23);
            
            MainWindowManager.Load(this);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RoflSettings.Default.LoLExecLocation) || !File.Exists(RoflSettings.Default.LoLExecLocation))
            {
                if (GameLocator.FindLeagueInstallPath(out string path))
                {
                    try
                    {
                        var execPath = GameLocator.FindLeagueExecutable(path);
                        RoflSettings.Default.LoLExecLocation = path;
                        this.GeneralGameTextBox.Text = execPath;

                        MessageBox.Show("Automatically detected League of Legends install!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not find League of Legends executable, please select the game executable (League of Legends.exe)\n\n" + ex.Message, "Error finding game executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        GeneralGameBrowseButton_Click(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show("Could not find League of Legends install location, please select the game launcher (LeagueClient.exe)\n\n" + path + ".", "Error finding install path", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GeneralInstallBrowseButton_Click(this, new EventArgs());
                }
            }
            else
            {
                this.GeneralGameTextBox.Text = RoflSettings.Default.LoLExecLocation;
            }

            this.GeneralLaunchComboBox.SelectedItem = this.GeneralLaunchComboBox.Items[RoflSettings.Default.StartupMode];
            this.GeneralRegionComboBox.SelectedItem = RoflSettings.Default.Region;
            this.GeneralUsernameTextBox.Text = RoflSettings.Default.Username;
        }

        private void SettingsForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.MainTabControl.SelectedIndex == 1)
            {
                // Populate List of execs
                this.ExecItemsList.Items.Clear();
                this.ExecItemsList.Items.AddRange(ExecsManager.GetSavedExecs());
            }
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
            var dialog = new OpenFileDialog
            {
                Filter = "League of Legends.exe (*.exe)|*.exe",
                Multiselect = false,
                Title = "Select League of Legends executable"
            };
            while (dialog.ShowDialog() == DialogResult.OK)
            {
                var filepath = dialog.FileName;
                if (string.IsNullOrEmpty(filepath))
                {
                    return;
                }

                if (GameLocator.CheckLeagueExecutable(filepath))
                {
                    this.GeneralGameTextBox.Text = filepath;
                    return;
                }
                else
                {
                    MessageBox.Show("Invalid League executable", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GeneralInstallBrowseButton_Click(object sender, EventArgs e)
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
                    RoflSettings.Default.LoLExecLocation = path;
                    this.GeneralGameTextBox.Text = path;
                    return;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Could not find League of Legends executable, please try again\n\n" + ex.Message, "Error finding game executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ExecItemsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItemName = (string)this.ExecItemsList.SelectedItem;
            var selectedExec = ExecsManager.GetExec(selectedItemName);

            if (selectedExec == null) { return; }

            this.ExecDeleteButton.Enabled = true;

            // Update groupbox

            this.GBoxExecNameTextBox.Text = selectedExec.Name;
            this.GBoxTargetLocationTextBox.Text = selectedExec.TargetPath;
            this.GBoxPatchVersTextBox.Text = selectedExec.PatchVersion;
            this.GBoxLastModifTextBox.Text = selectedExec.ModifiedDate.ToString("yyyy/dd/MM");
        }

        private void ExecAddButton_Click(object sender, EventArgs e)
        {
            var addForm = new ExecAddForm();
            var formResult = addForm.ShowDialog();

            if(formResult == DialogResult.OK)
            {
                var newExec = addForm.NewLeagueExec;
                // Save execinfo file
                ExecsManager.SaveExecFile(newExec);
                this.ExecItemsList.Items.Add(newExec.Name);
            }
        }

        private void ExecDeleteButton_Click(object sender, EventArgs e)
        {
            var selectedName = (string)this.ExecItemsList.SelectedItem;

            var result = ExecsManager.DeleteExecFile(selectedName);

            if(result.StartsWith("FALSE"))
            {
                MessageBox.Show(result.Substring(result.IndexOf(':') + 1), "Error deleting entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            else if(result.StartsWith("TRUE"))
            {
                this.ExecItemsList.Items.Clear();
                this.ExecItemsList.Items.AddRange(ExecsManager.GetSavedExecs());


                this.GBoxExecNameTextBox.Text = "";
                this.GBoxTargetLocationTextBox.Text = "";
                this.GBoxPatchVersTextBox.Text = "";
                this.GBoxLastModifTextBox.Text = "";

                ExecDeleteButton.Enabled = false;
            }
        }

        private void GBoxSetDefaultCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
