using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ROFLPlayer.Lib
{
    public struct FileBaseData
    {
        public long GameLength;
        public string GameVersion;
        public string Champion;
        public int CreepScore;
        public long GoldEarned;
        public string WonGame;
        public int Kills;
        public int Deaths;
        public int Assists;
    }

    public class DetailWindowManager
    {
        public static string GetReplayFilename(string path)
        {
            return Path.GetFileName(path);
        }

        public static Task<FileBaseData> GetFileData(string path)
        {
            FileBaseData returnVal = new FileBaseData();

            var basicData = JObject.Parse(LeagueManager.GetReplayJSON(path));

            returnVal.GameLength = (long)basicData["gameLength"];
            returnVal.GameVersion = (string)basicData["gameVersion"];

            var playerData = JArray.Parse(((string)basicData["statsJson"]).Replace(@"\", ""));

            var userinfo =
                (from user in playerData
                where ((string)user["NAME"]).ToUpper() == RoflSettings.Default.Username.ToUpper()
                select user).FirstOrDefault();

            if (userinfo != null)
            {
                returnVal.Champion = (string)userinfo["SKIN"];
                returnVal.CreepScore = (int)userinfo["MINIONS_KILLED"];
                returnVal.GoldEarned = (long)userinfo["GOLD_EARNED"];
                returnVal.Kills = (int)userinfo["CHAMPIONS_KILLED"];
                returnVal.Deaths = (int)userinfo["NUM_DEATHS"];
                returnVal.Assists = (int)userinfo["ASSISTS"];
                returnVal.WonGame = (string)userinfo["WIN"];
            }
            else
            {
                returnVal.Champion = null;
                returnVal.CreepScore = 0;
                returnVal.GoldEarned = 0;
                returnVal.Kills = 0;
                returnVal.Deaths = 0;
                returnVal.Assists = 0;
                returnVal.WonGame = null;
            }

            return Task.FromResult<FileBaseData>(returnVal);
        }
    }
}
