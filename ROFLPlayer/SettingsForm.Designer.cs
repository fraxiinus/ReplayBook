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
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.GeneralTab = new System.Windows.Forms.TabPage();
            this.GeneralFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.GeneralGameLabel = new System.Windows.Forms.Label();
            this.GeneralGameTextBox = new System.Windows.Forms.TextBox();
            this.GeneralGameBrowseButton = new System.Windows.Forms.Button();
            this.GeneralGameClearButton = new System.Windows.Forms.Button();
            this.DeleteMe = new System.Windows.Forms.Button();
            this.GeneralDivider1 = new System.Windows.Forms.Label();
            this.GeneralLaunchLabel = new System.Windows.Forms.Label();
            this.GeneralLaunchComboBox = new System.Windows.Forms.ComboBox();
            this.AboutTab = new System.Windows.Forms.TabPage();
            this.MainCancelButton = new System.Windows.Forms.Button();
            this.MainOkButton = new System.Windows.Forms.Button();
            this.AboutTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AboutLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.AboutTitleLabel = new System.Windows.Forms.Label();
            this.AboutVersionLabel = new System.Windows.Forms.Label();
            this.AboutAuthorLabel = new System.Windows.Forms.Label();
            this.AboutGithubButton = new System.Windows.Forms.Button();
            this.MainTabControl.SuspendLayout();
            this.GeneralTab.SuspendLayout();
            this.GeneralFlowLayout.SuspendLayout();
            this.AboutTab.SuspendLayout();
            this.AboutTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AboutLogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.GeneralTab);
            this.MainTabControl.Controls.Add(this.AboutTab);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainTabControl.Location = new System.Drawing.Point(5, 5);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(348, 423);
            this.MainTabControl.TabIndex = 0;
            // 
            // GeneralTab
            // 
            this.GeneralTab.Controls.Add(this.GeneralFlowLayout);
            this.GeneralTab.Location = new System.Drawing.Point(4, 22);
            this.GeneralTab.Name = "GeneralTab";
            this.GeneralTab.Padding = new System.Windows.Forms.Padding(3);
            this.GeneralTab.Size = new System.Drawing.Size(340, 397);
            this.GeneralTab.TabIndex = 1;
            this.GeneralTab.Text = "General";
            this.GeneralTab.UseVisualStyleBackColor = true;
            // 
            // GeneralFlowLayout
            // 
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameTextBox);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameBrowseButton);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameClearButton);
            this.GeneralFlowLayout.Controls.Add(this.DeleteMe);
            this.GeneralFlowLayout.Controls.Add(this.GeneralDivider1);
            this.GeneralFlowLayout.Controls.Add(this.GeneralLaunchLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralLaunchComboBox);
            this.GeneralFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.GeneralFlowLayout.Name = "GeneralFlowLayout";
            this.GeneralFlowLayout.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.GeneralFlowLayout.Size = new System.Drawing.Size(334, 391);
            this.GeneralFlowLayout.TabIndex = 0;
            // 
            // GeneralGameLabel
            // 
            this.GeneralGameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameLabel.AutoSize = true;
            this.GeneralGameLabel.Location = new System.Drawing.Point(5, 13);
            this.GeneralGameLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameLabel.Name = "GeneralGameLabel";
            this.GeneralGameLabel.Size = new System.Drawing.Size(87, 13);
            this.GeneralGameLabel.TabIndex = 0;
            this.GeneralGameLabel.Text = "Executable path:";
            // 
            // GeneralGameTextBox
            // 
            this.GeneralGameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GeneralGameTextBox.Location = new System.Drawing.Point(102, 10);
            this.GeneralGameTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameTextBox.Name = "GeneralGameTextBox";
            this.GeneralGameTextBox.Size = new System.Drawing.Size(227, 20);
            this.GeneralGameTextBox.TabIndex = 1;
            // 
            // GeneralGameBrowseButton
            // 
            this.GeneralGameBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameBrowseButton.Location = new System.Drawing.Point(5, 40);
            this.GeneralGameBrowseButton.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameBrowseButton.Name = "GeneralGameBrowseButton";
            this.GeneralGameBrowseButton.Size = new System.Drawing.Size(88, 25);
            this.GeneralGameBrowseButton.TabIndex = 2;
            this.GeneralGameBrowseButton.Text = "Browse...";
            this.GeneralGameBrowseButton.UseVisualStyleBackColor = true;
            this.GeneralGameBrowseButton.Click += new System.EventHandler(this.GeneralGameBrowseButton_Click);
            // 
            // GeneralGameClearButton
            // 
            this.GeneralGameClearButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameClearButton.Enabled = false;
            this.GeneralGameClearButton.Location = new System.Drawing.Point(103, 40);
            this.GeneralGameClearButton.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameClearButton.Name = "GeneralGameClearButton";
            this.GeneralGameClearButton.Size = new System.Drawing.Size(88, 25);
            this.GeneralGameClearButton.TabIndex = 3;
            this.GeneralGameClearButton.Text = "Clear";
            this.GeneralGameClearButton.UseVisualStyleBackColor = true;
            this.GeneralGameClearButton.Click += new System.EventHandler(this.GeneralGameClearButton_Click);
            // 
            // DeleteMe
            // 
            this.DeleteMe.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DeleteMe.Location = new System.Drawing.Point(201, 40);
            this.DeleteMe.Margin = new System.Windows.Forms.Padding(5);
            this.DeleteMe.Name = "DeleteMe";
            this.DeleteMe.Size = new System.Drawing.Size(88, 25);
            this.DeleteMe.TabIndex = 4;
            this.DeleteMe.Text = "Play";
            this.DeleteMe.UseVisualStyleBackColor = true;
            this.DeleteMe.Click += new System.EventHandler(this.DeleteMe_Click);
            // 
            // GeneralDivider1
            // 
            this.GeneralDivider1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeneralDivider1.Location = new System.Drawing.Point(5, 75);
            this.GeneralDivider1.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDivider1.Name = "GeneralDivider1";
            this.GeneralDivider1.Size = new System.Drawing.Size(315, 2);
            this.GeneralDivider1.TabIndex = 6;
            // 
            // GeneralLaunchLabel
            // 
            this.GeneralLaunchLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralLaunchLabel.AutoSize = true;
            this.GeneralLaunchLabel.Location = new System.Drawing.Point(5, 91);
            this.GeneralLaunchLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralLaunchLabel.Name = "GeneralLaunchLabel";
            this.GeneralLaunchLabel.Size = new System.Drawing.Size(101, 13);
            this.GeneralLaunchLabel.TabIndex = 5;
            this.GeneralLaunchLabel.Text = "Double-click action:";
            // 
            // GeneralLaunchComboBox
            // 
            this.GeneralLaunchComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralLaunchComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GeneralLaunchComboBox.FormattingEnabled = true;
            this.GeneralLaunchComboBox.Items.AddRange(new object[] {
            "Play replay",
            "Show details"});
            this.GeneralLaunchComboBox.Location = new System.Drawing.Point(116, 87);
            this.GeneralLaunchComboBox.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralLaunchComboBox.Name = "GeneralLaunchComboBox";
            this.GeneralLaunchComboBox.Size = new System.Drawing.Size(213, 21);
            this.GeneralLaunchComboBox.TabIndex = 7;
            // 
            // AboutTab
            // 
            this.AboutTab.Controls.Add(this.AboutTableLayoutPanel);
            this.AboutTab.Location = new System.Drawing.Point(4, 22);
            this.AboutTab.Name = "AboutTab";
            this.AboutTab.Padding = new System.Windows.Forms.Padding(3);
            this.AboutTab.Size = new System.Drawing.Size(340, 397);
            this.AboutTab.TabIndex = 2;
            this.AboutTab.Text = "About";
            this.AboutTab.UseVisualStyleBackColor = true;
            // 
            // MainCancelButton
            // 
            this.MainCancelButton.Location = new System.Drawing.Point(275, 434);
            this.MainCancelButton.Name = "MainCancelButton";
            this.MainCancelButton.Size = new System.Drawing.Size(75, 23);
            this.MainCancelButton.TabIndex = 2;
            this.MainCancelButton.Text = "Cancel";
            this.MainCancelButton.UseVisualStyleBackColor = true;
            this.MainCancelButton.Click += new System.EventHandler(this.MainCancelButton_Click);
            // 
            // MainOkButton
            // 
            this.MainOkButton.Location = new System.Drawing.Point(194, 434);
            this.MainOkButton.Name = "MainOkButton";
            this.MainOkButton.Size = new System.Drawing.Size(75, 23);
            this.MainOkButton.TabIndex = 3;
            this.MainOkButton.Text = "OK";
            this.MainOkButton.UseVisualStyleBackColor = true;
            this.MainOkButton.Click += new System.EventHandler(this.MainOkButton_Click);
            // 
            // AboutTableLayoutPanel
            // 
            this.AboutTableLayoutPanel.ColumnCount = 1;
            this.AboutTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AboutTableLayoutPanel.Controls.Add(this.AboutLogoPictureBox, 0, 1);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutTitleLabel, 0, 2);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutVersionLabel, 0, 3);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutAuthorLabel, 0, 4);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutGithubButton, 0, 5);
            this.AboutTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AboutTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.AboutTableLayoutPanel.Name = "AboutTableLayoutPanel";
            this.AboutTableLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.AboutTableLayoutPanel.RowCount = 6;
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.AboutTableLayoutPanel.Size = new System.Drawing.Size(334, 391);
            this.AboutTableLayoutPanel.TabIndex = 0;
            // 
            // AboutLogoPictureBox
            // 
            this.AboutLogoPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.AboutLogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("AboutLogoPictureBox.Image")));
            this.AboutLogoPictureBox.Location = new System.Drawing.Point(125, 28);
            this.AboutLogoPictureBox.Name = "AboutLogoPictureBox";
            this.AboutLogoPictureBox.Size = new System.Drawing.Size(84, 96);
            this.AboutLogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.AboutLogoPictureBox.TabIndex = 1;
            this.AboutLogoPictureBox.TabStop = false;
            // 
            // AboutTitleLabel
            // 
            this.AboutTitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AboutTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutTitleLabel.Location = new System.Drawing.Point(3, 127);
            this.AboutTitleLabel.Name = "AboutTitleLabel";
            this.AboutTitleLabel.Size = new System.Drawing.Size(328, 23);
            this.AboutTitleLabel.TabIndex = 2;
            this.AboutTitleLabel.Text = "ROFL Player";
            this.AboutTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutVersionLabel
            // 
            this.AboutVersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AboutVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutVersionLabel.Location = new System.Drawing.Point(3, 150);
            this.AboutVersionLabel.Name = "AboutVersionLabel";
            this.AboutVersionLabel.Size = new System.Drawing.Size(328, 23);
            this.AboutVersionLabel.TabIndex = 3;
            this.AboutVersionLabel.Text = "Beta";
            this.AboutVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutAuthorLabel
            // 
            this.AboutAuthorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AboutAuthorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutAuthorLabel.Location = new System.Drawing.Point(3, 173);
            this.AboutAuthorLabel.Name = "AboutAuthorLabel";
            this.AboutAuthorLabel.Size = new System.Drawing.Size(328, 23);
            this.AboutAuthorLabel.TabIndex = 4;
            this.AboutAuthorLabel.Text = "Anchu Lee";
            this.AboutAuthorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutGithubButton
            // 
            this.AboutGithubButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AboutGithubButton.Location = new System.Drawing.Point(130, 360);
            this.AboutGithubButton.Margin = new System.Windows.Forms.Padding(130, 3, 130, 3);
            this.AboutGithubButton.Name = "AboutGithubButton";
            this.AboutGithubButton.Size = new System.Drawing.Size(74, 23);
            this.AboutGithubButton.TabIndex = 5;
            this.AboutGithubButton.Text = "GitHub";
            this.AboutGithubButton.UseVisualStyleBackColor = true;
            this.AboutGithubButton.Click += new System.EventHandler(this.AboutGithubButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 465);
            this.Controls.Add(this.MainOkButton);
            this.Controls.Add(this.MainCancelButton);
            this.Controls.Add(this.MainTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ROFL Player Settings";
            this.MainTabControl.ResumeLayout(false);
            this.GeneralTab.ResumeLayout(false);
            this.GeneralFlowLayout.ResumeLayout(false);
            this.GeneralFlowLayout.PerformLayout();
            this.AboutTab.ResumeLayout(false);
            this.AboutTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AboutLogoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage GeneralTab;
        private System.Windows.Forms.TabPage AboutTab;
        private System.Windows.Forms.FlowLayoutPanel GeneralFlowLayout;
        private System.Windows.Forms.Label GeneralGameLabel;
        private System.Windows.Forms.TextBox GeneralGameTextBox;
        private System.Windows.Forms.Button GeneralGameBrowseButton;
        private System.Windows.Forms.Button MainCancelButton;
        private System.Windows.Forms.Button MainOkButton;
        private System.Windows.Forms.Button GeneralGameClearButton;
        private System.Windows.Forms.Button DeleteMe;
        private System.Windows.Forms.Label GeneralLaunchLabel;
        private System.Windows.Forms.Label GeneralDivider1;
        private System.Windows.Forms.ComboBox GeneralLaunchComboBox;
        private System.Windows.Forms.TableLayoutPanel AboutTableLayoutPanel;
        private System.Windows.Forms.PictureBox AboutLogoPictureBox;
        private System.Windows.Forms.Label AboutTitleLabel;
        private System.Windows.Forms.Label AboutVersionLabel;
        private System.Windows.Forms.Label AboutAuthorLabel;
        private System.Windows.Forms.Button AboutGithubButton;
    }
}