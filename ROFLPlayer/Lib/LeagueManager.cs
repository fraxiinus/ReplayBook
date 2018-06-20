using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;


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

            if(!lolpath.Contains("League of Legends.exe"))
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

        
    }
}
