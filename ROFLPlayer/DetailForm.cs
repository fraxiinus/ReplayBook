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
using System.Net;

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
                var matchid = DetailWindowManager.FindMatchIDInFilename(filename);
                if(matchid != 0)
                {
                    // Query RIOT API for match information
                    MatchIDDataLabel.Text = matchid.ToString();
                }
                else
                {
                    // Otherwise set label and enable button
                    MatchIDDataLabel.Text = "Could not find match ID";
                }

                var filedata = readtask.Result;
                GeneralGameVersionDataLabel.Text = filedata.GameVersion;
                GeneralGameLengthDataLabel.Text = ((float)filedata.GameLength / 60000.0).ToString("0.00");


                DetailWindowManager.PopulateGeneralGameData(filedata, this);
                
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

        private void GeneralStartReplayButton_Click(object sender, EventArgs e)
        {
            var playtask = Task.Run(() => ReplayManager.StartReplay(replaypath, GeneralStartReplayButton));
        }
    }
}
