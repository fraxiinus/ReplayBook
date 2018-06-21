namespace ROFLPlayer
{
    partial class DetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailForm));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.GeneralTab = new System.Windows.Forms.TabPage();
            this.GeneralFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.GeneralDivider1 = new System.Windows.Forms.Label();
            this.AboutTab = new System.Windows.Forms.TabPage();
            this.AboutTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.AboutLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.AboutTitleLabel = new System.Windows.Forms.Label();
            this.AboutVersionLabel = new System.Windows.Forms.Label();
            this.AboutAuthorLabel = new System.Windows.Forms.Label();
            this.AboutGithubButton = new System.Windows.Forms.Button();
            this.AboutCatLabel = new System.Windows.Forms.Label();
            this.MainOkButton = new System.Windows.Forms.Button();
            this.MainCancelButton = new System.Windows.Forms.Button();
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
            this.MainTabControl.TabIndex = 1;
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
            this.GeneralFlowLayout.Controls.Add(this.GeneralDivider1);
            this.GeneralFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.GeneralFlowLayout.Name = "GeneralFlowLayout";
            this.GeneralFlowLayout.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.GeneralFlowLayout.Size = new System.Drawing.Size(334, 391);
            this.GeneralFlowLayout.TabIndex = 0;
            // 
            // GeneralDivider1
            // 
            this.GeneralDivider1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeneralDivider1.Location = new System.Drawing.Point(5, 10);
            this.GeneralDivider1.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDivider1.Name = "GeneralDivider1";
            this.GeneralDivider1.Size = new System.Drawing.Size(315, 2);
            this.GeneralDivider1.TabIndex = 6;
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
            // AboutTableLayoutPanel
            // 
            this.AboutTableLayoutPanel.ColumnCount = 1;
            this.AboutTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AboutTableLayoutPanel.Controls.Add(this.AboutLogoPictureBox, 0, 1);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutTitleLabel, 0, 2);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutVersionLabel, 0, 3);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutAuthorLabel, 0, 4);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutGithubButton, 0, 6);
            this.AboutTableLayoutPanel.Controls.Add(this.AboutCatLabel, 0, 5);
            this.AboutTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AboutTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.AboutTableLayoutPanel.Name = "AboutTableLayoutPanel";
            this.AboutTableLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.AboutTableLayoutPanel.RowCount = 7;
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.AboutTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
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
            this.AboutGithubButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AboutGithubButton.Location = new System.Drawing.Point(3, 360);
            this.AboutGithubButton.Name = "AboutGithubButton";
            this.AboutGithubButton.Size = new System.Drawing.Size(75, 23);
            this.AboutGithubButton.TabIndex = 5;
            this.AboutGithubButton.Text = "GitHub";
            this.AboutGithubButton.UseVisualStyleBackColor = true;
            // 
            // AboutCatLabel
            // 
            this.AboutCatLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AboutCatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutCatLabel.Location = new System.Drawing.Point(3, 196);
            this.AboutCatLabel.Name = "AboutCatLabel";
            this.AboutCatLabel.Size = new System.Drawing.Size(328, 20);
            this.AboutCatLabel.TabIndex = 6;
            this.AboutCatLabel.Text = "buff nidalee";
            this.AboutCatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainOkButton
            // 
            this.MainOkButton.Location = new System.Drawing.Point(194, 434);
            this.MainOkButton.Name = "MainOkButton";
            this.MainOkButton.Size = new System.Drawing.Size(75, 23);
            this.MainOkButton.TabIndex = 5;
            this.MainOkButton.Text = "OK";
            this.MainOkButton.UseVisualStyleBackColor = true;
            this.MainOkButton.Click += new System.EventHandler(this.MainOkButton_Click);
            // 
            // MainCancelButton
            // 
            this.MainCancelButton.Location = new System.Drawing.Point(275, 434);
            this.MainCancelButton.Name = "MainCancelButton";
            this.MainCancelButton.Size = new System.Drawing.Size(75, 23);
            this.MainCancelButton.TabIndex = 4;
            this.MainCancelButton.Text = "Cancel";
            this.MainCancelButton.UseVisualStyleBackColor = true;
            this.MainCancelButton.Click += new System.EventHandler(this.MainCancelButton_Click);
            // 
            // DetailForm
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
            this.Name = "DetailForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ROFL Player";
            this.MainTabControl.ResumeLayout(false);
            this.GeneralTab.ResumeLayout(false);
            this.GeneralFlowLayout.ResumeLayout(false);
            this.AboutTab.ResumeLayout(false);
            this.AboutTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AboutLogoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage GeneralTab;
        private System.Windows.Forms.FlowLayoutPanel GeneralFlowLayout;
        private System.Windows.Forms.Label GeneralDivider1;
        private System.Windows.Forms.TabPage AboutTab;
        private System.Windows.Forms.TableLayoutPanel AboutTableLayoutPanel;
        private System.Windows.Forms.PictureBox AboutLogoPictureBox;
        private System.Windows.Forms.Label AboutTitleLabel;
        private System.Windows.Forms.Label AboutVersionLabel;
        private System.Windows.Forms.Label AboutAuthorLabel;
        private System.Windows.Forms.Button AboutGithubButton;
        private System.Windows.Forms.Label AboutCatLabel;
        private System.Windows.Forms.Button MainOkButton;
        private System.Windows.Forms.Button MainCancelButton;
    }
}