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
        /// <summary>
        /// Returns the League of Legends executable
        /// </summary>
        /// <param name="startingpath"></param>
        /// <returns></returns>
        public static string FindLeagueExecutable(string startingpath)
        {
            if(string.IsNullOrEmpty(startingpath))
            {
                throw new ArgumentNullException("Input path cannot be empty");
            }
            if(!Directory.Exists(startingpath))
            {
                throw new DirectoryNotFoundException("Input path does not exist");
            }

            // Browse to releases folder
            var browse = Path.Combine(startingpath, "RADS", "solutions", "lol_game_client_sln", "releases");
            if(!Directory.Exists(browse))
            {
                throw new DirectoryNotFoundException("Critical League of Legends folders do not exist");
            }

            var releasefolders = Directory.GetDirectories(browse);
            if(releasefolders.Count() > 1)
            {
                // Somehow choose
                throw new Exception("More than one release folder found");
            }
            else if(releasefolders.Count() == 1)
            {
                browse = Path.Combine(browse, releasefolders[0], "deploy");
            }
            else
            {
                throw new DirectoryNotFoundException("No release folder found");
            }

            browse = Path.Combine(browse, "League of Legends.exe");

            if (CheckLeagueExecutable(browse))
            {
                RoflSettings.Default.LoLExecLocation = browse;
                RoflSettings.Default.Save();
                return browse;
            }
            else
            {
                throw new FileNotFoundException("Could not find League of Legends.exe");
            }
        }

        /// <summary>
        /// Check if league of legends executable is valid
        /// </summary>
        /// <returns></returns>
        public static bool CheckLeagueExecutable()
        {
            var lolpath = RoflSettings.Default.LoLExecLocation;

            // Check the name of the file
            if (string.IsNullOrEmpty(lolpath) || !lolpath.Contains("League of Legends.exe") || !File.Exists(lolpath) || !string.Equals(FileVersionInfo.GetVersionInfo(lolpath).FileDescription, @"League of Legends (TM) Client"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if league of legends executable is valid
        /// </summary>
        /// <returns></returns>
        public static bool CheckLeagueExecutable(string lolpath)
        {

            // Check the name of the file
            if (string.IsNullOrEmpty(lolpath) || !lolpath.Contains("League of Legends.exe") || !File.Exists(lolpath) || !string.Equals(FileVersionInfo.GetVersionInfo(lolpath).FileDescription, @"League of Legends (TM) Client"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deduct the map type from game information
        /// </summary>
        /// <param name="replay"></param>
        /// <returns></returns>
        public static Maps GetMapType(ReplayHeader replay)
        {

            // Check if any players have killed jungle creeps, Rules out HA
            var JungleCheck = (from player in replay.MatchMetadata.Players
                               where player["NEUTRAL_MINIONS_KILLED"].ToObject<int>() > 0
                               select player);

            // Check if any players have placed wards, Rules out TT and HA
            var WardCheck = (from player in replay.MatchMetadata.Players
                             where player["WARD_PLACED"].ToObject<int>() > 0
                             select player);

            // Double check between TT and SR
            var DragonCheck = (from player in replay.MatchMetadata.Players
                               where player["DRAGON_KILLS"].ToObject<int>() > 0
                               select player);

            if(JungleCheck.Count() > 0)
            {
                if(WardCheck.Count() == 0 && DragonCheck.Count() == 0)
                {
                    return Maps.TwistedTreeline;
                }
                else
                {
                    return Maps.SummonersRift;
                }
                
            }
            else
            {
                return Maps.HowlingAbyss;
            }

        }
    }
}
