using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Rofl.Parser;
using ROFLPlayer.Models;
using Microsoft.Win32;


namespace ROFLPlayer.Utilities
{

    public class GameLocator
    {
        /// <summary>
        /// Searches the Windows Registry for where League of Legends is installed. Path is returned if found.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FindLeagueInstallPath(out string path)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node"))
            {
                var riotSubkeyName = (from subkeyName in key.GetSubKeyNames()
                                      where subkeyName == "Riot Games, Inc"
                                      select subkeyName).FirstOrDefault();

                if (!string.IsNullOrEmpty(riotSubkeyName))
                {
                    using (RegistryKey subkey = key.OpenSubKey($@"{riotSubkeyName}\League of Legends"))
                    {
                        if(subkey == null)
                        {
                            path = "Could not find League of Legends subkey in registry";
                            return false;
                        }
                        else
                        {
                            path = subkey.GetValue("Location").ToString();
                        }
                    }
                    RoflSettings.Default.StartFolder = path;
                    RoflSettings.Default.Save();
                    return true;
                }
            }

            path = "Could not find Riot Games, Inc subkey in registry";
            return false;
        }

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
    }
}
