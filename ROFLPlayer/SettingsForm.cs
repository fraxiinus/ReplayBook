using System;
using System.Windows.Forms;
using ROFLPlayer.Utilities;
using System.Drawing;

namespace ROFLPlayer
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            // Do sizing on objects
            this.GeneralGameComboBox.AutoSize = false;
            this.GeneralGameComboBox.Size = new System.Drawing.Size(200, 23);
            this.GeneralLaunchComboBox.AutoSize = false;
            this.GeneralLaunchComboBox.Size = new System.Drawing.Size(200, 23);
            this.GeneralUsernameTextBox.AutoSize = false;
            this.GeneralUsernameTextBox.Size = new Size(200, 23);
            this.GeneralRegionComboBox.AutoSize = false;
            this.GeneralRegionComboBox.Size = new Size(200, 23);
        }

        /// <summary>
        /// Refresh execs list
        /// </summary>
        private void RefreshExecListBox()
        {
            this.ExecItemsList.Items.Clear();
            this.ExecItemsList.Items.AddRange(ExecsManager.GetSavedExecs());
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            /// THIS BLOCK OF CODE IS FOR AUTOMATICALLY UPDATING THE EXECUTABLE LOCATION
            // TODO MOVE THIS SOMEWHERE ELSE LOL
            /*
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
            } */
            ///

            // Load saved executable entries for combo box
            this.GeneralGameComboBox.Items.AddRange(ExecsManager.GetSavedExecs());

            // Restore saved default entry
            var selectedItem = ExecsManager.GetDefaultExecName();
            if(selectedItem != null)
            {
                this.GeneralGameComboBox.SelectedItem = selectedItem;
            }

            // Restore saved settings
            this.GeneralLaunchComboBox.SelectedItem = this.GeneralLaunchComboBox.Items[RoflSettings.Default.StartupMode];
            this.GeneralRegionComboBox.SelectedItem = RoflSettings.Default.Region;
            this.GeneralUsernameTextBox.Text = RoflSettings.Default.Username;
        }

        /// <summary>
        /// Actions for when main tab changes
        /// </summary>
        private void SettingsForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If tab is now on executables
            if (this.MainTabControl.SelectedIndex == 1)
            {
                // Populate List of execs
                RefreshExecListBox();
                this.ExecDeleteButton.Enabled = false;
                this.ExecEditButton.Enabled = false;
            }
            // If tab is now on general
            else if (this.MainTabControl.SelectedIndex == 0)
            {
                // Populate List of execs
                this.GeneralGameComboBox.Items.Clear();
                this.GeneralGameComboBox.Items.AddRange(ExecsManager.GetSavedExecs());

                // Select saved item by name
                var selectedItem = ExecsManager.GetDefaultExecName();
                if (selectedItem != null)
                {
                    this.GeneralGameComboBox.SelectedItem = selectedItem;
                }
            }
        }

        /// <summary>
        /// Actions for ok button
        /// </summary>
        private void MainOkButton_Click(object sender, EventArgs e)
        {
            // Save selected default exec
            ExecsManager.SetDefaultExecByName(this.GeneralGameComboBox.SelectedItem.ToString());

            // Save double click launch option
            RoflSettings.Default.StartupMode = this.GeneralLaunchComboBox.SelectedIndex;

            // Save username
            RoflSettings.Default.Username = this.GeneralUsernameTextBox.Text;

            // Save region
            RoflSettings.Default.Region = this.GeneralRegionComboBox.Text;

            // Save config
            RoflSettings.Default.Save();
            Environment.Exit(1);
        }

        /// <summary>
        /// Actions for cancel button
        /// </summary>
        private void MainCancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        /*
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
        */

        /*
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
        */

        /*
        private void GeneralGameClearButton_Click(object sender, EventArgs e)
        {
            this.GeneralGameTextBox.Text = string.Empty;
        }
        */

        /// <summary>
        /// Actions for github button
        /// </summary>
        private void AboutGithubButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/leeanchu/ROFL-Player");
        }

        /// <summary>
        /// Actions for selecting exec
        /// </summary>
        private void ExecItemsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get exec that is selected
            var selectedItemName = (string)this.ExecItemsList.SelectedItem;
            var selectedExec = ExecsManager.GetExec(selectedItemName);

            // Nothing? Weird, just return
            if (selectedExec == null) { return; }

            // Enable selected context buttons
            this.ExecDeleteButton.Enabled = true;
            this.ExecEditButton.Enabled = true;

            // Update groupbox
            this.GBoxExecNameTextBox.Text = selectedExec.Name;
            this.GBoxTargetLocationTextBox.Text = selectedExec.TargetPath;
            this.GBoxPatchVersTextBox.Text = selectedExec.PatchVersion;
            this.GBoxLastModifTextBox.Text = selectedExec.ModifiedDate.ToString("yyyy/dd/MM");
            this.GBoxAllowUpdatesTextBox.Text = selectedExec.AllowUpdates.ToString();
        }

        /// <summary>
        /// Actions for add exec button
        /// </summary>
        private void ExecAddButton_Click(object sender, EventArgs e)
        {
            // Start add exec form
            var addForm = new ExecAddForm();
            var formResult = addForm.ShowDialog();

            // If form exited with ok
            if(formResult == DialogResult.OK)
            {
                // Get new exec
                var newExec = addForm.NewLeagueExec;

                // Save execinfo file
                ExecsManager.SaveExecFile(newExec);

                // Add to exec items list
                RefreshExecListBox();
            }
        }

        /// <summary>
        /// Actions for delete exec button
        /// </summary>
        private void ExecDeleteButton_Click(object sender, EventArgs e)
        {
            // Get exec that is selected
            var selectedName = (string)this.ExecItemsList.SelectedItem;

            // Attempt to delete
            var result = ExecsManager.DeleteExecFile(selectedName);

            // Check for error/result
            if(result.StartsWith("FALSE"))
            {
                MessageBox.Show(result.Substring(result.IndexOf(':') + 1), "Error deleting entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            else if(result.StartsWith("TRUE"))
            {
                // Repopulate list
                RefreshExecListBox();

                // Reset info box
                this.GBoxExecNameTextBox.Text = "";
                this.GBoxTargetLocationTextBox.Text = "";
                this.GBoxPatchVersTextBox.Text = "";
                this.GBoxLastModifTextBox.Text = "";

                // turn off context buttons
                ExecDeleteButton.Enabled = false;
                ExecEditButton.Enabled = false;
            }
        }

        /// <summary>
        /// Actions for edit exec button, also called on double click action
        /// </summary>
        private void ExecEditButton_Click(object sender, EventArgs e)
        {
            // Get exec that is selected
            var selectedName = (string)this.ExecItemsList.SelectedItem;
            var exec = ExecsManager.GetExec(selectedName);

            // Check is gotten exec
            if(exec == null)
            {
                // This happens when nothing is selected
                //MessageBox.Show("Specified entry does not exist. Delete and re-add", "Error reading entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Start add form, but in edit mode
                var editForm = new ExecAddForm(exec);
                var formResult = editForm.ShowDialog();

                // if form exited with ok
                if (formResult == DialogResult.OK)
                {
                    // Get edited exec
                    var newExec = editForm.NewLeagueExec;

                    // Save exec file
                    ExecsManager.DeleteExecFile(selectedName);
                    ExecsManager.SaveExecFile(newExec);

                    // Refresh list of execs
                    RefreshExecListBox();

                    // Clear info box
                    this.GBoxExecNameTextBox.Text = "";
                    this.GBoxTargetLocationTextBox.Text = "";
                    this.GBoxPatchVersTextBox.Text = "";
                    this.GBoxLastModifTextBox.Text = "";

                    // disable context buttons
                    ExecDeleteButton.Enabled = false;
                    ExecEditButton.Enabled = false;
                }
            }
        }
    }
}
