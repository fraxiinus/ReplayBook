namespace ROFLPlayer
{
    partial class UpdateSplashForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateSplashForm));
            this.TitleLabel = new System.Windows.Forms.Label();
            this.LoadingProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Location = new System.Drawing.Point(6, 5);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(115, 13);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "Starting ROFL Player...";
            // 
            // LoadingProgressBar
            // 
            this.LoadingProgressBar.Location = new System.Drawing.Point(8, 21);
            this.LoadingProgressBar.Name = "LoadingProgressBar";
            this.LoadingProgressBar.Size = new System.Drawing.Size(267, 23);
            this.LoadingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.LoadingProgressBar.TabIndex = 1;
            // 
            // UpdateSplashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 52);
            this.Controls.Add(this.LoadingProgressBar);
            this.Controls.Add(this.TitleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateSplashForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ROFL Player";
            this.Load += new System.EventHandler(this.UpdateSplashForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.ProgressBar LoadingProgressBar;
    }
}