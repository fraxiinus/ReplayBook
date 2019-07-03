using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using Rofl.Parsers.Models;
using ROFLPlayer.Utilities;
using System.Drawing;
using System.Collections.Generic;

namespace ROFLPlayer.Managers
{

    public class DetailWindowManager
    {
        /*
        public static long FindMatchIDInFilename(string filename)
        {
            var match = Regex.Match(filename, "\\d{10}");
            if(match.Success)
            {
                return long.Parse(match.Value);
            }
            else
            {
                return 0;
            }
        }
        */

        /// <summary>
        /// Fill the Combo Box with player names
        /// </summary>
        /// <param name="data"></param>
        /// <param name="form"></param>
        public static void PopulatePlayerData(MatchMetadata data, Form form)
        {
            var playernames =
                from player in data.AllPlayers
                select player["NAME"];

            form.BeginInvoke((Action)(() =>
            {
                ((ComboBox)form.Controls.Find("PlayerSelectComboBox", true)[0]).Items.AddRange(playernames.ToArray());
            }));
        }

        /// <summary>
        /// Fill out the list of player names and images. Set the victory text
        /// </summary>
        /// <param name="data"></param>
        /// <param name="form"></param>
        public static void PopulateGeneralReplayData(ReplayHeader data, Form form)
        {
            // Figure out which map the replay is for and download map image
            var map = GameDetailsReader.GetMapType(data);
            var maptask = ImageDownloader.GetMinimapImageAsync(map);

            form.BeginInvoke((Action)(async () =>
            {
                form.Controls.Find("GeneralGameVersionDataLabel", true)[0].Text = data.MatchMetadata.GameVersion;
                form.Controls.Find("GeneralGameMatchIDData", true)[0].Text = data.PayloadFields.MatchId.ToString();

                // Calculate game duration
                var time = ((decimal)(data.MatchMetadata.GameDuration / 1000) / 60);
                var minutes = (int)time;
                var seconds = (int)((time % 1.0m) * 60);
                form.Controls.Find("GeneralGameLengthDataLabel", true)[0].Text = $"{minutes} minutes and {seconds} seconds";

                // Find the map picturebox and set the tooltip
                var mapimg = (PictureBox)form.Controls.Find($"GeneralGamePictureBox", true)[0];
                new ToolTip().SetToolTip(mapimg, map.ToString());

                // Set the map image
                var imgpath = await maptask;

                if (!string.IsNullOrEmpty(imgpath))
                {
                    mapimg.WaitOnLoad = false;
                    mapimg.LoadAsync(imgpath);
                }
            }));

            // Default victory text to draw
            string wongame = "No Contest";

            // If there are any blue players
            if (data.MatchMetadata.BluePlayers.ElementAt(0) != null)
            {
                // Since we're looking at blue players first, check who won
                if(data.MatchMetadata.BluePlayers.ElementAt(0)["WIN"].ToString().ToUpper() == "WIN")
                {
                    wongame = "Blue Victory";
                }
                else
                {
                    wongame = "Red Victory";
                }

                var counter = 1; // Counter used to match player number to UI views
                foreach (var player in data.MatchMetadata.BluePlayers)
                {
                    // Kick off task to download champion image
                    var getimgtask = ImageDownloader.GetChampionIconImageAsync(player["SKIN"].ToString());

                    form.BeginInvoke((Action)(async () => {
                        var namelabel = form.Controls.Find($"GeneralPlayerName{counter}", true)[0];
                        namelabel.Text = player["NAME"].ToString();

                        // Set the tooltip for champion image
                        var champimg = (PictureBox)form.Controls.Find($"GeneralPlayerImage{counter}", true)[0];
                        new ToolTip().SetToolTip(champimg, player["SKIN"].ToString());

                        // Bold the name of the user
                        if (player["NAME"].ToString().ToUpper() == RoflSettings.Default.Username.ToUpper())
                        {
                            namelabel.Font = new System.Drawing.Font(namelabel.Font.FontFamily, namelabel.Font.Size, System.Drawing.FontStyle.Bold);
                        }

                        counter++;

                        // Set the champion image
                        var imgpath = await getimgtask;

                        if (!string.IsNullOrEmpty(imgpath))
                        {
                            champimg.WaitOnLoad = false;
                            champimg.LoadAsync(imgpath);
                        }
                        else
                        {
                            champimg.Image = champimg.ErrorImage;
                        }
                    }));
                }

                // Hide labels for extra player spots
                for(int i = data.MatchMetadata.BluePlayers.Count() + 1; i <= 6; i++)
                {
                    var namelabel = form.Controls.Find($"GeneralPlayerName{i}", true)[0];
                    namelabel.Visible = false;
                }
            }

            // If there are any red players
            if(data.MatchMetadata.RedPlayers.ElementAt(0) != null)
            {
                // Maybe there were no blue players, so lets see if red won (this seems redundant...)
                if (data.MatchMetadata.RedPlayers.ElementAt(0)["WIN"].ToString().ToUpper() == "WIN")
                {
                    wongame = "Purple Victory";
                }
                else
                {
                    wongame = "Blue Victory";
                }

                var counter = 7; // Counter used to match player number to UI views
                foreach (var player in data.MatchMetadata.RedPlayers)
                {
                    // Kick off task to download champion image
                    var getimgtask = ImageDownloader.GetChampionIconImageAsync(player["SKIN"].ToString());

                    form.BeginInvoke((Action)(async () =>
                    {
                        var namelabel = form.Controls.Find($"GeneralPlayerName{counter}", true)[0];
                        namelabel.Text = player["NAME"].ToString();

                        // Set the tooltip for champion image
                        var champimg = (PictureBox)form.Controls.Find($"GeneralPlayerImage{counter}", true)[0];
                        new ToolTip().SetToolTip(champimg, player["SKIN"].ToString());

                        // Bold the name of the user
                        if (player["NAME"].ToString().ToUpper() == RoflSettings.Default.Username.ToUpper())
                        {
                            namelabel.Font = new System.Drawing.Font(namelabel.Font.FontFamily, namelabel.Font.Size, System.Drawing.FontStyle.Bold);
                        }

                        counter++;

                        // Set the champion image
                        var imgpath = await getimgtask;

                        if (!string.IsNullOrEmpty(imgpath))
                        {
                            champimg.WaitOnLoad = false;
                            champimg.LoadAsync(imgpath);
                        }
                        else
                        {
                            champimg.Image = champimg.ErrorImage;
                        }

                    }));
                }

                // Hide labels for extra player spots
                for (int i = data.MatchMetadata.RedPlayers.Count() + 7; i <= 12; i++)
                {
                    var namelabel = form.Controls.Find($"GeneralPlayerName{i}", true)[0];
                    namelabel.Visible = false;
                }

            }

            // We should know who won by now
            form.BeginInvoke((Action)(() => {
                form.Controls.Find("GeneralMatchWinnerLabel", true)[0].Text = wongame;
            }));
        }

        /// <summary>
        /// Fill out player stats
        /// </summary>
        /// <param name="player"></param>
        /// <param name="form"></param>
        public static void PopulatePlayerStatsData(Dictionary<string, string> player, Form form)
        {
            // We should already have downloaded the champion image, double check. Will return if we do.
            var getimgtask = ImageDownloader.GetChampionIconImageAsync(player["SKIN"].ToString());

            // Setup tasks that will be used to download item images
            Task<string>[] itemTasks = new Task<string>[7];

            for (int taskCounter = 0; taskCounter < 7; taskCounter++)
            {
                itemTasks[taskCounter] = ImageDownloader.GetItemImageAsync(int.Parse(player["ITEM" + taskCounter]));
            }

            form.BeginInvoke((Action)(async () =>
            {
                ///// General Information
                var champimage = (PictureBox)form.Controls.Find("PlayerStatsChampImage", true)[0];

                // set champion image
                var imgpath = await getimgtask;
                if (!string.IsNullOrEmpty(imgpath))
                {
                    champimage.WaitOnLoad = false;
                    champimage.LoadAsync(imgpath);
                }
                else
                {
                    champimage.Image = champimage.ErrorImage;
                }

                // Set victory text
                var victorylabel = (TextBox)form.Controls.Find("PlayerStatswin", true)[0];
                if(player["WIN"].ToString() == "Fail")
                {
                    victorylabel.Text = "Defeat";
                    victorylabel.ForeColor = Color.Red;
                }
                else
                {
                    victorylabel.Text = "Victory!";
                    victorylabel.ForeColor = Color.Green;
                }

                ///// Champion, Level, KDA, CS
                var champlabel = (TextBox)form.Controls.Find("PlayerStatsChampName", true)[0];
                champlabel.Text = player["SKIN"].ToString();

                var levellabel = (TextBox)form.Controls.Find("PlayerStatsChampLevel", true)[0];
                levellabel.Text = $"Level {player["LEVEL"].ToString()}";

                var kdalabel = (TextBox)form.Controls.Find("PlayerStatsKDA", true)[0];
                kdalabel.Text = $"{player["CHAMPIONS_KILLED"].ToString()} / {player["NUM_DEATHS"].ToString()} / {player["ASSISTS"].ToString()}";

                var cslabel = (TextBox)form.Controls.Find("PlayerStatsCreeps", true)[0];
                cslabel.Text = $"{player["MINIONS_KILLED"].ToString()} CS";

                ///// Player Gold, Neutral Kills, Turrets
                var goldearnedlabel = (TextBox)form.Controls.Find("PlayerGoldEarned", true)[0];
                if(int.TryParse(player["GOLD_EARNED"].ToString(), out int goldearned))
                {
                    goldearnedlabel.Text = goldearned.ToString("N0");
                }

                var goldspendlabel = (TextBox)form.Controls.Find("PlayerGoldSpent", true)[0];
                if (int.TryParse(player["GOLD_SPENT"].ToString(), out int goldspent))
                {
                    goldspendlabel.Text = goldspent.ToString("N0");
                }

                var neutralkillslabel = (TextBox)form.Controls.Find("PlayerGoldNeutralCreeps", true)[0];
                neutralkillslabel.Text = player["NEUTRAL_MINIONS_KILLED"].ToString();

                var towerskilledlabel = (TextBox)form.Controls.Find("PlayerGoldTowerKills", true)[0];
                towerskilledlabel.Text = player["TURRETS_KILLED"].ToString();

                ///// Player Misc Stats Table

                var damagetochampslabel = (TextBox)form.Controls.Find("PlayerTotalDamageToChampions", true)[0];
                if (int.TryParse(player["TOTAL_DAMAGE_DEALT_TO_CHAMPIONS"].ToString(), out int totaldamagetochamps))
                {
                    damagetochampslabel.Text = totaldamagetochamps.ToString("N0");
                }

                var damagetoobjlabel = (TextBox)form.Controls.Find("PlayerTotalDamageToObjectives", true)[0];
                if (int.TryParse(player["TOTAL_DAMAGE_DEALT_TO_OBJECTIVES"].ToString(), out int totaldamagetoobjective))
                {
                    damagetoobjlabel.Text = totaldamagetoobjective.ToString("N0");
                }

                var damagetotowerlabel = (TextBox)form.Controls.Find("PlayerTotalDamageToTurrets", true)[0];
                if (int.TryParse(player["TOTAL_DAMAGE_DEALT_TO_TURRETS"].ToString(), out int totaldamagetotower))
                {
                    damagetotowerlabel.Text = totaldamagetotower.ToString("N0");
                }

                var totaldamagelabel = (TextBox)form.Controls.Find("PlayerTotalDamageDealt", true)[0];
                if (int.TryParse(player["TOTAL_DAMAGE_DEALT"].ToString(), out int totaldamage))
                {
                    totaldamagelabel.Text = totaldamage.ToString("N0");
                }

                var totalheallabel = (TextBox)form.Controls.Find("PlayerDamageHealed", true)[0];
                if (int.TryParse(player["TOTAL_HEAL"].ToString(), out int totalheal))
                {
                    totalheallabel.Text = totalheal.ToString("N0");
                }

                var totaltakenlabel = (TextBox)form.Controls.Find("PlayerDamageTaken", true)[0];
                if (int.TryParse(player["TOTAL_DAMAGE_TAKEN"].ToString(), out int totaltaken))
                {
                    totaltakenlabel.Text = totaltaken.ToString("N0");
                }

                var visionscorelabel = (TextBox)form.Controls.Find("PlayerVisionScore", true)[0];
                if (int.TryParse(player["VISION_SCORE"].ToString(), out int visionscore))
                {
                    visionscorelabel.Text = visionscore.ToString("N0");
                }

                var wardsplacedlabel = (TextBox)form.Controls.Find("PlayerWardsPlaced", true)[0];
                if (int.TryParse(player["WARD_PLACED"].ToString(), out int wardsplaced))
                {
                    wardsplacedlabel.Text = wardsplaced.ToString("N0");
                }

                ///// Player Inventory
                var allboxes = form.Controls.Find("PlayerSpellsItemsTable", true)[0].Controls;

                // Grab all item image boxes
                var itemboxes =
                    (from Control boxes in allboxes
                    where boxes.Name.Contains("PlayerItemImage")
                    select boxes).Cast<PictureBox>().ToArray();

                // Set item images
                for (int loadImageCounter = 0; loadImageCounter < 7; loadImageCounter++)
                {
                    var itemPath = await itemTasks[loadImageCounter];
                    if(!string.IsNullOrEmpty(itemPath) && !itemPath.Equals("EMPTY"))
                    {
                        itemboxes[loadImageCounter].WaitOnLoad = false;
                        itemboxes[loadImageCounter].LoadAsync(itemPath);
                    }
                    else if (itemPath.Equals("EMPTY"))
                    {
                        itemboxes[loadImageCounter].Image = null;
                    }
                    else
                    {
                        itemboxes[loadImageCounter].Image = itemboxes[loadImageCounter].ErrorImage;
                    }
                }
            }));
        }

        /// <summary>
        /// Output all header data into a JSON file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static async Task<bool> WriteReplayHeaderToFile(string path, ReplayHeader header)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(JsonConvert.SerializeObject(header));
                }
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Given region name (e.g. NA, EUW), return region endpoint name for URLs
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public static string GetRegionEndpointName(string regionName)
        {
            switch (regionName)
            {
                case "BR":
                    return "BR1";
                case "EUNE":
                    return "EUN1";
                case "EUW":
                    return "EUW1";
                case "JP":
                    return "JP1";
                case "KR":
                    return "KR";
                case "LAN":
                    return "LA1";
                case "LAS":
                    return "LA2";
                case "NA":
                    return "NA1";
                case "OCE":
                    return "OC1";
                case "TR":
                    return "TR1";
                case "RU":
                    return "RU";
                case "PBE":
                    return "PBE1";
                default:
                    return null;
            }
        }
    }
}
