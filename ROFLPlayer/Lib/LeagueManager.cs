using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rofl.Parser;

namespace ROFLPlayer.Lib
{

    public class LeagueManager
    {
        public static bool CheckLeagueExecutable()
        {
            var lolpath = RoflSettings.Default.LoLExecLocation;

            // Check the name of the file
            if (string.IsNullOrEmpty(lolpath))
            {
                return false;
            }

            if (!lolpath.Contains("League of Legends.exe"))
            {
                return false;
            }

            // Check file exists
            if (!File.Exists(lolpath))
            {
                return false;
            }

            // Check the description of the file
            if (!string.Equals(FileVersionInfo.GetVersionInfo(lolpath).FileDescription, @"League of Legends (TM) Client"))
            {
                return false;
            }

            return true;
        }

        public static bool CheckLeagueExecutable(string lolpath)
        {

            // Check the name of the file
            if (string.IsNullOrEmpty(lolpath))
            {
                return false;
            }

            if (!lolpath.Contains("League of Legends.exe"))
            {
                return false;
            }

            // Check file exists
            if (!File.Exists(lolpath))
            {
                return false;
            }

            // Check the description of the file
            if (!string.Equals(FileVersionInfo.GetVersionInfo(lolpath).FileDescription, @"League of Legends (TM) Client"))
            {
                return false;
            }

            return true;
        }

        public static bool CheckReplayFile(string replaypath)
        {
            if (!File.Exists(replaypath)) { return false; }
            using (var filestream = new FileStream(replaypath, FileMode.Open))
            {
                var magicbuffer = new byte[4];
                var magicnumber = new byte[] { 0x52, 0x49, 0x4F, 0x54 };    // R I O T
                filestream.Read(magicbuffer, 0, 4);
                if (magicbuffer.SequenceEqual(magicnumber))
                {
                    return true;
                }
            }
            return false;
        }

        public static Maps GetMapType(ReplayHeader replay)
        {

            // Check if any players have killed jungle creeps, Rules out HA
            var JungleCheck = (from player in replay.MatchMetadata.Players
                               where player["NEUTRAL_MINIONS_KILLED"].ToObject<int>() > 0
                               select player);

            // Check if any players have placed wards, Rules out TT and HA
            //var WardCheck = (from player in replay.MatchMetadata.Players
            //                 where player["WARD_PLACED"].ToObject<int>() > 0
            //                 select player);

            if(JungleCheck.Count() > 0)
            {
                return Maps.SummonersRift;
            }
            else
            {
                return Maps.HowlingAbyss;
            }

        }
    }
}
