using System;
using System.Windows.Forms;
using Rofl.Main.Utilities;
using System.Drawing;
using Rofl.Executables;
using Rofl.Executables.Models;
using System.Linq;

namespace Rofl.Main
{
    public partial class SettingsForm : Form
    {
        private readonly ExeManager _exeManager;

        public SettingsForm(ExeManager exeManager)
        {
            _exeManager = exeManager;

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

            string[] exeNames = (from exe in _exeManager.GetExecutables()
                                 select exe.Name).ToArray();

            this.ExecItemsList.Items.Add(_exeManager.GetDefaultExecutable().Name);

            this.ExecItemsList.Items.AddRange(exeNames);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // Set version text in about tab
            this.AboutVersionLabel.Text = RoflSettings.Default.VersionString;

            // Load saved executable entries for combo box
            string[] exeNames = (from exe in _exeManager.GetExecutables()
                                 select exe.Name).ToArray();

            this.GeneralGameComboBox.Items.Add(_exeManager.GetDefaultExecutable().Name);
            this.GeneralGameComboBox.Items.AddRange(exeNames);

            // Restore saved default entry
            string selectedItem = _exeManager.GetDefaultExecutable().Name;
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
                string[] exeNames = (from exe in _exeManager.GetExecutables()
                                     select exe.Name).ToArray();

                this.GeneralGameComboBox.Items.Add(_exeManager.GetDefaultExecutable().Name);
                this.GeneralGameComboBox.Items.AddRange(exeNames);

                // Select saved item by name
                string selectedItem = _exeManager.GetDefaultExecutable().Name;
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
            if(this.GeneralGameComboBox.SelectedItem != null)
            {
                string defaultExeName = this.GeneralGameComboBox.SelectedItem.ToString();

                LeagueExecutable leagueExecutable = _exeManager.GetExecutable(defaultExeName);

                _exeManager.SetDefaultExectuable(this.GeneralGameComboBox.SelectedItem.ToString());
            }
            _exeManager.Save();

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

            LeagueExecutable selectedExec = _exeManager.GetExecutable(selectedItemName);

            // Nothing? Weird, just return
            if (selectedExec == null) { return; }

            // Enable selected context buttons
            this.ExecDeleteButton.Enabled = true;
            this.ExecEditButton.Enabled = true;

            // Update groupbox
            this.GBoxExecNameTextBox.Text = selectedExec.Name;
            this.GBoxTargetLocationTextBox.Text = selectedExec.TargetPath;
            this.GBoxPatchVersTextBox.Text = selectedExec.PatchVersion;
            this.GBoxLastModifTextBox.Text = selectedExec.ModifiedDate.ToString("yyyy/MM/dd");
            this.GBoxAllowUpdatesTextBox.Text = selectedExec.AllowUpdates.ToString();
        }

        /// <summary>
        /// Actions for add exec button
        /// </summary>
        private void ExecAddButton_Click(object sender, EventArgs e)
        {
            // Start add exec form
            var addForm = new ExecAddForm(_exeManager.ExeTools);
            var formResult = addForm.ShowDialog();

            // If form exited with ok
            if(formResult == DialogResult.OK)
            {
                // Get new exec
                var newExec = addForm.NewLeagueExec;

                // Save execinfo file
                _exeManager.AddExecutable(newExec);

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
            try
            {
                _exeManager.DeleteExecutable(selectedName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting entry\n{ex.GetType().ToString()} - {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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

        /// <summary>
        /// Actions for edit exec button, also called on double click action
        /// </summary>
        private void ExecEditButton_Click(object sender, EventArgs e)
        {
            // Get exec that is selected
            var selectedName = (string)this.ExecItemsList.SelectedItem;

            LeagueExecutable exec = _exeManager.GetExecutable(selectedName);

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
                    if(!newExec.IsDefault)
                    {
                        _exeManager.DeleteExecutable(selectedName);
                        _exeManager.AddExecutable(newExec);
                    }

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
