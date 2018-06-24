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
    public struct PlayerInfo
    {
        public string Champion;
        public string Name;

        public string Team;
        public string Win;
    }

    public struct FileBaseData
    {
        public long GameLength;
        public string GameVersion;
        public PlayerInfo[] BluePlayers;
        public PlayerInfo[] PurplePlayers;
        public string WonGame;
    }

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
                form.Controls.Find("GeneralGameLengthDataLabel", true)[0].Text = ((float)data.GameLength / 60000.0).ToString("0.00");
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
