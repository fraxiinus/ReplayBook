using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using Rofl.Parser;


namespace ROFLPlayer.Lib
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

        public async static Task PopulateGeneralReplayData(ReplayHeader data, Form form)
        {
            var map = LeagueManager.GetMapType(data);
            var maptask = FileManager.GetMinimapImage(map);

            form.BeginInvoke((Action)(async () =>
            {
                form.Controls.Find("GeneralGameVersionDataLabel", true)[0].Text = data.MatchMetadata.GameVersion;
                var time = ((decimal)(data.MatchMetadata.GameDuration / 1000) / 60);
                var minutes = (int)time;
                var seconds = (int)((time % 1.0m) * 60);
                form.Controls.Find("GeneralGameLengthDataLabel", true)[0].Text = $"{minutes} minutes and {seconds} seconds";
                form.Controls.Find("GeneralGameMatchIDData", true)[0].Text = data.MatchHeader.MatchID.ToString();
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

            if(blueplayers == null && purpleplayers == null)
            {
                return;
            }

            string wongame = "";
            if(blueplayers.ElementAt(0)["WIN"].ToString().ToUpper() == "WIN")
            {
                wongame = "Blue Victory!";
            }
            else
            {
                wongame = "Purple Victory!";
            }

            if (blueplayers == null)
            { }
            else
            {
                form.BeginInvoke((Action)(() => {
                    form.Controls.Find("GeneralMatchWinnerLabel", true)[0].Text = wongame;
                }));

                var counter = 1;
                foreach (var player in blueplayers)
                {
                    var getimgtask = FileManager.GetChampionIconImage(player["SKIN"].ToString());

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
                        
                    }));
                }
            }

            if (purpleplayers == null)
            { }
            else
            {
                var counter = 6;
                foreach (var player in purpleplayers)
                {
                    var getimgtask = FileManager.GetChampionIconImage(player["SKIN"].ToString());

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
                        
                    }));
                }
            }
            return;
        }

        public static async Task<RunResult<string>> WriteReplayHeaderToFile(string path, ReplayHeader header)
        {
            var result = new RunResult<string> { Success = false, Result= null };
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(JsonConvert.SerializeObject(header));
                }
            }
            catch(Exception ex)
            {
                result.Message = "Writing json to file: " + ex.Message;
            }

            result.Success = true;
            result.Result = path;
            result.Message = "Dumped header to file";

            return result;
        }
    }
}
