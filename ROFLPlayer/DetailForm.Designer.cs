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
            this.GeneralGameFileLabel = new System.Windows.Forms.Label();
            this.GeneralDivider1 = new System.Windows.Forms.Label();
            this.GeneralGameVersionLabel = new System.Windows.Forms.Label();
            this.GeneralGameVersionDataLabel = new System.Windows.Forms.Label();
            this.GeneralGameLengthLabel = new System.Windows.Forms.Label();
            this.GeneralGameLengthDataLabel = new System.Windows.Forms.Label();
            this.GeneralDivider2 = new System.Windows.Forms.Label();
            this.GeneralMatchIDLabel = new System.Windows.Forms.Label();
            this.GeneralMatchIDDataLabel = new System.Windows.Forms.Label();
            this.GeneralSetMatchIDButton = new System.Windows.Forms.Button();
            this.GeneralGameRegionLabel = new System.Windows.Forms.Label();
            this.GeneralGameRegionDataLabel = new System.Windows.Forms.Label();
            this.GeneralGameModeLabel = new System.Windows.Forms.Label();
            this.GeneralGameModeDataLabel = new System.Windows.Forms.Label();
            this.GeneralGameMapLabel = new System.Windows.Forms.Label();
            this.GeneralGameMapDataLabel = new System.Windows.Forms.Label();
            this.GeneralGameDateLabel = new System.Windows.Forms.Label();
            this.GeneralGameDateDataLabel = new System.Windows.Forms.Label();
            this.GeneralDivider3 = new System.Windows.Forms.Label();
            this.GeneralUserChampionPictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.GeneralUserInfoNameLabel = new System.Windows.Forms.Label();
            this.GeneralUserInfoScoreLabel = new System.Windows.Forms.Label();
            this.GeneralUserInfoCreepScoreLabel = new System.Windows.Forms.Label();
            this.GeneralUserInfoGoldLabel = new System.Windows.Forms.Label();
            this.GeneralDebugDumpJsonButton = new System.Windows.Forms.Button();
            this.GeneralGameStatsButton = new System.Windows.Forms.Button();
            this.GeneralStartReplayButton = new System.Windows.Forms.Button();
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
            this.GeneralUserInfoWinLabel = new System.Windows.Forms.Label();
            this.MainTabControl.SuspendLayout();
            this.GeneralTab.SuspendLayout();
            this.GeneralFlowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralGamePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralUserChampionPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
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
            this.GeneralFlowLayout.Controls.Add(this.GeneralGamePictureBox);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameFileLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralDivider1);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameVersionLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameVersionDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameLengthLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameLengthDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralDivider2);
            this.GeneralFlowLayout.Controls.Add(this.GeneralMatchIDLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralMatchIDDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralSetMatchIDButton);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameRegionLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameRegionDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameModeLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameModeDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameMapLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameMapDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameDateLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralGameDateDataLabel);
            this.GeneralFlowLayout.Controls.Add(this.GeneralDivider3);
            this.GeneralFlowLayout.Controls.Add(this.GeneralUserChampionPictureBox);
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
            this.GeneralGameFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameFileLabel.AutoSize = true;
            this.GeneralGameFileLabel.Location = new System.Drawing.Point(65, 28);
            this.GeneralGameFileLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameFileLabel.Name = "GeneralGameFileLabel";
            this.GeneralGameFileLabel.Size = new System.Drawing.Size(10, 13);
            this.GeneralGameFileLabel.TabIndex = 11;
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
            this.GeneralGameVersionDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameVersionDataLabel.Location = new System.Drawing.Point(91, 82);
            this.GeneralGameVersionDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameVersionDataLabel.Name = "GeneralGameVersionDataLabel";
            this.GeneralGameVersionDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GeneralGameVersionDataLabel.TabIndex = 13;
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
            this.GeneralGameLengthDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameLengthDataLabel.Location = new System.Drawing.Point(91, 105);
            this.GeneralGameLengthDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameLengthDataLabel.Name = "GeneralGameLengthDataLabel";
            this.GeneralGameLengthDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GeneralGameLengthDataLabel.TabIndex = 10;
            this.GeneralGameLengthDataLabel.Text = "-";
            // 
            // GeneralDivider2
            // 
            this.GeneralDivider2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeneralDivider2.Location = new System.Drawing.Point(5, 128);
            this.GeneralDivider2.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDivider2.Name = "GeneralDivider2";
            this.GeneralDivider2.Size = new System.Drawing.Size(315, 2);
            this.GeneralDivider2.TabIndex = 16;
            // 
            // GeneralMatchIDLabel
            // 
            this.GeneralMatchIDLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralMatchIDLabel.Location = new System.Drawing.Point(5, 140);
            this.GeneralMatchIDLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralMatchIDLabel.Name = "GeneralMatchIDLabel";
            this.GeneralMatchIDLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralMatchIDLabel.TabIndex = 17;
            this.GeneralMatchIDLabel.Text = "Match ID:";
            // 
            // GeneralMatchIDDataLabel
            // 
            this.GeneralMatchIDDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralMatchIDDataLabel.Location = new System.Drawing.Point(91, 140);
            this.GeneralMatchIDDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralMatchIDDataLabel.Name = "GeneralMatchIDDataLabel";
            this.GeneralMatchIDDataLabel.Size = new System.Drawing.Size(149, 13);
            this.GeneralMatchIDDataLabel.TabIndex = 18;
            this.GeneralMatchIDDataLabel.Text = "-";
            // 
            // GeneralSetMatchIDButton
            // 
            this.GeneralSetMatchIDButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralSetMatchIDButton.Enabled = false;
            this.GeneralSetMatchIDButton.Location = new System.Drawing.Point(245, 135);
            this.GeneralSetMatchIDButton.Margin = new System.Windows.Forms.Padding(0);
            this.GeneralSetMatchIDButton.Name = "GeneralSetMatchIDButton";
            this.GeneralSetMatchIDButton.Size = new System.Drawing.Size(75, 23);
            this.GeneralSetMatchIDButton.TabIndex = 19;
            this.GeneralSetMatchIDButton.Text = "Select...";
            this.GeneralSetMatchIDButton.UseVisualStyleBackColor = true;
            // 
            // GeneralGameRegionLabel
            // 
            this.GeneralGameRegionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameRegionLabel.Location = new System.Drawing.Point(5, 163);
            this.GeneralGameRegionLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameRegionLabel.Name = "GeneralGameRegionLabel";
            this.GeneralGameRegionLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralGameRegionLabel.TabIndex = 24;
            this.GeneralGameRegionLabel.Text = "Region:";
            // 
            // GeneralGameRegionDataLabel
            // 
            this.GeneralGameRegionDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameRegionDataLabel.Location = new System.Drawing.Point(91, 163);
            this.GeneralGameRegionDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameRegionDataLabel.Name = "GeneralGameRegionDataLabel";
            this.GeneralGameRegionDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GeneralGameRegionDataLabel.TabIndex = 25;
            this.GeneralGameRegionDataLabel.Text = "-";
            // 
            // GeneralGameModeLabel
            // 
            this.GeneralGameModeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameModeLabel.Location = new System.Drawing.Point(5, 186);
            this.GeneralGameModeLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameModeLabel.Name = "GeneralGameModeLabel";
            this.GeneralGameModeLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralGameModeLabel.TabIndex = 14;
            this.GeneralGameModeLabel.Text = "Game Mode:";
            // 
            // GeneralGameModeDataLabel
            // 
            this.GeneralGameModeDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameModeDataLabel.Location = new System.Drawing.Point(91, 186);
            this.GeneralGameModeDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameModeDataLabel.Name = "GeneralGameModeDataLabel";
            this.GeneralGameModeDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GeneralGameModeDataLabel.TabIndex = 15;
            this.GeneralGameModeDataLabel.Text = "-";
            // 
            // GeneralGameMapLabel
            // 
            this.GeneralGameMapLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameMapLabel.Location = new System.Drawing.Point(5, 209);
            this.GeneralGameMapLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameMapLabel.Name = "GeneralGameMapLabel";
            this.GeneralGameMapLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralGameMapLabel.TabIndex = 22;
            this.GeneralGameMapLabel.Text = "Map:";
            // 
            // GeneralGameMapDataLabel
            // 
            this.GeneralGameMapDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameMapDataLabel.Location = new System.Drawing.Point(91, 209);
            this.GeneralGameMapDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameMapDataLabel.Name = "GeneralGameMapDataLabel";
            this.GeneralGameMapDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GeneralGameMapDataLabel.TabIndex = 23;
            this.GeneralGameMapDataLabel.Text = "-";
            // 
            // GeneralGameDateLabel
            // 
            this.GeneralGameDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameDateLabel.Location = new System.Drawing.Point(5, 232);
            this.GeneralGameDateLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameDateLabel.Name = "GeneralGameDateLabel";
            this.GeneralGameDateLabel.Size = new System.Drawing.Size(76, 13);
            this.GeneralGameDateLabel.TabIndex = 20;
            this.GeneralGameDateLabel.Text = "Date:";
            // 
            // GeneralGameDateDataLabel
            // 
            this.GeneralGameDateDataLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralGameDateDataLabel.Location = new System.Drawing.Point(91, 232);
            this.GeneralGameDateDataLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameDateDataLabel.Name = "GeneralGameDateDataLabel";
            this.GeneralGameDateDataLabel.Size = new System.Drawing.Size(230, 13);
            this.GeneralGameDateDataLabel.TabIndex = 21;
            this.GeneralGameDateDataLabel.Text = "-";
            // 
            // GeneralDivider3
            // 
            this.GeneralDivider3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GeneralDivider3.Location = new System.Drawing.Point(5, 255);
            this.GeneralDivider3.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralDivider3.Name = "GeneralDivider3";
            this.GeneralDivider3.Size = new System.Drawing.Size(315, 2);
            this.GeneralDivider3.TabIndex = 26;
            // 
            // GeneralUserChampionPictureBox
            // 
            this.GeneralUserChampionPictureBox.Location = new System.Drawing.Point(5, 267);
            this.GeneralUserChampionPictureBox.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralUserChampionPictureBox.Name = "GeneralUserChampionPictureBox";
            this.GeneralUserChampionPictureBox.Size = new System.Drawing.Size(50, 50);
            this.GeneralUserChampionPictureBox.TabIndex = 27;
            this.GeneralUserChampionPictureBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.GeneralUserInfoWinLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.GeneralUserInfoNameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.GeneralUserInfoScoreLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.GeneralUserInfoCreepScoreLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.GeneralUserInfoGoldLabel, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(65, 267);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(256, 52);
            this.tableLayoutPanel1.TabIndex = 28;
            // 
            // GeneralUserInfoNameLabel
            // 
            this.GeneralUserInfoNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralUserInfoNameLabel.AutoSize = true;
            this.GeneralUserInfoNameLabel.Location = new System.Drawing.Point(5, 5);
            this.GeneralUserInfoNameLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralUserInfoNameLabel.Name = "GeneralUserInfoNameLabel";
            this.GeneralUserInfoNameLabel.Size = new System.Drawing.Size(54, 13);
            this.GeneralUserInfoNameLabel.TabIndex = 29;
            this.GeneralUserInfoNameLabel.Text = "Champion";
            // 
            // GeneralUserInfoScoreLabel
            // 
            this.GeneralUserInfoScoreLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralUserInfoScoreLabel.AutoSize = true;
            this.GeneralUserInfoScoreLabel.Location = new System.Drawing.Point(5, 31);
            this.GeneralUserInfoScoreLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralUserInfoScoreLabel.Name = "GeneralUserInfoScoreLabel";
            this.GeneralUserInfoScoreLabel.Size = new System.Drawing.Size(54, 13);
            this.GeneralUserInfoScoreLabel.TabIndex = 30;
            this.GeneralUserInfoScoreLabel.Text = "0 / 0 / 0";
            // 
            // GeneralUserInfoCreepScoreLabel
            // 
            this.GeneralUserInfoCreepScoreLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralUserInfoCreepScoreLabel.AutoSize = true;
            this.GeneralUserInfoCreepScoreLabel.Location = new System.Drawing.Point(69, 31);
            this.GeneralUserInfoCreepScoreLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralUserInfoCreepScoreLabel.Name = "GeneralUserInfoCreepScoreLabel";
            this.GeneralUserInfoCreepScoreLabel.Size = new System.Drawing.Size(13, 13);
            this.GeneralUserInfoCreepScoreLabel.TabIndex = 31;
            this.GeneralUserInfoCreepScoreLabel.Text = "0";
            // 
            // GeneralUserInfoGoldLabel
            // 
            this.GeneralUserInfoGoldLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralUserInfoGoldLabel.AutoSize = true;
            this.GeneralUserInfoGoldLabel.Location = new System.Drawing.Point(92, 31);
            this.GeneralUserInfoGoldLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralUserInfoGoldLabel.Name = "GeneralUserInfoGoldLabel";
            this.GeneralUserInfoGoldLabel.Size = new System.Drawing.Size(159, 13);
            this.GeneralUserInfoGoldLabel.TabIndex = 32;
            this.GeneralUserInfoGoldLabel.Text = "0";
            // 
            // GeneralDebugDumpJsonButton
            // 
            this.GeneralDebugDumpJsonButton.Location = new System.Drawing.Point(5, 329);
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
            this.GeneralGameStatsButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralGameStatsButton.Enabled = false;
            this.GeneralGameStatsButton.Location = new System.Drawing.Point(90, 329);
            this.GeneralGameStatsButton.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralGameStatsButton.Name = "GeneralGameStatsButton";
            this.GeneralGameStatsButton.Size = new System.Drawing.Size(75, 23);
            this.GeneralGameStatsButton.TabIndex = 29;
            this.GeneralGameStatsButton.Text = "Details...";
            this.GeneralGameStatsButton.UseVisualStyleBackColor = true;
            // 
            // GeneralStartReplayButton
            // 
            this.GeneralStartReplayButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.GeneralStartReplayButton.Location = new System.Drawing.Point(175, 329);
            this.GeneralStartReplayButton.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralStartReplayButton.Name = "GeneralStartReplayButton";
            this.GeneralStartReplayButton.Size = new System.Drawing.Size(75, 23);
            this.GeneralStartReplayButton.TabIndex = 30;
            this.GeneralStartReplayButton.Text = "Play replay";
            this.GeneralStartReplayButton.UseVisualStyleBackColor = true;
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
            // GeneralUserInfoWinLabel
            // 
            this.GeneralUserInfoWinLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GeneralUserInfoWinLabel.AutoSize = true;
            this.GeneralUserInfoWinLabel.Location = new System.Drawing.Point(92, 5);
            this.GeneralUserInfoWinLabel.Margin = new System.Windows.Forms.Padding(5);
            this.GeneralUserInfoWinLabel.Name = "GeneralUserInfoWinLabel";
            this.GeneralUserInfoWinLabel.Size = new System.Drawing.Size(159, 13);
            this.GeneralUserInfoWinLabel.TabIndex = 33;
            this.GeneralUserInfoWinLabel.Text = "-";
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
            this.GeneralFlowLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralGamePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeneralUserChampionPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
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
        private System.Windows.Forms.Label GeneralGameLengthDataLabel;
        private System.Windows.Forms.Label GeneralGameFileLabel;
        private System.Windows.Forms.Label GeneralGameVersionLabel;
        private System.Windows.Forms.Label GeneralGameVersionDataLabel;
        private System.Windows.Forms.Label GeneralDivider2;
        private System.Windows.Forms.Label GeneralGameModeLabel;
        private System.Windows.Forms.Label GeneralGameModeDataLabel;
        private System.Windows.Forms.Label GeneralMatchIDLabel;
        private System.Windows.Forms.Label GeneralMatchIDDataLabel;
        private System.Windows.Forms.Button GeneralSetMatchIDButton;
        private System.Windows.Forms.Label GeneralGameDateLabel;
        private System.Windows.Forms.Label GeneralGameDateDataLabel;
        private System.Windows.Forms.Label GeneralGameMapLabel;
        private System.Windows.Forms.Label GeneralGameMapDataLabel;
        private System.Windows.Forms.Label GeneralGameRegionLabel;
        private System.Windows.Forms.Label GeneralGameRegionDataLabel;
        private System.Windows.Forms.Label GeneralDivider3;
        private System.Windows.Forms.PictureBox GeneralUserChampionPictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label GeneralUserInfoNameLabel;
        private System.Windows.Forms.Label GeneralUserInfoScoreLabel;
        private System.Windows.Forms.Label GeneralUserInfoCreepScoreLabel;
        private System.Windows.Forms.Label GeneralUserInfoGoldLabel;
        private System.Windows.Forms.Button GeneralGameStatsButton;
        private System.Windows.Forms.Button GeneralStartReplayButton;
        private System.Windows.Forms.Label GeneralUserInfoWinLabel;
    }
}