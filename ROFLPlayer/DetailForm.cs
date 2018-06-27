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
using System.IO;
using Rofl.Parser;

namespace ROFLPlayer
{
    public partial class DetailForm : Form
    {
        private string replaypath = "";
        private ReplayHeader fileinfo = null;

        public DetailForm(string replayPath)
        {
            replaypath = replayPath;
            InitializeComponent();
        }

        private async void DetailForm_Load(object sender, EventArgs e)
        {
            var filename = DetailWindowManager.GetReplayFilename(replaypath);
            GeneralGameFileLabel.Text = filename;
            //ReplayHeader parseresult = null;
            try
            {
                fileinfo = await ReplayReader.ReadReplayFileAsync(replaypath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Parsing Replay: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (fileinfo != null)
            {
                await DetailWindowManager.PopulateGeneralReplayData(fileinfo, this);
            }
            else
            {
                MessageBox.Show("Error Parsing Replay", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void button1_Click(object sender, EventArgs e)
        {
            if(fileinfo == null)
            {
                //fileinfo = (await LeagueManager.LoadAndParseReplayHeaders(replaypath)).Result;
                fileinfo = ReplayReader.ReadReplayFile(replaypath);
            }

            if(!string.IsNullOrEmpty(replaypath))
            {
                var outputfile = Path.Combine(Path.GetDirectoryName(replaypath), Path.GetFileNameWithoutExtension(replaypath) + ".json" );
                var dumpresult = await DetailWindowManager.WriteReplayHeaderToFile(outputfile, fileinfo);
                if (dumpresult.Success)
                {
                    MessageBox.Show(dumpresult.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(dumpresult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GeneralStartReplayButton_Click(object sender, EventArgs e)
        {
            var playtask = Task.Run(() => ReplayManager.StartReplay(replaypath, GeneralStartReplayButton));
        }
    }
}
