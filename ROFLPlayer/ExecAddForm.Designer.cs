namespace ROFLPlayer
{
    partial class ExecAddForm
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
            this.ExecFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.ExecNameLabel = new System.Windows.Forms.Label();
            this.ExecNameTextBox = new System.Windows.Forms.TextBox();
            this.ExecPathLabel = new System.Windows.Forms.Label();
            this.ExecTargetTextBox = new System.Windows.Forms.TextBox();
            this.ExecStartLabel = new System.Windows.Forms.Label();
            this.ExecStartTextBox = new System.Windows.Forms.TextBox();
            this.ExecUpdateCheckbox = new System.Windows.Forms.CheckBox();
            this.ExecBrowseButton = new System.Windows.Forms.Button();
            this.ExecInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.ExecSaveButton = new System.Windows.Forms.Button();
            this.ExecCancelButton = new System.Windows.Forms.Button();
            this.ExecFlowLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExecFlowLayout
            // 
            this.ExecFlowLayout.Controls.Add(this.ExecNameLabel);
            this.ExecFlowLayout.Controls.Add(this.ExecNameTextBox);
            this.ExecFlowLayout.Controls.Add(this.ExecPathLabel);
            this.ExecFlowLayout.Controls.Add(this.ExecTargetTextBox);
            this.ExecFlowLayout.Controls.Add(this.ExecStartLabel);
            this.ExecFlowLayout.Controls.Add(this.ExecStartTextBox);
            this.ExecFlowLayout.Controls.Add(this.ExecUpdateCheckbox);
            this.ExecFlowLayout.Controls.Add(this.ExecBrowseButton);
            this.ExecFlowLayout.Controls.Add(this.ExecInfoGroupBox);
            this.ExecFlowLayout.Controls.Add(this.ExecSaveButton);
            this.ExecFlowLayout.Controls.Add(this.ExecCancelButton);
            this.ExecFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExecFlowLayout.Location = new System.Drawing.Point(5, 5);
            this.ExecFlowLayout.Name = "ExecFlowLayout";
            this.ExecFlowLayout.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.ExecFlowLayout.Size = new System.Drawing.Size(348, 346);
            this.ExecFlowLayout.TabIndex = 0;
            // 
            // ExecNameLabel
            // 
            this.ExecNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ExecNameLabel.Location = new System.Drawing.Point(5, 13);
            this.ExecNameLabel.Margin = new System.Windows.Forms.Padding(5);
            this.ExecNameLabel.Name = "ExecNameLabel";
            this.ExecNameLabel.Size = new System.Drawing.Size(75, 13);
            this.ExecNameLabel.TabIndex = 0;
            this.ExecNameLabel.Text = "Name:";
            // 
            // ExecNameTextBox
            // 
            this.ExecNameTextBox.Location = new System.Drawing.Point(90, 10);
            this.ExecNameTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.ExecNameTextBox.Name = "ExecNameTextBox";
            this.ExecNameTextBox.Size = new System.Drawing.Size(245, 20);
            this.ExecNameTextBox.TabIndex = 1;
            // 
            // ExecPathLabel
            // 
            this.ExecPathLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ExecPathLabel.Location = new System.Drawing.Point(5, 43);
            this.ExecPathLabel.Margin = new System.Windows.Forms.Padding(5);
            this.ExecPathLabel.Name = "ExecPathLabel";
            this.ExecPathLabel.Size = new System.Drawing.Size(75, 13);
            this.ExecPathLabel.TabIndex = 2;
            this.ExecPathLabel.Text = "Target:";
            // 
            // ExecTargetTextBox
            // 
            this.ExecTargetTextBox.Location = new System.Drawing.Point(90, 40);
            this.ExecTargetTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.ExecTargetTextBox.Name = "ExecTargetTextBox";
            this.ExecTargetTextBox.ReadOnly = true;
            this.ExecTargetTextBox.Size = new System.Drawing.Size(245, 20);
            this.ExecTargetTextBox.TabIndex = 3;
            // 
            // ExecStartLabel
            // 
            this.ExecStartLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ExecStartLabel.Location = new System.Drawing.Point(5, 73);
            this.ExecStartLabel.Margin = new System.Windows.Forms.Padding(5);
            this.ExecStartLabel.Name = "ExecStartLabel";
            this.ExecStartLabel.Size = new System.Drawing.Size(75, 13);
            this.ExecStartLabel.TabIndex = 5;
            this.ExecStartLabel.Text = "Start in:";
            // 
            // ExecStartTextBox
            // 
            this.ExecStartTextBox.Location = new System.Drawing.Point(90, 70);
            this.ExecStartTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.ExecStartTextBox.Name = "ExecStartTextBox";
            this.ExecStartTextBox.ReadOnly = true;
            this.ExecStartTextBox.Size = new System.Drawing.Size(245, 20);
            this.ExecStartTextBox.TabIndex = 6;
            // 
            // ExecUpdateCheckbox
            // 
            this.ExecUpdateCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ExecUpdateCheckbox.AutoSize = true;
            this.ExecUpdateCheckbox.Location = new System.Drawing.Point(159, 103);
            this.ExecUpdateCheckbox.Margin = new System.Windows.Forms.Padding(159, 5, 5, 5);
            this.ExecUpdateCheckbox.Name = "ExecUpdateCheckbox";
            this.ExecUpdateCheckbox.Size = new System.Drawing.Size(92, 17);
            this.ExecUpdateCheckbox.TabIndex = 10;
            this.ExecUpdateCheckbox.Text = "Allow updates";
            this.ExecUpdateCheckbox.UseVisualStyleBackColor = true;
            // 
            // ExecBrowseButton
            // 
            this.ExecBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ExecBrowseButton.Location = new System.Drawing.Point(261, 100);
            this.ExecBrowseButton.Margin = new System.Windows.Forms.Padding(5);
            this.ExecBrowseButton.Name = "ExecBrowseButton";
            this.ExecBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.ExecBrowseButton.TabIndex = 4;
            this.ExecBrowseButton.Text = "Browse...";
            this.ExecBrowseButton.UseVisualStyleBackColor = true;
            this.ExecBrowseButton.Click += new System.EventHandler(this.ExecBrowseButton_Click);
            // 
            // ExecInfoGroupBox
            // 
            this.ExecInfoGroupBox.Location = new System.Drawing.Point(5, 133);
            this.ExecInfoGroupBox.Margin = new System.Windows.Forms.Padding(5);
            this.ExecInfoGroupBox.Name = "ExecInfoGroupBox";
            this.ExecInfoGroupBox.Size = new System.Drawing.Size(330, 168);
            this.ExecInfoGroupBox.TabIndex = 7;
            this.ExecInfoGroupBox.TabStop = false;
            this.ExecInfoGroupBox.Text = "Target Information";
            // 
            // ExecSaveButton
            // 
            this.ExecSaveButton.Location = new System.Drawing.Point(5, 311);
            this.ExecSaveButton.Margin = new System.Windows.Forms.Padding(5);
            this.ExecSaveButton.Name = "ExecSaveButton";
            this.ExecSaveButton.Size = new System.Drawing.Size(75, 23);
            this.ExecSaveButton.TabIndex = 9;
            this.ExecSaveButton.Text = "Save";
            this.ExecSaveButton.UseVisualStyleBackColor = true;
            // 
            // ExecCancelButton
            // 
            this.ExecCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExecCancelButton.Location = new System.Drawing.Point(85, 311);
            this.ExecCancelButton.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.ExecCancelButton.Name = "ExecCancelButton";
            this.ExecCancelButton.Size = new System.Drawing.Size(75, 23);
            this.ExecCancelButton.TabIndex = 8;
            this.ExecCancelButton.Text = "Cancel";
            this.ExecCancelButton.UseVisualStyleBackColor = true;
            this.ExecCancelButton.Click += new System.EventHandler(this.ExecCancelButton_Click);
            // 
            // ExecAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.ExecCancelButton;
            this.ClientSize = new System.Drawing.Size(358, 356);
            this.Controls.Add(this.ExecFlowLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExecAddForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Executable...";
            this.ExecFlowLayout.ResumeLayout(false);
            this.ExecFlowLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ExecFlowLayout;
        private System.Windows.Forms.Label ExecNameLabel;
        private System.Windows.Forms.TextBox ExecNameTextBox;
        private System.Windows.Forms.Label ExecPathLabel;
        private System.Windows.Forms.TextBox ExecTargetTextBox;
        private System.Windows.Forms.Button ExecBrowseButton;
        private System.Windows.Forms.Label ExecStartLabel;
        private System.Windows.Forms.TextBox ExecStartTextBox;
        private System.Windows.Forms.GroupBox ExecInfoGroupBox;
        private System.Windows.Forms.Button ExecSaveButton;
        private System.Windows.Forms.Button ExecCancelButton;
        private System.Windows.Forms.CheckBox ExecUpdateCheckbox;
    }
}