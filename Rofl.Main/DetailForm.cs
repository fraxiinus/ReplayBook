using Rofl.Executables;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Logger;
using Rofl.Main.Managers;
using Rofl.Reader.Models;
using Rofl.Requests;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rofl.Main
{
    public partial class DetailForm : Form
    {
        private ReplayFile _replayFile;
        private RequestManager _requestManager;
        private ExeManager _exeManager;
        private ReplayPlayer _replayPlayer;
        private Scribe _logger;

        public DetailForm(ReplayFile replayFile, RequestManager requestManager, ExeManager exeManager, ReplayPlayer replayPlayer, Scribe scribe)
        {
            _replayFile = replayFile;
            _requestManager = requestManager;
            _exeManager = exeManager;
            _replayPlayer = replayPlayer;
            _logger = scribe;

            InitializeComponent();

            // Load split button menu for game executables
            LeagueExecutable[] listOfExecs = _exeManager.GetExecutables();

            // No items? Don't load the menu
            if (listOfExecs.Count() > 0)
            {
                var execMenu = new ContextMenuStrip
                {
                    ShowCheckMargin = false,
                    ShowImageMargin = false,
                };

                execMenu.ItemClicked += new ToolStripItemClickedEventHandler(GeneralStartReplayMenuItem_Click);

                foreach (var item in listOfExecs)
                {
                    execMenu.Items.Add(item.Name);
                }

                this.GeneralPlayReplaySplitButton.Menu = execMenu;
            }
        }

        private async void DetailForm_Load(object sender, EventArgs e)
        {
            // Load in compatibility mode
            if(_replayFile.Type != REPLAYTYPES.ROFL)
            {
                this.Text = this.Text + " - Compatibility Mode";
                this.GeneralPlayReplaySplitButton.Enabled = false;
            }

            // Set version text in about tab
            this.AboutVersionLabel.Text = RoflSettings.Default.VersionString;
            this.GeneralGameFileLabel.Text = _replayFile.Name;

            await _requestManager.SetDataDragonVersionAsync(_replayFile.Data.MatchMetadata.GameVersion);

            DetailWindowManager.PopulatePlayerData(_replayFile.Data.MatchMetadata, this);
            DetailWindowManager.PopulateGeneralReplayData(_requestManager, _replayFile.Data, this);
        }

        /// <summary>
        /// Actions for when user changes tabs
        /// </summary>
        private void MainTabControl_IndexChanged(object sender, EventArgs e)
        {
            switch (MainTabControl.SelectedIndex)
            {
                case 0: // General Tab
                    break;
                case 1: // Stats Tab
                    // If window just started
                    if (PlayerSelectComboBox.SelectedIndex == -1)
                    {
                        // Check if player name is part of the players
                        if (PlayerSelectComboBox.Items.Contains(RoflSettings.Default.Username))
                        {
                            // Select plater
                            PlayerSelectComboBox.SelectedItem = RoflSettings.Default.Username;
                        } else
                        {
                            // Select first
                            PlayerSelectComboBox.SelectedIndex = 0;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        ///////////////////// General Tab Methods

        private void MainCancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /*
        private void MainOkButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        */

        private void GeneralStartReplayMenuItem_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            GeneralPlayReplaySplitButton.Enabled = false;
            StartReplay(e.ClickedItem.Text);
        }

        private void GeneralStartReplaySplitButton_Click(object sender, EventArgs e)
        {
            GeneralPlayReplaySplitButton.Enabled = false;
            StartReplay();
        }

        private void StartReplay(string execName = "default")
        {
            LeagueExecutable exec = null;

            // Get default exec or specified exec
            if(execName.Equals("default"))
            {
                // Start update form with default
                var result = new UpdateSplashForm(_exeManager).ShowDialog();

                if(result == DialogResult.OK)
                {
                    exec = _exeManager.GetDefaultExecutable();
                }
                else
                {
                    // Failed to get exec, stop
                    this.GeneralPlayReplaySplitButton.Enabled = true;
                    return;
                }
            } else
            {
                // Start update form with target
                var result = new UpdateSplashForm(_exeManager, execName).ShowDialog();

                if (result == DialogResult.OK)
                {
                    exec = _exeManager.GetExecutable(execName);
                }
                else
                {
                    // Failed to get exec, stop
                    this.GeneralPlayReplaySplitButton.Enabled = true;
                    return;
                }
            }

            // This really shouldn't happen, but just to be safe
            if(exec == null)
            {
                MessageBox.Show($"Could not find executable data {execName}\nPlease run ROFL Player and check the executables", "Failed to start replay",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Start League of Legends,
            var playtask = Task.Run(() =>
            {
                //ReplayManager.StartReplay(_fileInfo.Location, exec.TargetPath);
                _replayPlayer.Play(exec, _replayFile.Location);
            }).ContinueWith((t) =>
            // When the user closes the game
            {
                this.BeginInvoke((Action)(() =>
                {
                    if (t.IsFaulted)
                    {
                        _logger.Error(this.GetType().ToString(), t.Exception.ToString());
                        MessageBox.Show("Failed to play replay! Check logs for detailed information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    GeneralPlayReplaySplitButton.Enabled = true;
                }));
            });
        }

        private void GeneralGameViewOnlineButton_Click(object sender, EventArgs e)
        {
            var matchId = _replayFile.Data.PayloadFields.MatchId;
            string regionEndpoint = DetailWindowManager.GetRegionEndpointName(RoflSettings.Default.Region);

            MessageBox.Show("This feature is still a work in progress! I need more information from different regions. Let me know if you encounter any problems.\n\n" +
                            $"Region: {RoflSettings.Default.Region}\n" +
                            $"Region Endpoint: {regionEndpoint}", "Beta Feature", MessageBoxButtons.OK, MessageBoxIcon.Information);

            System.Diagnostics.Process.Start(@"https://matchhistory.na.leagueoflegends.com/en/#match-details/" + regionEndpoint + "/" + matchId);
        }

        ///////////////////// Player Tab Methods
        private void PlayerSelectComboBox_SelectChanged(object sender, EventArgs e)
        {
            var selectedplayername = PlayerSelectComboBox.Text;

            // Find the selected player dictionary
            var player =
                (from qplayer in _replayFile.Data.MatchMetadata.AllPlayers
                 where qplayer["NAME"].ToString().ToUpper() == selectedplayername.ToUpper()
                 select qplayer).FirstOrDefault();

            // Set images to null, so they appear blank
            PlayerStatsChampImage.Image = null;
            PlayerItemImage1.Image = null;
            PlayerItemImage4.Image = null;
            PlayerItemImage2.Image = null;
            PlayerItemImage5.Image = null;
            PlayerItemImage3.Image = null;
            PlayerItemImage6.Image = null;
            PlayerItemImage7.Image = null;

            // Call the manager to populate data
            DetailWindowManager.PopulatePlayerStatsData(_requestManager, player, this);
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

            if (!string.IsNullOrEmpty(_replayFile.Location))
            {
                var outputfile = Path.Combine(Path.GetDirectoryName(_replayFile.Location), Path.GetFileNameWithoutExtension(_replayFile.Location) + ".json");
                var success = await DetailWindowManager.WriteReplayHeaderToFile(outputfile, _replayFile.Data);
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
            System.Diagnostics.Process.Start(@"https://github.com/leeanchu/ROFL-Player");
        }
    }
}
