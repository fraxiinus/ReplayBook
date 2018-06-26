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
            this.GeneralGamePictureBox = new System.Windows.Forms.PictureBox();
            this.GeneralGameFileLabel = new System.Windows.Forms.TextBox();
            this.GeneralDivider1 = new System.Windows.Forms.Label();
            this.GeneralGameVersionLabel = new System.Windows.Forms.Label();
            this.GeneralGameVersionDataLabel = new System.Windows.Forms.TextBox();
            this.GeneralGameLengthLabel = new System.Windows.Forms.Label();
            this.GeneralGameLengthDataLabel = new System.Windows.Forms.TextBox();
            this.GeneralGameMatchIDLabel = new System.Windows.Forms.Label();
            this.GeneralGameMatchIDData = new System.Windows.Forms.TextBox();
            this.GeneralDivider2 = new System.Windows.Forms.Label();
            this.GeneralMatchWinnerLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.GeneralPlayerImage1 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerImage2 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerImage3 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerImage4 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerName1 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerName2 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerImage5 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerName3 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerName4 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerImage6 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerImage7 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerImage8 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerImage9 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerImage10 = new System.Windows.Forms.PictureBox();
            this.GeneralPlayerName5 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerName6 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerName7 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerName8 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerName9 = new System.Windows.Forms.TextBox();
            this.GeneralPlayerName10 = new System.Windows.Forms.TextBox();
            this.GeneralDebugDumpJsonButton = new System.Windows.Forms.Button();
            this.GeneralGameStatsButton = new System.Windows.Forms.Button();
            this.GeneralStartReplayButton = new System.Windows.Forms.Button();
            this.MatchTab = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.MatchIDLabel = new System.Windows.Forms.Label();
            this.MatchIDDataLabel = new System.Windows.Forms.Label();
            this.SetMatchIDButton = new System.Windows.Forms.Button();
            this.GameRegionLabel = new System.Windows.Forms.Label();
            this.GameRegionDataLabel = new System.Windows.Forms.Label();
            this.GameModeLabel = new System.Windows.Forms.Label();
            this.GameModeDataLabel = new System.Windows.Forms.Label();
            this.GameMapLabel = new System.Windows.Forms.Label();
            this.GameMapDataLabel = new System.Windows.Forms.Label();
            this.GameDateLabel = new System.Windows.Forms.Label();
            this.GameDateDataLabel = new System.Windows.Forms.Label();
            this.GeneralDivider3 = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.GeneralGamePictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage10)).BeginInit();
            this.MatchTab.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.AboutTab.SuspendLayout();
            this.AboutTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AboutLogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.GeneralTab);
            this.MainTabControl.Controls.Add(this.MatchTab);
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
            this.GeneralFlowLayout.Controls.Add(this.GeneralGamePictureBox);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameFileLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralDivider1);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameVersionLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameVersionDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameLengthLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameLengthDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameMatchIDLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameMatchIDData);
            this.GeneralFlowLayout.Controls.Add(this.GeneralDivider2);
            this.GeneralFlowLayout.Controls.Add(this.GeneralMatchWinnerLabel);
            this.GeneralFlowLayout.Controls.Add(this.tableLayoutPanel1);
            this.GeneralFlowLayout.Controls.Add(this.GeneralDebugDumpJsonButton);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameStatsButton);
            this.GeneralFlowLayout.Controls.Add(this.GeneralStartReplayButton);
            this.GeneralFlowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralFlowLayout.Location = new System.Drawing.Point(3, 3);
            this.GeneralFlowLayout.Name = "GeneralFlowLayout";
            this.GeneralFlowLayout.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.GeneralFlowLayout.Size = new System.Drawing.Size(334, 391);
            this.GeneralFlowLayout.TabIndex = 0;
            // 
            // GeneralGamePictureBox
            // 
            this.GeneralGamePictureBox.Location = new System.Drawing.Point(5, 10);
            this.GeneralGamePictureBox.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGamePictureBox.Name = "GeneralGamePictureBox";
            this.GeneralGamePictureBox.Size = new System.Drawing.Size(50, 50);
            this.GeneralGamePictureBox.TabIndex = 8;
            this.GeneralGamePictureBox.TabStop = false;
            // 
            // GeneralGameFileLabel
            // 
            this.GeneralGameFileLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameFileLabel.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralGameFileLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralGameFileLabel.Location = new System.Drawing.Point(65, 28);
            this.GeneralGameFileLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameFileLabel.Name = "GeneralGameFileLabel";
            this.GeneralGameFileLabel.ReadOnly = true;
            this.GeneralGameFileLabel.Size = new System.Drawing.Size(231, 13);
            this.GeneralGameFileLabel.TabIndex = 42;
            this.GeneralGameFileLabel.Text = "-";
            // 
            // GeneralDivider1
            // 
            this.GeneralDivider1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeneralDivider1.Location = new System.Drawing.Point(5, 70);
            this.GeneralDivider1.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDivider1.Name = "GeneralDivider1";
            this.GeneralDivider1.Size = new System.Drawing.Size(315, 2);
            this.GeneralDivider1.TabIndex = 6;
            // 
            // GeneralGameVersionLabel
            // 
            this.GeneralGameVersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameVersionLabel.Location = new System.Drawing.Point(5, 82);
            this.GeneralGameVersionLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameVersionLabel.Name = "GeneralGameVersionLabel";
            this.GeneralGameVersionLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralGameVersionLabel.TabIndex = 12;
            this.GeneralGameVersionLabel.Text = "Game Version:";
            // 
            // GeneralGameVersionDataLabel
            // 
            this.GeneralGameVersionDataLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameVersionDataLabel.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralGameVersionDataLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralGameVersionDataLabel.Location = new System.Drawing.Point(91, 82);
            this.GeneralGameVersionDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameVersionDataLabel.Name = "GeneralGameVersionDataLabel";
            this.GeneralGameVersionDataLabel.ReadOnly = true;
            this.GeneralGameVersionDataLabel.Size = new System.Drawing.Size(231, 13);
            this.GeneralGameVersionDataLabel.TabIndex = 40;
            this.GeneralGameVersionDataLabel.Text = "-";
            // 
            // GeneralGameLengthLabel
            // 
            this.GeneralGameLengthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameLengthLabel.Location = new System.Drawing.Point(5, 105);
            this.GeneralGameLengthLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameLengthLabel.Name = "GeneralGameLengthLabel";
            this.GeneralGameLengthLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralGameLengthLabel.TabIndex = 9;
            this.GeneralGameLengthLabel.Text = "Game Length:";
            // 
            // GeneralGameLengthDataLabel
            // 
            this.GeneralGameLengthDataLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameLengthDataLabel.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralGameLengthDataLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralGameLengthDataLabel.Location = new System.Drawing.Point(91, 105);
            this.GeneralGameLengthDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameLengthDataLabel.Name = "GeneralGameLengthDataLabel";
            this.GeneralGameLengthDataLabel.ReadOnly = true;
            this.GeneralGameLengthDataLabel.Size = new System.Drawing.Size(231, 13);
            this.GeneralGameLengthDataLabel.TabIndex = 41;
            this.GeneralGameLengthDataLabel.Text = "-";
            // 
            // GeneralGameMatchIDLabel
            // 
            this.GeneralGameMatchIDLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameMatchIDLabel.Location = new System.Drawing.Point(5, 128);
            this.GeneralGameMatchIDLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameMatchIDLabel.Name = "GeneralGameMatchIDLabel";
            this.GeneralGameMatchIDLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralGameMatchIDLabel.TabIndex = 43;
            this.GeneralGameMatchIDLabel.Text = "Match ID:";
            // 
            // GeneralGameMatchIDData
            // 
            this.GeneralGameMatchIDData.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameMatchIDData.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralGameMatchIDData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralGameMatchIDData.Location = new System.Drawing.Point(91, 128);
            this.GeneralGameMatchIDData.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameMatchIDData.Name = "GeneralGameMatchIDData";
            this.GeneralGameMatchIDData.ReadOnly = true;
            this.GeneralGameMatchIDData.Size = new System.Drawing.Size(231, 13);
            this.GeneralGameMatchIDData.TabIndex = 44;
            this.GeneralGameMatchIDData.Text = "-";
            // 
            // GeneralDivider2
            // 
            this.GeneralDivider2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeneralDivider2.Location = new System.Drawing.Point(5, 151);
            this.GeneralDivider2.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDivider2.Name = "GeneralDivider2";
            this.GeneralDivider2.Size = new System.Drawing.Size(315, 2);
            this.GeneralDivider2.TabIndex = 16;
            // 
            // GeneralMatchWinnerLabel
            // 
            this.GeneralMatchWinnerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralMatchWinnerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GeneralMatchWinnerLabel.Location = new System.Drawing.Point(5, 163);
            this.GeneralMatchWinnerLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralMatchWinnerLabel.Name = "GeneralMatchWinnerLabel";
            this.GeneralMatchWinnerLabel.Size = new System.Drawing.Size(316, 16);
            this.GeneralMatchWinnerLabel.TabIndex = 39;
            this.GeneralMatchWinnerLabel.Text = "-";
            this.GeneralMatchWinnerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName4, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage6, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage7, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage8, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage9, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerImage10, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName5, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName6, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName7, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName8, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName9, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.GeneralPlayerName10, 3, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 189);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(316, 154);
            this.tableLayoutPanel1.TabIndex = 28;
            // 
            // GeneralPlayerImage1
            // 
            this.GeneralPlayerImage1.Location = new System.Drawing.Point(3, 3);
            this.GeneralPlayerImage1.Name = "GeneralPlayerImage1";
            this.GeneralPlayerImage1.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage1.TabIndex = 41;
            this.GeneralPlayerImage1.TabStop = false;
            // 
            // GeneralPlayerImage2
            // 
            this.GeneralPlayerImage2.Location = new System.Drawing.Point(3, 33);
            this.GeneralPlayerImage2.Name = "GeneralPlayerImage2";
            this.GeneralPlayerImage2.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage2.TabIndex = 42;
            this.GeneralPlayerImage2.TabStop = false;
            // 
            // GeneralPlayerImage3
            // 
            this.GeneralPlayerImage3.Location = new System.Drawing.Point(3, 63);
            this.GeneralPlayerImage3.Name = "GeneralPlayerImage3";
            this.GeneralPlayerImage3.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage3.TabIndex = 43;
            this.GeneralPlayerImage3.TabStop = false;
            // 
            // GeneralPlayerImage4
            // 
            this.GeneralPlayerImage4.Location = new System.Drawing.Point(3, 93);
            this.GeneralPlayerImage4.Name = "GeneralPlayerImage4";
            this.GeneralPlayerImage4.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage4.TabIndex = 44;
            this.GeneralPlayerImage4.TabStop = false;
            // 
            // GeneralPlayerName1
            // 
            this.GeneralPlayerName1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName1.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName1.Location = new System.Drawing.Point(35, 8);
            this.GeneralPlayerName1.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName1.Name = "GeneralPlayerName1";
            this.GeneralPlayerName1.ReadOnly = true;
            this.GeneralPlayerName1.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName1.TabIndex = 43;
            this.GeneralPlayerName1.Text = "-";
            // 
            // GeneralPlayerName2
            // 
            this.GeneralPlayerName2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName2.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName2.Location = new System.Drawing.Point(35, 38);
            this.GeneralPlayerName2.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName2.Name = "GeneralPlayerName2";
            this.GeneralPlayerName2.ReadOnly = true;
            this.GeneralPlayerName2.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName2.TabIndex = 44;
            this.GeneralPlayerName2.Text = "-";
            // 
            // GeneralPlayerImage5
            // 
            this.GeneralPlayerImage5.Location = new System.Drawing.Point(3, 123);
            this.GeneralPlayerImage5.Name = "GeneralPlayerImage5";
            this.GeneralPlayerImage5.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage5.TabIndex = 45;
            this.GeneralPlayerImage5.TabStop = false;
            // 
            // GeneralPlayerName3
            // 
            this.GeneralPlayerName3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName3.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName3.Location = new System.Drawing.Point(35, 68);
            this.GeneralPlayerName3.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName3.Name = "GeneralPlayerName3";
            this.GeneralPlayerName3.ReadOnly = true;
            this.GeneralPlayerName3.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName3.TabIndex = 45;
            this.GeneralPlayerName3.Text = "-";
            // 
            // GeneralPlayerName4
            // 
            this.GeneralPlayerName4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName4.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName4.Location = new System.Drawing.Point(35, 98);
            this.GeneralPlayerName4.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName4.Name = "GeneralPlayerName4";
            this.GeneralPlayerName4.ReadOnly = true;
            this.GeneralPlayerName4.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName4.TabIndex = 46;
            this.GeneralPlayerName4.Text = "-";
            // 
            // GeneralPlayerImage6
            // 
            this.GeneralPlayerImage6.Location = new System.Drawing.Point(289, 3);
            this.GeneralPlayerImage6.Name = "GeneralPlayerImage6";
            this.GeneralPlayerImage6.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage6.TabIndex = 46;
            this.GeneralPlayerImage6.TabStop = false;
            // 
            // GeneralPlayerImage7
            // 
            this.GeneralPlayerImage7.Location = new System.Drawing.Point(289, 33);
            this.GeneralPlayerImage7.Name = "GeneralPlayerImage7";
            this.GeneralPlayerImage7.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage7.TabIndex = 47;
            this.GeneralPlayerImage7.TabStop = false;
            // 
            // GeneralPlayerImage8
            // 
            this.GeneralPlayerImage8.Location = new System.Drawing.Point(289, 63);
            this.GeneralPlayerImage8.Name = "GeneralPlayerImage8";
            this.GeneralPlayerImage8.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage8.TabIndex = 48;
            this.GeneralPlayerImage8.TabStop = false;
            // 
            // GeneralPlayerImage9
            // 
            this.GeneralPlayerImage9.Location = new System.Drawing.Point(289, 93);
            this.GeneralPlayerImage9.Name = "GeneralPlayerImage9";
            this.GeneralPlayerImage9.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage9.TabIndex = 49;
            this.GeneralPlayerImage9.TabStop = false;
            // 
            // GeneralPlayerImage10
            // 
            this.GeneralPlayerImage10.Location = new System.Drawing.Point(289, 123);
            this.GeneralPlayerImage10.Name = "GeneralPlayerImage10";
            this.GeneralPlayerImage10.Size = new System.Drawing.Size(24, 24);
            this.GeneralPlayerImage10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.GeneralPlayerImage10.TabIndex = 50;
            this.GeneralPlayerImage10.TabStop = false;
            // 
            // GeneralPlayerName5
            // 
            this.GeneralPlayerName5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName5.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName5.Location = new System.Drawing.Point(35, 130);
            this.GeneralPlayerName5.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName5.Name = "GeneralPlayerName5";
            this.GeneralPlayerName5.ReadOnly = true;
            this.GeneralPlayerName5.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName5.TabIndex = 51;
            this.GeneralPlayerName5.Text = "-";
            // 
            // GeneralPlayerName6
            // 
            this.GeneralPlayerName6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName6.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName6.Location = new System.Drawing.Point(188, 8);
            this.GeneralPlayerName6.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName6.Name = "GeneralPlayerName6";
            this.GeneralPlayerName6.ReadOnly = true;
            this.GeneralPlayerName6.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName6.TabIndex = 52;
            this.GeneralPlayerName6.Text = "-";
            this.GeneralPlayerName6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GeneralPlayerName7
            // 
            this.GeneralPlayerName7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName7.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName7.Location = new System.Drawing.Point(188, 38);
            this.GeneralPlayerName7.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName7.Name = "GeneralPlayerName7";
            this.GeneralPlayerName7.ReadOnly = true;
            this.GeneralPlayerName7.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName7.TabIndex = 53;
            this.GeneralPlayerName7.Text = "-";
            this.GeneralPlayerName7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GeneralPlayerName8
            // 
            this.GeneralPlayerName8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName8.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName8.Location = new System.Drawing.Point(188, 68);
            this.GeneralPlayerName8.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName8.Name = "GeneralPlayerName8";
            this.GeneralPlayerName8.ReadOnly = true;
            this.GeneralPlayerName8.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName8.TabIndex = 54;
            this.GeneralPlayerName8.Text = "-";
            this.GeneralPlayerName8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GeneralPlayerName9
            // 
            this.GeneralPlayerName9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName9.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName9.Location = new System.Drawing.Point(188, 98);
            this.GeneralPlayerName9.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName9.Name = "GeneralPlayerName9";
            this.GeneralPlayerName9.ReadOnly = true;
            this.GeneralPlayerName9.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName9.TabIndex = 55;
            this.GeneralPlayerName9.Text = "-";
            this.GeneralPlayerName9.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GeneralPlayerName10
            // 
            this.GeneralPlayerName10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralPlayerName10.BackColor = System.Drawing.SystemColors.Window;
            this.GeneralPlayerName10.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GeneralPlayerName10.Location = new System.Drawing.Point(188, 130);
            this.GeneralPlayerName10.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralPlayerName10.Name = "GeneralPlayerName10";
            this.GeneralPlayerName10.ReadOnly = true;
            this.GeneralPlayerName10.Size = new System.Drawing.Size(93, 13);
            this.GeneralPlayerName10.TabIndex = 56;
            this.GeneralPlayerName10.Text = "-";
            this.GeneralPlayerName10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GeneralDebugDumpJsonButton
            // 
            this.GeneralDebugDumpJsonButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralDebugDumpJsonButton.Location = new System.Drawing.Point(5, 353);
            this.GeneralDebugDumpJsonButton.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDebugDumpJsonButton.Name = "GeneralDebugDumpJsonButton";
            this.GeneralDebugDumpJsonButton.Size = new System.Drawing.Size(75, 23);
            this.GeneralDebugDumpJsonButton.TabIndex = 7;
            this.GeneralDebugDumpJsonButton.Text = "Dump JSON";
            this.GeneralDebugDumpJsonButton.UseVisualStyleBackColor = true;
            this.GeneralDebugDumpJsonButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // GeneralGameStatsButton
            // 
            this.GeneralGameStatsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameStatsButton.Enabled = false;
            this.GeneralGameStatsButton.Location = new System.Drawing.Point(90, 353);
            this.GeneralGameStatsButton.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameStatsButton.Name = "GeneralGameStatsButton";
            this.GeneralGameStatsButton.Size = new System.Drawing.Size(75, 23);
            this.GeneralGameStatsButton.TabIndex = 29;
            this.GeneralGameStatsButton.Text = "Details...";
            this.GeneralGameStatsButton.UseVisualStyleBackColor = true;
            // 
            // GeneralStartReplayButton
            // 
            this.GeneralStartReplayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralStartReplayButton.Location = new System.Drawing.Point(175, 353);
            this.GeneralStartReplayButton.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralStartReplayButton.Name = "GeneralStartReplayButton";
            this.GeneralStartReplayButton.Size = new System.Drawing.Size(75, 23);
            this.GeneralStartReplayButton.TabIndex = 30;
            this.GeneralStartReplayButton.Text = "Play replay";
            this.GeneralStartReplayButton.UseVisualStyleBackColor = true;
            this.GeneralStartReplayButton.Click += new System.EventHandler(this.GeneralStartReplayButton_Click);
            // 
            // MatchTab
            // 
            this.MatchTab.Controls.Add(this.flowLayoutPanel1);
            this.MatchTab.Location = new System.Drawing.Point(4, 22);
            this.MatchTab.Name = "MatchTab";
            this.MatchTab.Padding = new System.Windows.Forms.Padding(3);
            this.MatchTab.Size = new System.Drawing.Size(340, 397);
            this.MatchTab.TabIndex = 3;
            this.MatchTab.Text = "Match";
            this.MatchTab.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.MatchIDLabel);
            this.flowLayoutPanel1.Controls.Add(this.MatchIDDataLabel);
            this.flowLayoutPanel1.Controls.Add(this.SetMatchIDButton);
            this.flowLayoutPanel1.Controls.Add(this.GameRegionLabel);
            this.flowLayoutPanel1.Controls.Add(this.GameRegionDataLabel);
            this.flowLayoutPanel1.Controls.Add(this.GameModeLabel);
            this.flowLayoutPanel1.Controls.Add(this.GameModeDataLabel);
            this.flowLayoutPanel1.Controls.Add(this.GameMapLabel);
            this.flowLayoutPanel1.Controls.Add(this.GameMapDataLabel);
            this.flowLayoutPanel1.Controls.Add(this.GameDateLabel);
            this.flowLayoutPanel1.Controls.Add(this.GameDateDataLabel);
            this.flowLayoutPanel1.Controls.Add(this.GeneralDivider3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(334, 391);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // MatchIDLabel
            // 
            this.MatchIDLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.MatchIDLabel.Location = new System.Drawing.Point(5, 5);
            this.MatchIDLabel.Margin = new System.Windows.Forms.Padding(5);
            this.MatchIDLabel.Name = "MatchIDLabel";
            this.MatchIDLabel.Size = new System.Drawing.Size(76, 13);
            this.MatchIDLabel.TabIndex = 29;
            this.MatchIDLabel.Text = "Match ID:";
            // 
            // MatchIDDataLabel
            // 
            this.MatchIDDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.MatchIDDataLabel.Location = new System.Drawing.Point(91, 5);
            this.MatchIDDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.MatchIDDataLabel.Name = "MatchIDDataLabel";
            this.MatchIDDataLabel.Size = new System.Drawing.Size(149, 13);
            this.MatchIDDataLabel.TabIndex = 30;
            this.MatchIDDataLabel.Text = "-";
            // 
            // SetMatchIDButton
            // 
            this.SetMatchIDButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.SetMatchIDButton.Enabled = false;
            this.SetMatchIDButton.Location = new System.Drawing.Point(245, 0);
            this.SetMatchIDButton.Margin = new System.Windows.Forms.Padding(0);
            this.SetMatchIDButton.Name = "SetMatchIDButton";
            this.SetMatchIDButton.Size = new System.Drawing.Size(75, 23);
            this.SetMatchIDButton.TabIndex = 31;
            this.SetMatchIDButton.Text = "Select...";
            this.SetMatchIDButton.UseVisualStyleBackColor = true;
            // 
            // GameRegionLabel
            // 
            this.GameRegionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameRegionLabel.Location = new System.Drawing.Point(5, 28);
            this.GameRegionLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameRegionLabel.Name = "GameRegionLabel";
            this.GameRegionLabel.Size = new System.Drawing.Size(76, 13);
            this.GameRegionLabel.TabIndex = 36;
            this.GameRegionLabel.Text = "Region:";
            // 
            // GameRegionDataLabel
            // 
            this.GameRegionDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameRegionDataLabel.Location = new System.Drawing.Point(91, 28);
            this.GameRegionDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameRegionDataLabel.Name = "GameRegionDataLabel";
            this.GameRegionDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GameRegionDataLabel.TabIndex = 37;
            this.GameRegionDataLabel.Text = "-";
            // 
            // GameModeLabel
            // 
            this.GameModeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameModeLabel.Location = new System.Drawing.Point(5, 51);
            this.GameModeLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameModeLabel.Name = "GameModeLabel";
            this.GameModeLabel.Size = new System.Drawing.Size(76, 13);
            this.GameModeLabel.TabIndex = 27;
            this.GameModeLabel.Text = "Game Mode:";
            // 
            // GameModeDataLabel
            // 
            this.GameModeDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameModeDataLabel.Location = new System.Drawing.Point(91, 51);
            this.GameModeDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameModeDataLabel.Name = "GameModeDataLabel";
            this.GameModeDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GameModeDataLabel.TabIndex = 28;
            this.GameModeDataLabel.Text = "-";
            // 
            // GameMapLabel
            // 
            this.GameMapLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameMapLabel.Location = new System.Drawing.Point(5, 74);
            this.GameMapLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameMapLabel.Name = "GameMapLabel";
            this.GameMapLabel.Size = new System.Drawing.Size(76, 13);
            this.GameMapLabel.TabIndex = 34;
            this.GameMapLabel.Text = "Map:";
            // 
            // GameMapDataLabel
            // 
            this.GameMapDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameMapDataLabel.Location = new System.Drawing.Point(91, 74);
            this.GameMapDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameMapDataLabel.Name = "GameMapDataLabel";
            this.GameMapDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GameMapDataLabel.TabIndex = 35;
            this.GameMapDataLabel.Text = "-";
            // 
            // GameDateLabel
            // 
            this.GameDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameDateLabel.Location = new System.Drawing.Point(5, 97);
            this.GameDateLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameDateLabel.Name = "GameDateLabel";
            this.GameDateLabel.Size = new System.Drawing.Size(76, 13);
            this.GameDateLabel.TabIndex = 32;
            this.GameDateLabel.Text = "Date:";
            // 
            // GameDateDataLabel
            // 
            this.GameDateDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameDateDataLabel.Location = new System.Drawing.Point(91, 97);
            this.GameDateDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GameDateDataLabel.Name = "GameDateDataLabel";
            this.GameDateDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GameDateDataLabel.TabIndex = 33;
            this.GameDateDataLabel.Text = "-";
            // 
            // GeneralDivider3
            // 
            this.GeneralDivider3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeneralDivider3.Location = new System.Drawing.Point(5, 120);
            this.GeneralDivider3.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDivider3.Name = "GeneralDivider3";
            this.GeneralDivider3.Size = new System.Drawing.Size(315, 2);
            this.GeneralDivider3.TabIndex = 38;
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
            this.AboutLogoPictureBox.Image = global::ROFLPlayer.Properties.Resources.iconBMP;
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
            this.Load += new System.EventHandler(this.DetailForm_Load);
            this.MainTabControl.ResumeLayout(false);
            this.GeneralTab.ResumeLayout(false);
            this.GeneralFlowLayout.ResumeLayout(false);
            this.GeneralFlowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralGamePictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralPlayerImage10)).EndInit();
            this.MatchTab.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
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
        private System.Windows.Forms.Button GeneralDebugDumpJsonButton;
        private System.Windows.Forms.PictureBox GeneralGamePictureBox;
        private System.Windows.Forms.Label GeneralGameLengthLabel;
        private System.Windows.Forms.Label GeneralGameVersionLabel;
        private System.Windows.Forms.Label GeneralDivider2;
        private System.Windows.Forms.Button GeneralGameStatsButton;
        private System.Windows.Forms.Button GeneralStartReplayButton;
        private System.Windows.Forms.TabPage MatchTab;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label MatchIDLabel;
        private System.Windows.Forms.Label MatchIDDataLabel;
        private System.Windows.Forms.Button SetMatchIDButton;
        private System.Windows.Forms.Label GameRegionLabel;
        private System.Windows.Forms.Label GameRegionDataLabel;
        private System.Windows.Forms.Label GameModeLabel;
        private System.Windows.Forms.Label GameModeDataLabel;
        private System.Windows.Forms.Label GameMapLabel;
        private System.Windows.Forms.Label GameMapDataLabel;
        private System.Windows.Forms.Label GameDateLabel;
        private System.Windows.Forms.Label GameDateDataLabel;
        private System.Windows.Forms.Label GeneralDivider3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label GeneralMatchWinnerLabel;
        private System.Windows.Forms.PictureBox GeneralPlayerImage1;
        private System.Windows.Forms.PictureBox GeneralPlayerImage2;
        private System.Windows.Forms.PictureBox GeneralPlayerImage3;
        private System.Windows.Forms.PictureBox GeneralPlayerImage4;
        private System.Windows.Forms.PictureBox GeneralPlayerImage5;
        private System.Windows.Forms.PictureBox GeneralPlayerImage6;
        private System.Windows.Forms.PictureBox GeneralPlayerImage7;
        private System.Windows.Forms.PictureBox GeneralPlayerImage8;
        private System.Windows.Forms.PictureBox GeneralPlayerImage9;
        private System.Windows.Forms.PictureBox GeneralPlayerImage10;
        private System.Windows.Forms.TextBox GeneralGameVersionDataLabel;
        private System.Windows.Forms.TextBox GeneralGameLengthDataLabel;
        private System.Windows.Forms.TextBox GeneralGameFileLabel;
        private System.Windows.Forms.TextBox GeneralPlayerName1;
        private System.Windows.Forms.TextBox GeneralPlayerName2;
        private System.Windows.Forms.TextBox GeneralPlayerName3;
        private System.Windows.Forms.TextBox GeneralPlayerName4;
        private System.Windows.Forms.TextBox GeneralPlayerName5;
        private System.Windows.Forms.TextBox GeneralPlayerName6;
        private System.Windows.Forms.TextBox GeneralPlayerName7;
        private System.Windows.Forms.TextBox GeneralPlayerName8;
        private System.Windows.Forms.TextBox GeneralPlayerName9;
        private System.Windows.Forms.TextBox GeneralPlayerName10;
        private System.Windows.Forms.Label GeneralGameMatchIDLabel;
        private System.Windows.Forms.TextBox GeneralGameMatchIDData;
    }
}