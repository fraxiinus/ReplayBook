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
        }

        private void DetailForm_Load(object sender, EventArgs e)
        {
            if (LeagueManager.CheckReplayFile(replaypath))
            {

                // Read replay file async and get the required data
                var readtask = Task.Run<FileBaseData>(() => DetailWindowManager.GetFileData(replaypath));   // This is properly run in a thread!

                readtask.ContinueWith(x => { DetailWindowManager.PopulateGeneralReplayData(readtask.Result, this); });   // this is properly run in a thread!

                var filename = DetailWindowManager.GetReplayFilename(replaypath);

                GeneralGameFileLabel.Text = filename;

                // Read replay file name for match ID
                var matchid = DetailWindowManager.FindMatchIDInFilename(filename);
                if (matchid != 0)
                {
                    // Query RIOT API for match information
                    GeneralGameMatchIDData.Text = matchid.ToString();
                    
                }
                else
                {
                    // Otherwise set label and enable button
                    GeneralGameMatchIDData.Text = "Not found";
                    GeneralSetMatchIDButton.Enabled = true;
                }
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
            var result = LeagueManager.DumpReplayJSON(replaypath);
            if(result.Success)
            {
                MessageBox.Show(result.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //LeagueManager.DumpJSON(replaypath);
        }

        private void GeneralStartReplayButton_Click(object sender, EventArgs e)
        {
            var playtask = Task.Run(() => ReplayManager.StartReplay(replaypath, GeneralStartReplayButton));
        }
    }
}
