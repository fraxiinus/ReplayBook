using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using Rofl.Parser;
using ROFLPlayer.Utilities;
using System.Drawing;

namespace ROFLPlayer.Managers
{

    public class DetailWindowManager
    {
        public static string GetReplayFilename(string path)
        {
            return Path.GetFileName(path);
        }

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

        public static void PopulatePlayerData(ReplayMatchMetadata data, Form form)
        {
            var playernames =
                from player in data.Players
                select (string)player["NAME"];

            form.BeginInvoke((Action)(() =>
            {
                ((ComboBox)form.Controls.Find("PlayerSelectComboBox", true)[0]).Items.AddRange(playernames.ToArray());
            }));
        }

        public static void PopulateGeneralReplayData(ReplayHeader data, Form form)
        {
            var map = GameDetailsReader.GetMapType(data);
            var maptask = ImageDownloader.GetMinimapImageAsync(map);

            form.BeginInvoke((Action)(async () =>
            {
                form.Controls.Find("GeneralGameVersionDataLabel", true)[0].Text = data.MatchMetadata.GameVersion;
                var time = ((decimal)(data.MatchMetadata.GameDuration / 1000) / 60);
                var minutes = (int)time;
                var seconds = (int)((time % 1.0m) * 60);
                form.Controls.Find("GeneralGameLengthDataLabel", true)[0].Text = $"{minutes} minutes and {seconds} seconds";
                form.Controls.Find("GeneralGameMatchIDData", true)[0].Text = data.MatchHeader.MatchId.ToString();
                var mapimg = (PictureBox)form.Controls.Find($"GeneralGamePictureBox", true)[0];
                new ToolTip().SetToolTip(mapimg, map.ToString());

                var imgpath = await maptask;

                if (!string.IsNullOrEmpty(imgpath))
                {
                    mapimg.WaitOnLoad = false;
                    mapimg.LoadAsync(imgpath);
                }
            }));


            var blueplayers =
                (from player in data.MatchMetadata.Players
                where player["TEAM"].ToString() == "100"
                select player).DefaultIfEmpty();

            var purpleplayers =
                (from player in data.MatchMetadata.Players
                 where player["TEAM"].ToString() == "200"
                select player).DefaultIfEmpty();

            string wongame = "No Contest";

            if (blueplayers.ElementAt(0) != null)
            {
                if(blueplayers.ElementAt(0)["WIN"].ToString().ToUpper() == "WIN")
                {
                    wongame = "Blue Victory";
                }
                else
                {
                    wongame = "Purple Victory";
                }

                var counter = 1;
                foreach (var player in blueplayers)
                {
                    var getimgtask = ImageDownloader.GetChampionIconImageAsync(player["SKIN"].ToString());

                    form.BeginInvoke((Action)(async () => {
                        var namelabel = form.Controls.Find($"GeneralPlayerName{counter}", true)[0];
                        namelabel.Text = player["NAME"].ToString();

                        var champimg = (PictureBox)form.Controls.Find($"GeneralPlayerImage{counter}", true)[0];
                        new ToolTip().SetToolTip(champimg, player["SKIN"].ToString());

                        if (player["NAME"].ToString().ToUpper() == RoflSettings.Default.Username.ToUpper())
                        {
                            namelabel.Font = new System.Drawing.Font(namelabel.Font.FontFamily, namelabel.Font.Size, System.Drawing.FontStyle.Bold);
                        }

                        counter++;

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
            }

            if(purpleplayers.ElementAt(0) != null)
            {
                if (purpleplayers.ElementAt(0)["WIN"].ToString().ToUpper() == "WIN")
                {
                    wongame = "Purple Victory";
                }
                else
                {
                    wongame = "Blue Victory";
                }

                var counter = 6;
                foreach (var player in purpleplayers)
                {
                    var getimgtask = ImageDownloader.GetChampionIconImageAsync(player["SKIN"].ToString());

                    form.BeginInvoke((Action)(async () => {
                        var namelabel = form.Controls.Find($"GeneralPlayerName{counter}", true)[0];
                        namelabel.Text = player["NAME"].ToString();

                        var champimg = (PictureBox)form.Controls.Find($"GeneralPlayerImage{counter}", true)[0];
                        new ToolTip().SetToolTip(champimg, player["SKIN"].ToString());

                        if (player["NAME"].ToString().ToUpper() == RoflSettings.Default.Username.ToUpper())
                        {
                            namelabel.Font = new System.Drawing.Font(namelabel.Font.FontFamily, namelabel.Font.Size, System.Drawing.FontStyle.Bold);
                        }
                        counter++;
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
            }

            form.BeginInvoke((Action)(() => {
                form.Controls.Find("GeneralMatchWinnerLabel", true)[0].Text = wongame;
            }));

            return;
        }

        public static void PopulatePlayerStatsData(JToken player, Form form)
        {
            var getimgtask = ImageDownloader.GetChampionIconImageAsync(player["SKIN"].ToString());
            var item0task = ImageDownloader.GetItemImageAsync(player["ITEM0"].ToObject<int>());
            var item1task = ImageDownloader.GetItemImageAsync(player["ITEM3"].ToObject<int>());
            var item2task = ImageDownloader.GetItemImageAsync(player["ITEM1"].ToObject<int>());
            var item3task = ImageDownloader.GetItemImageAsync(player["ITEM4"].ToObject<int>());
            var item4task = ImageDownloader.GetItemImageAsync(player["ITEM2"].ToObject<int>());
            var item5task = ImageDownloader.GetItemImageAsync(player["ITEM5"].ToObject<int>());
            var item6task = ImageDownloader.GetItemImageAsync(player["ITEM6"].ToObject<int>());

            form.BeginInvoke((Action)(async () =>
            {
                ///// General Information
                var champimage = (PictureBox)form.Controls.Find("PlayerStatsChampImage", true)[0];

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

                var champlabel = (TextBox)form.Controls.Find("PlayerStatsChampName", true)[0];
                champlabel.Text = player["SKIN"].ToString();

                var levellabel = (TextBox)form.Controls.Find("PlayerStatsChampLevel", true)[0];
                levellabel.Text = $"Level {player["LEVEL"].ToString()}";

                var kdalabel = (TextBox)form.Controls.Find("PlayerStatsKDA", true)[0];
                kdalabel.Text = $"{player["CHAMPIONS_KILLED"].ToString()} / {player["NUM_DEATHS"].ToString()} / {player["ASSISTS"].ToString()}";

                var cslabel = (TextBox)form.Controls.Find("PlayerStatsCreeps", true)[0];
                cslabel.Text = $"{player["MINIONS_KILLED"].ToString()} CS";

                ///// Player Gold
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

                var itemboxes =
                    (from Control boxes in allboxes
                    where boxes.Name.Contains("PlayerItemImage")
                    select boxes).Cast<PictureBox>().ToArray();

                var item0path = await item0task;
                if (!string.IsNullOrEmpty(item0path))
                {
                    itemboxes[0].WaitOnLoad = false;
                    itemboxes[0].LoadAsync(item0path);
                }

                var item1path = await item1task;
                if (!string.IsNullOrEmpty(item1path))
                {
                    itemboxes[1].WaitOnLoad = false;
                    itemboxes[1].LoadAsync(item1path);
                }

                var item2path = await item2task;
                if (!string.IsNullOrEmpty(item2path))
                {
                    itemboxes[2].WaitOnLoad = false;
                    itemboxes[2].LoadAsync(item2path);
                }

                var item3path = await item3task;
                if (!string.IsNullOrEmpty(item3path))
                {
                    itemboxes[3].WaitOnLoad = false;
                    itemboxes[3].LoadAsync(item3path);
                }

                var item4path = await item4task;
                if (!string.IsNullOrEmpty(item4path))
                {
                    itemboxes[4].WaitOnLoad = false;
                    itemboxes[4].LoadAsync(item4path);
                }

                var item5path = await item5task;
                if (!string.IsNullOrEmpty(item5path))
                {
                    itemboxes[5].WaitOnLoad = false;
                    itemboxes[5].LoadAsync(item5path);
                }

                var item6path = await item6task;
                if (!string.IsNullOrEmpty(item6path))
                {
                    itemboxes[6].WaitOnLoad = false;
                    itemboxes[6].LoadAsync(item6path);
                }
            }));
        }

        public static async Task<bool> WriteReplayHeaderToFile(string path, ReplayHeader header)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(JsonConvert.SerializeObject(header));
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
