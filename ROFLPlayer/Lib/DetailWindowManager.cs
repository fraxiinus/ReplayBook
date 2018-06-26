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

        public static Task<bool> PopulateGeneralReplayData(FileBaseData data, Form form)
        {
            form.BeginInvoke((Action)(() =>
            {
                form.Controls.Find("GeneralGameVersionDataLabel", true)[0].Text = data.GameVersion;
                var time = ((decimal)(data.GameLength / 1000) / 60);
                var minutes = (int)time;
                var seconds = (int)((time % 1.0m) * 60);
                form.Controls.Find("GeneralGameLengthDataLabel", true)[0].Text = $"{minutes} minutes and {seconds} seconds";
            }));

            if (data.BluePlayers == null)
            { }
            else
            {
                form.BeginInvoke((Action)(() => {
                    form.Controls.Find("GeneralMatchWinnerLabel", true)[0].Text = data.WonGame;
                }));

                var counter = 1;
                foreach (var player in data.BluePlayers)
                {
                    form.BeginInvoke((Action)(() => {
                        var namelabel = form.Controls.Find($"GeneralPlayerName{counter}", true)[0];
                        namelabel.Text = player.Name;

                        var champimage = (PictureBox)form.Controls.Find($"GeneralPlayerImage{counter}", true)[0];
                        counter++;
                        new ToolTip().SetToolTip(champimage, player.Champion);

                        try
                        {
                            champimage.WaitOnLoad = false;

                            var imgtask = Task.Run<string>(() => FileManager.GetChampionIconImage(player.Champion));

                            imgtask.ContinueWith(x => 
                            {
                                if (!string.IsNullOrEmpty(x.Result))
                                {
                                    champimage.LoadAsync(x.Result);
                                }
                            });

                        }
                        catch (WebException) { }

                        if (player.Name.ToUpper() == RoflSettings.Default.Username.ToUpper())
                        {
                            namelabel.Font = new System.Drawing.Font(namelabel.Font.FontFamily, namelabel.Font.Size, System.Drawing.FontStyle.Bold);
                        }
                    }));

                }
            }

            if (data.PurplePlayers == null)
            { }
            else
            {
                var counter = 6;
                foreach (var player in data.PurplePlayers)
                {
                    form.BeginInvoke((Action)(() =>
                    {
                        var namelabel = form.Controls.Find($"GeneralPlayerName{counter}", true)[0];
                        namelabel.Text = player.Name;

                        var champimage = (PictureBox)form.Controls.Find($"GeneralPlayerImage{counter}", true)[0];
                        counter++;
                        new ToolTip().SetToolTip(champimage, player.Champion);

                        try
                        {
                            champimage.WaitOnLoad = false;
                            var imgtask = Task.Run<string>(() => FileManager.GetChampionIconImage(player.Champion));

                            imgtask.ContinueWith(x =>
                            {
                                if (!string.IsNullOrEmpty(x.Result))
                                {
                                    champimage.LoadAsync(x.Result);
                                }
                            });

                        }
                        catch (WebException) { }

                        if (player.Name.ToUpper() == RoflSettings.Default.Username.ToUpper())
                        {
                            namelabel.Font = new System.Drawing.Font(namelabel.Font.FontFamily, namelabel.Font.Size, System.Drawing.FontStyle.Bold);
                        }
                        
                    }));
                }
            }

            return Task.FromResult<bool>(true);
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

        public static Task<FileBaseData> GetFileData(string path)
        {
            
            FileBaseData returnVal = new FileBaseData();

            var basicData = JObject.Parse(LeagueManager.GetReplayJSON(path));

            returnVal.GameLength = (long)basicData["gameLength"];
            returnVal.GameVersion = (string)basicData["gameVersion"];

            var playerData = JArray.Parse(((string)basicData["statsJson"]).Replace(@"\", ""));

            var blueplayers =
                (from user in playerData
                 where (string)user["TEAM"] == "100"
                 select new PlayerInfo { Name = (string)user["NAME"], Champion = (string)user["SKIN"], Team = (string)user["TEAM"],  Win = (string)user["WIN"]}).ToArray();

            var purpleplayers =
                (from user in playerData
                 where (string)user["TEAM"] == "200"
                 select new PlayerInfo { Name = (string)user["NAME"], Champion = (string)user["SKIN"], Team = (string)user["TEAM"], Win = (string)user["WIN"] }).ToArray();


            if (blueplayers.Length > 0)
            {
                if(string.Equals(blueplayers[0].Win, "Win"))
                {
                    returnVal.WonGame = "Blue Victory";
                }
                else
                {
                    returnVal.WonGame = "Purple Victory";
                }

                returnVal.BluePlayers = blueplayers;
            }
            else
            {
                returnVal.BluePlayers = null;
            }

            if(purpleplayers.Length > 0)
            {
                returnVal.PurplePlayers = purpleplayers;
            }
            else
            {
                returnVal.PurplePlayers = null;
            }

            return Task.FromResult<FileBaseData>(returnVal);
        }
    }
}
