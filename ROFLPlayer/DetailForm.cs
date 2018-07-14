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
using System.Threading;

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
                DetailWindowManager.PopulatePlayerData(fileinfo.MatchMetadata, this);
                await DetailWindowManager.PopulateGeneralReplayData(fileinfo, this);
            }
            else
            {
                MessageBox.Show("Error Parsing Replay", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

        }

        ///////////////////// General Tab Methods

        private void MainCancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void MainOkButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void GeneralStartReplayButton_Click(object sender, EventArgs e)
        {
            GeneralStartReplayButton.Enabled = false;
            var playtask = Task.Run(() =>
            {
                ReplayManager.StartReplay(replaypath);
            }).ContinueWith((t) =>
            {
                this.BeginInvoke((Action)(() =>
                {
                    if (t.IsFaulted)
                    {
                        MessageBox.Show("Failed to play replay: " + t.Exception.GetType().ToString() + "\n" +  t.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    GeneralStartReplayButton.Enabled = true;
                }));
            });
        }

        private void GeneralGameViewOnlineButton_Click(object sender, EventArgs e)
        {
            //
        }

        ///////////////////// Player Tab Methods
        private void PlayerSelectComboBox_SelectChanged(object sender, EventArgs e)
        {
            var selectedplayername = PlayerSelectComboBox.Text;

            var player =
                (from qplayer in fileinfo.MatchMetadata.Players
                 where qplayer["NAME"].ToString().ToUpper() == selectedplayername.ToUpper()
                 select qplayer).FirstOrDefault();

            PlayerStatsChampImage.Image = null;
            PlayerItemImage1.Image = null;
            PlayerItemImage2.Image = null;
            PlayerItemImage3.Image = null;
            PlayerItemImage4.Image = null;
            PlayerItemImage5.Image = null;
            PlayerItemImage6.Image = null;
            PlayerItemImage7.Image = null;

            DetailWindowManager.PopulatePlayerStatsData(player, this);
        }

        private void PlayerStatsChampName_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(PlayerStatsChampName.Text, PlayerStatsChampName.Font);
            PlayerStatsChampName.Width = size.Width;
        }

        private void PlayerStatsKDA_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(PlayerStatsKDA.Text, PlayerStatsKDA.Font);
            PlayerStatsKDA.Width = size.Width;
        }

        ///////////////////// Debug Methods

        private async void GeneralDebugDumpJsonButton_Click(object sender, EventArgs e)
        {
            if (fileinfo == null)
            {
                //fileinfo = (await LeagueManager.LoadAndParseReplayHeaders(replaypath)).Result;
                fileinfo = ReplayReader.ReadReplayFile(replaypath);
            }

            if (!string.IsNullOrEmpty(replaypath))
            {
                var outputfile = Path.Combine(Path.GetDirectoryName(replaypath), Path.GetFileNameWithoutExtension(replaypath) + ".json");
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

        private void AboutGithubButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/andrew1421lee/ROFL-Player");
        }
    }
}
