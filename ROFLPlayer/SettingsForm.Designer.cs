namespace ROFLPlayer
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.settingsTabControl = new System.Windows.Forms.TabControl();
            this.MainTab = new System.Windows.Forms.TabPage();
            this.AboutTab = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.GeneralGameLabel = new System.Windows.Forms.Label();
            this.GeneralGameTextBox = new System.Windows.Forms.TextBox();
            this.GeneralGameBrowseButton = new System.Windows.Forms.Button();
            this.MainPlayButton = new System.Windows.Forms.Button();
            this.MainCancelButton = new System.Windows.Forms.Button();
            this.MainOkButton = new System.Windows.Forms.Button();
            this.settingsTabControl.SuspendLayout();
            this.MainTab.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsTabControl
            // 
            this.settingsTabControl.Controls.Add(this.MainTab);
            this.settingsTabControl.Controls.Add(this.AboutTab);
            this.settingsTabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.settingsTabControl.Location = new System.Drawing.Point(5, 5);
            this.settingsTabControl.Name = "settingsTabControl";
            this.settingsTabControl.SelectedIndex = 0;
            this.settingsTabControl.Size = new System.Drawing.Size(348, 408);
            this.settingsTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.settingsTabControl.TabIndex = 0;
            // 
            // MainTab
            // 
            this.MainTab.Controls.Add(this.flowLayoutPanel1);
            this.MainTab.Location = new System.Drawing.Point(4, 22);
            this.MainTab.Name = "MainTab";
            this.MainTab.Padding = new System.Windows.Forms.Padding(3);
            this.MainTab.Size = new System.Drawing.Size(340, 382);
            this.MainTab.TabIndex = 1;
            this.MainTab.Text = "General";
            this.MainTab.UseVisualStyleBackColor = true;
            // 
            // AboutTab
            // 
            this.AboutTab.Location = new System.Drawing.Point(4, 22);
            this.AboutTab.Name = "AboutTab";
            this.AboutTab.Padding = new System.Windows.Forms.Padding(3);
            this.AboutTab.Size = new System.Drawing.Size(410, 414);
            this.AboutTab.TabIndex = 2;
            this.AboutTab.Text = "About";
            this.AboutTab.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.GeneralGameLabel);
            this.flowLayoutPanel1.Controls.Add(this.GeneralGameTextBox);
            this.flowLayoutPanel1.Controls.Add(this.GeneralGameBrowseButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(334, 376);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // GeneralGameLabel
            // 
            this.GeneralGameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameLabel.AutoSize = true;
            this.GeneralGameLabel.Location = new System.Drawing.Point(3, 5);
            this.GeneralGameLabel.Name = "GeneralGameLabel";
            this.GeneralGameLabel.Size = new System.Drawing.Size(127, 13);
            this.GeneralGameLabel.TabIndex = 0;
            this.GeneralGameLabel.Text = "League of Legends Path:";
            // 
            // GeneralGameTextBox
            // 
            this.GeneralGameTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GeneralGameTextBox.Location = new System.Drawing.Point(3, 22);
            this.GeneralGameTextBox.Name = "GeneralGameTextBox";
            this.GeneralGameTextBox.Size = new System.Drawing.Size(234, 23);
            this.GeneralGameTextBox.TabIndex = 1;
            // 
            // GeneralGameBrowseButton
            // 
            this.GeneralGameBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameBrowseButton.Location = new System.Drawing.Point(243, 21);
            this.GeneralGameBrowseButton.Name = "GeneralGameBrowseButton";
            this.GeneralGameBrowseButton.Size = new System.Drawing.Size(88, 25);
            this.GeneralGameBrowseButton.TabIndex = 2;
            this.GeneralGameBrowseButton.Text = "Browse...";
            this.GeneralGameBrowseButton.UseVisualStyleBackColor = true;
            // 
            // MainPlayButton
            // 
            this.MainPlayButton.Location = new System.Drawing.Point(275, 419);
            this.MainPlayButton.Name = "MainPlayButton";
            this.MainPlayButton.Size = new System.Drawing.Size(75, 23);
            this.MainPlayButton.TabIndex = 1;
            this.MainPlayButton.Text = "Play";
            this.MainPlayButton.UseVisualStyleBackColor = true;
            // 
            // MainCancelButton
            // 
            this.MainCancelButton.Location = new System.Drawing.Point(194, 419);
            this.MainCancelButton.Name = "MainCancelButton";
            this.MainCancelButton.Size = new System.Drawing.Size(75, 23);
            this.MainCancelButton.TabIndex = 2;
            this.MainCancelButton.Text = "Cancel";
            this.MainCancelButton.UseVisualStyleBackColor = true;
            // 
            // MainOkButton
            // 
            this.MainOkButton.Location = new System.Drawing.Point(113, 419);
            this.MainOkButton.Name = "MainOkButton";
            this.MainOkButton.Size = new System.Drawing.Size(75, 23);
            this.MainOkButton.TabIndex = 3;
            this.MainOkButton.Text = "OK";
            this.MainOkButton.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 450);
            this.Controls.Add(this.MainOkButton);
            this.Controls.Add(this.MainCancelButton);
            this.Controls.Add(this.MainPlayButton);
            this.Controls.Add(this.settingsTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ROFL Player Settings";
            this.settingsTabControl.ResumeLayout(false);
            this.MainTab.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl settingsTabControl;
        private System.Windows.Forms.TabPage MainTab;
        private System.Windows.Forms.TabPage AboutTab;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label GeneralGameLabel;
        private System.Windows.Forms.TextBox GeneralGameTextBox;
        private System.Windows.Forms.Button GeneralGameBrowseButton;
        private System.Windows.Forms.Button MainPlayButton;
        private System.Windows.Forms.Button MainCancelButton;
        private System.Windows.Forms.Button MainOkButton;
    }
}