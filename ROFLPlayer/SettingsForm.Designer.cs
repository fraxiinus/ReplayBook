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
            this.AboutTab = new System.Windows.Forms.TabPage();
            this.GeneralFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.GeneralGameLabel = new System.Windows.Forms.Label();
            this.GeneralGameTextBox = new System.Windows.Forms.TextBox();
            this.GeneralGameBrowseButton = new System.Windows.Forms.Button();
            this.MainCancelButton = new System.Windows.Forms.Button();
            this.MainOkButton = new System.Windows.Forms.Button();
            this.AboutFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.GeneralGameClearButton = new System.Windows.Forms.Button();
            this.DeleteMe = new System.Windows.Forms.Button();
            this.MainTabControl.SuspendLayout();
            this.GeneralTab.SuspendLayout();
            this.AboutTab.SuspendLayout();
            this.GeneralFlowLayout.SuspendLayout();
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
            // AboutTab
            // 
            this.AboutTab.Controls.Add(this.AboutFlowLayout);
            this.AboutTab.Location = new System.Drawing.Point(4, 22);
            this.AboutTab.Name = "AboutTab";
            this.AboutTab.Padding = new System.Windows.Forms.Padding(3);
            this.AboutTab.Size = new System.Drawing.Size(340, 382);
            this.AboutTab.TabIndex = 2;
            this.AboutTab.Text = "About";
            this.AboutTab.UseVisualStyleBackColor = true;
            // 
            // GeneralFlowLayout
            // 
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameTextBox);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameBrowseButton);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameClearButton);
            this.GeneralFlowLayout.Controls.Add(this.DeleteMe);
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
            this.GeneralGameLabel.Location = new System.Drawing.Point(3, 5);
            this.GeneralGameLabel.Name = "GeneralGameLabel";
            this.GeneralGameLabel.Size = new System.Drawing.Size(126, 13);
            this.GeneralGameLabel.TabIndex = 0;
            this.GeneralGameLabel.Text = "League of Legends path:";
            // 
            // GeneralGameTextBox
            // 
            this.GeneralGameTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GeneralGameTextBox.Location = new System.Drawing.Point(3, 23);
            this.GeneralGameTextBox.Name = "GeneralGameTextBox";
            this.GeneralGameTextBox.Size = new System.Drawing.Size(234, 20);
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
            this.GeneralGameBrowseButton.Click += new System.EventHandler(this.GeneralGameBrowseButton_Click);
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
            // AboutFlowLayout
            // 
            this.AboutFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AboutFlowLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.AboutFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.AboutFlowLayout.Name = "AboutFlowLayout";
            this.AboutFlowLayout.Size = new System.Drawing.Size(334, 376);
            this.AboutFlowLayout.TabIndex = 0;
            // 
            // GeneralGameClearButton
            // 
            this.GeneralGameClearButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameClearButton.Location = new System.Drawing.Point(3, 52);
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
            this.DeleteMe.Location = new System.Drawing.Point(97, 52);
            this.DeleteMe.Name = "DeleteMe";
            this.DeleteMe.Size = new System.Drawing.Size(88, 25);
            this.DeleteMe.TabIndex = 4;
            this.DeleteMe.Text = "Play";
            this.DeleteMe.UseVisualStyleBackColor = true;
            this.DeleteMe.Click += new System.EventHandler(this.DeleteMe_Click);
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
            this.AboutTab.ResumeLayout(false);
            this.GeneralFlowLayout.ResumeLayout(false);
            this.GeneralFlowLayout.PerformLayout();
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
        private System.Windows.Forms.FlowLayoutPanel AboutFlowLayout;
        private System.Windows.Forms.Button GeneralGameClearButton;
        private System.Windows.Forms.Button DeleteMe;
    }
}