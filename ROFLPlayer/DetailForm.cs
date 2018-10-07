using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ROFLPlayer.Utilities;
using ROFLPlayer.Managers;
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

            try
            {
                fileinfo = await ReplayReader.ReadReplayFileAsync(replaypath);
                ImageDownloader.SetDataDragonVersion(fileinfo.MatchMetadata.GameVersion.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Parsing Replay: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            if (fileinfo != null)
            {
                DetailWindowManager.PopulatePlayerData(fileinfo.MatchMetadata, this);
                DetailWindowManager.PopulateGeneralReplayData(fileinfo, this);
            }
            else
            {
                MessageBox.Show("Error Parsing Replay", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private void MainTabControl_IndexChanged(object sender, EventArgs e)
        {
            switch (MainTabControl.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    if (PlayerSelectComboBox.SelectedIndex == -1 && PlayerSelectComboBox.Items.Contains(RoflSettings.Default.Username))
                    {
                        PlayerSelectComboBox.SelectedItem = RoflSettings.Default.Username;
                    }
                    break;
                default:
                    break;
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
            var matchId = fileinfo.MatchHeader.MatchId;
            string regionEndpoint;

            switch (RoflSettings.Default.Region)
            {
                case "BR":
                    regionEndpoint = "BR1";
                    break;
                case "EUNE":
                    regionEndpoint = "EUN1";
                    break;
                case "EUW":
                    regionEndpoint = "EUW1";
                    break;
                case "JP":
                    regionEndpoint = "JP1";
                    break;
                case "KR":
                    regionEndpoint = "KR";
                    break;
                case "LAN":
                    regionEndpoint = "LA1";
                    break;
                case "LAS":
                    regionEndpoint = "LA2";
                    break;
                case "NA":
                    regionEndpoint = "NA1";
                    break;
                case "OCE":
                    regionEndpoint = "OC1";
                    break;
                case "TR":
                    regionEndpoint = "TR1";
                    break;
                case "RU":
                    regionEndpoint = "RU";
                    break;
                case "PBE":
                    regionEndpoint = "PBE1";
                    break;
                default:
                    regionEndpoint = null;
                    break;
            }

            System.Diagnostics.Process.Start(@"https://matchhistory.na.leagueoflegends.com/en/#match-details/" + regionEndpoint + "/" + matchId);
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

        // Resize the player champ name and KDA text box based on the length of the text
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
                var success = await DetailWindowManager.WriteReplayHeaderToFile(outputfile, fileinfo);
                if (success)
                {
                    MessageBox.Show("Dumped JSON!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to dump JSON", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AboutGithubButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/andrew1421lee/ROFL-Player");
        }
    }
}
