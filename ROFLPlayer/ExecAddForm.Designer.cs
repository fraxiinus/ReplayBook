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
            this.ExecBrowseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ExecStartTextBox = new System.Windows.Forms.TextBox();
            this.ExecInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.ExecCancelButton = new System.Windows.Forms.Button();
            this.ExecSaveButton = new System.Windows.Forms.Button();
            this.ExecFlowLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExecFlowLayout
            // 
            this.ExecFlowLayout.Controls.Add(this.ExecNameLabel);
            this.ExecFlowLayout.Controls.Add(this.ExecNameTextBox);
            this.ExecFlowLayout.Controls.Add(this.ExecPathLabel);
            this.ExecFlowLayout.Controls.Add(this.ExecTargetTextBox);
            this.ExecFlowLayout.Controls.Add(this.label1);
            this.ExecFlowLayout.Controls.Add(this.ExecStartTextBox);
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
            this.ExecNameLabel.AutoSize = true;
            this.ExecNameLabel.Location = new System.Drawing.Point(3, 5);
            this.ExecNameLabel.Name = "ExecNameLabel";
            this.ExecNameLabel.Padding = new System.Windows.Forms.Padding(5);
            this.ExecNameLabel.Size = new System.Drawing.Size(48, 23);
            this.ExecNameLabel.TabIndex = 0;
            this.ExecNameLabel.Text = "Name:";
            // 
            // ExecNameTextBox
            // 
            this.ExecNameTextBox.Location = new System.Drawing.Point(107, 10);
            this.ExecNameTextBox.Margin = new System.Windows.Forms.Padding(53, 5, 5, 5);
            this.ExecNameTextBox.Name = "ExecNameTextBox";
            this.ExecNameTextBox.Size = new System.Drawing.Size(227, 20);
            this.ExecNameTextBox.TabIndex = 1;
            // 
            // ExecPathLabel
            // 
            this.ExecPathLabel.AutoSize = true;
            this.ExecPathLabel.Location = new System.Drawing.Point(3, 35);
            this.ExecPathLabel.Name = "ExecPathLabel";
            this.ExecPathLabel.Padding = new System.Windows.Forms.Padding(5);
            this.ExecPathLabel.Size = new System.Drawing.Size(51, 23);
            this.ExecPathLabel.TabIndex = 2;
            this.ExecPathLabel.Text = "Target:";
            // 
            // ExecTargetTextBox
            // 
            this.ExecTargetTextBox.Location = new System.Drawing.Point(107, 40);
            this.ExecTargetTextBox.Margin = new System.Windows.Forms.Padding(50, 5, 5, 5);
            this.ExecTargetTextBox.Name = "ExecTargetTextBox";
            this.ExecTargetTextBox.ReadOnly = true;
            this.ExecTargetTextBox.Size = new System.Drawing.Size(227, 20);
            this.ExecTargetTextBox.TabIndex = 3;
            // 
            // ExecBrowseButton
            // 
            this.ExecBrowseButton.Location = new System.Drawing.Point(260, 100);
            this.ExecBrowseButton.Margin = new System.Windows.Forms.Padding(260, 5, 5, 5);
            this.ExecBrowseButton.Name = "ExecBrowseButton";
            this.ExecBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.ExecBrowseButton.TabIndex = 4;
            this.ExecBrowseButton.Text = "Browse...";
            this.ExecBrowseButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 65);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(53, 23);
            this.label1.TabIndex = 5;
            this.label1.Text = "Start in:";
            // 
            // ExecStartTextBox
            // 
            this.ExecStartTextBox.Location = new System.Drawing.Point(107, 70);
            this.ExecStartTextBox.Margin = new System.Windows.Forms.Padding(48, 5, 5, 5);
            this.ExecStartTextBox.Name = "ExecStartTextBox";
            this.ExecStartTextBox.ReadOnly = true;
            this.ExecStartTextBox.Size = new System.Drawing.Size(227, 20);
            this.ExecStartTextBox.TabIndex = 6;
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
            // ExecCancelButton
            // 
            this.ExecCancelButton.Location = new System.Drawing.Point(85, 311);
            this.ExecCancelButton.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.ExecCancelButton.Name = "ExecCancelButton";
            this.ExecCancelButton.Size = new System.Drawing.Size(75, 23);
            this.ExecCancelButton.TabIndex = 8;
            this.ExecCancelButton.Text = "Cancel";
            this.ExecCancelButton.UseVisualStyleBackColor = true;
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
            // ExecAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 356);
            this.Controls.Add(this.ExecFlowLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ExecStartTextBox;
        private System.Windows.Forms.GroupBox ExecInfoGroupBox;
        private System.Windows.Forms.Button ExecSaveButton;
        private System.Windows.Forms.Button ExecCancelButton;
    }
}