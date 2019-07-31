using System;
using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Rofl.Executables.Utilities
{
    public class ExeTools
    {
        private readonly string _exceptionOriginName = "Rofl.Executables.Utilities.ExeTools";

        /// <summary>
        /// Tries to find league install from registry keys. 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string GetLeagueInstallPathFromRegistry()
        {
            string returnPath = null;

            // Open the windows registry
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node"))
            {
                // Try one of Riot's keys
                if (RegistryKeyExists(key, $@"Riot Games, Inc\League of Legends"))
                {
                    using (RegistryKey leagueKey = key.OpenSubKey($@"Riot Games, Inc\League of Legends"))
                    {
                        // Get the location key 
                        returnPath = leagueKey.GetValue("Path").ToString();

                        if (String.IsNullOrEmpty(returnPath) || !Directory.Exists(returnPath))
                        {
                            throw new DirectoryNotFoundException($"{_exceptionOriginName} - Path in registry does not exist");
                        }
                    }
                }
                // Try another name
                else if (RegistryKeyExists(key, $@"Riot Games\League of Legends"))
                {
                    using (RegistryKey leagueKey = key.OpenSubKey($@"Riot Games\League of Legends"))
                    {
                        // Get the location key 
                        returnPath = leagueKey.GetValue("Path").ToString();

                        if (String.IsNullOrEmpty(returnPath) || !Directory.Exists(returnPath))
                        {
                            throw new DirectoryNotFoundException($"{_exceptionOriginName} - Path in registry does not exist");
                        }
                    }
                }
                else
                {
                    throw new Exception($"{_exceptionOriginName} - Could not find League of Legends registry keys");
                }
            }

            return returnPath;
        }

        public string FindLeagueExecutablePath(string startDir)
        {
            if (string.IsNullOrEmpty(startDir))
            {
                throw new ArgumentNullException($"{_exceptionOriginName} - Input path cannot be empty");
            }
            if (!Directory.Exists(startDir))
            {
                throw new DirectoryNotFoundException($"{_exceptionOriginName} - Input path does not exist");
            }

            // Get names of folders in the directory
            var dirs = Directory.GetDirectories(startDir).Select(x => Path.GetFileName(x)).ToArray();
            if (dirs.Contains("Game"))
            {
                // Install is using new pather
                var newPath = Path.Combine(startDir, "Game", "League of Legends.exe");
                if (File.Exists(newPath) && ValidateLeagueExecutable(newPath))
                {
                    return newPath;
                }
                else
                {
                    throw new FileNotFoundException($"{_exceptionOriginName} - Could not find League of Legends.exe");
                }
            }
            else
            {
                // Install is from RADS era
                return FindLeagueExecutablePathInRADS(startDir);
            }
        }

        public bool ValidateLeagueExecutable(string filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !filePath.Contains("League of Legends.exe") || !File.Exists(filePath) || !(FileVersionInfo.GetVersionInfo(filePath).FileDescription).Equals(@"League of Legends (TM) Client"))
            {
                return false;
            }

            return true;
        }

        public string GetLeagueVersion(string filePath)
        {
            if (ValidateLeagueExecutable(filePath))
            {
                return FileVersionInfo.GetVersionInfo(filePath).FileVersion;
            }
            else
            {
                throw new FileNotFoundException($"{_exceptionOriginName} - Invalid executable");
            }
        }

        public DateTime GetLastModifiedDate(string filePath)
        {
            if(ValidateLeagueExecutable(filePath))
            {
                return (new FileInfo(filePath)).LastWriteTime;
            }
            else
            {
                throw new FileNotFoundException($"{_exceptionOriginName} - Invalid executable");
            }
        }

        private bool RegistryKeyExists(RegistryKey registry, string keyName)
        {
            try
            {
                using(RegistryKey resultKey = registry.OpenSubKey(keyName))
                {
                    if(resultKey != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private string FindLeagueExecutablePathInRADS(string startDir)
        {
            // Browse to releases folder
            var browse = Path.Combine(startDir, "RADS", "solutions", "lol_game_client_sln", "releases");
            if (!Directory.Exists(browse))
            {
                throw new DirectoryNotFoundException($"{_exceptionOriginName} - Critical League of Legends folders do not exist. Does the path " + browse + " exist?");
            }

            // Get most recent release by folder modification date
            DirectoryInfo[] folders = new DirectoryInfo(browse).GetDirectories();

            string releaseFolder = null;
            if (!folders.Any()) // No folders, something's wrong with install
            {
                releaseFolder = String.Empty;
            }
            else if (folders.Count() == 1) // Only one release, choose that one
            {
                releaseFolder = folders[0].FullName;
            }
            else // More than one, use the most recent
            {
                releaseFolder = folders.OrderBy(x => x.LastWriteTime).Last().FullName;
            }

            if (String.IsNullOrEmpty(releaseFolder))
            {
                throw new DirectoryNotFoundException($"{_exceptionOriginName} - No release folder found");
            }

            // Browse to executable
            browse = Path.Combine(browse, releaseFolder, "deploy", "League of Legends.exe");

            if (ValidateLeagueExecutable(browse))
            {
                return browse;
            }
            else
            {
                throw new FileNotFoundException($"{_exceptionOriginName} - Could not find League of Legends.exe");
            }
        }
    }
}
