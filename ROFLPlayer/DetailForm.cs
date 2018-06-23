using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ROFLPlayer.Lib;

namespace ROFLPlayer
{
    public partial class DetailForm : Form
    {
        private string replaypath = "";

        public DetailForm(string replayPath)
        {
            replaypath = replayPath;
            InitializeComponent();
            if (LeagueManager.CheckReplayFile(replayPath))
            {
                // Read replay file async and get the required data
                var readtask = Task.Run(() => DetailWindowManager.GetFileData(replaypath));

                var filename = DetailWindowManager.GetReplayFilename(replaypath);

                GeneralGameFileLabel.Text = filename;

                // Read replay file name for match ID
                // Query RIOT API for match information
                // Otherwise set label and enable button

                var filedata = readtask.Result;

                GeneralGameVersionDataLabel.Text = filedata.GameVersion;
                GeneralGameLengthDataLabel.Text = ((float)filedata.GameLength / 60000.0).ToString("0.00");
                GeneralUserInfoNameLabel.Text = filedata.Champion;
                GeneralUserInfoScoreLabel.Text = $"{filedata.Kills} / {filedata.Deaths} / {filedata.Assists}";
                GeneralUserInfoCreepScoreLabel.Text = filedata.CreepScore.ToString();
                GeneralUserInfoGoldLabel.Text = filedata.GoldEarned.ToString();
                GeneralUserInfoWinLabel.Text = filedata.WonGame;
            }
            else
            {
                MessageBox.Show("File is not a valid replay.", "Invalid Replay File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private void MainCancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void MainOkButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LeagueManager.DumpReplayJSON(replaypath);
            //LeagueManager.DumpJSON(replaypath);
        }
    }
}
