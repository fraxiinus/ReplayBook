using Newtonsoft.Json;
using ROFLPlayer.Models;
using System;
using System.IO;
using System.Linq;

namespace ROFLPlayer.Utilities
{
    public class ExecsManager
    {

        // Path to folder where execs are stored
        public static string ExecsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "execs");

        /// <summary>
        /// Get names of execs
        /// </summary>
        public static string[] GetSavedExecs()
        {
            // Double check if execs folder exists
            if (!Directory.Exists(ExecsFolder))
            {
                // If it doesn't create the folder
                Directory.CreateDirectory(ExecsFolder);

                // Return empty
                return new string[0];
            }

            // Get all files names in the folder
            return Directory.GetFiles(ExecsFolder).Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
        }

        /// <summary>
        /// Get exec object by name
        /// </summary>
        public static LeagueExecutable GetExec(string name)
        {
            // Create the path to supposed file
            var filePath = Path.Combine(ExecsFolder, name + ".json");

            // File does not exist, return null
            if(!File.Exists(filePath)) { return null; }

            // Deserialize file from JSON and return all text
            try
            {
                return JsonConvert.DeserializeObject<LeagueExecutable>(File.ReadAllText(filePath));
            }
            catch (Exception)
            {
                // Error occured during deserialization
                return null;
            }
        }

        /// <summary>
        /// Delete exec file by name
        /// </summary>
        public static string DeleteExecFile(string name)
        {
            // Create path for supposed file
            var filePath = Path.Combine(ExecsFolder, name + ".json");

            // File does not exist, return false + error
            if (!File.Exists(filePath)) { return "FALSE:File does not exist"; }

            try
            {
                // Try to delete file
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                // Error occured, return false + error
                return "FALSE:" + ex.ToString();
            }

            // Succeed, return true
            return "TRUE";
        }

        /// <summary>
        /// Save exec data to file
        /// </summary>
        public static string SaveExecFile(LeagueExecutable leagueExecutable)
        {
            // Null protection
            if (leagueExecutable == null) { return null; }

            // Path for created file
            var filePath = Path.Combine(ExecsFolder, leagueExecutable.Name + ".json");

            // Double check execs folder exists, create otherwise
            if(!Directory.Exists(ExecsFolder))
            {
                Directory.CreateDirectory(ExecsFolder);
            }

            // Serialize object into json
            var serialized = JsonConvert.SerializeObject(leagueExecutable);

            try
            {
                // Write json to file
                File.WriteAllText(filePath, serialized);
            }
            catch (Exception ex)
            {
                // Return error on exception
                return "!FAIL" + ex.ToString();
            }

            // return file path
            return filePath;
        }

        /// <summary>
        /// Get name of default exec
        /// </summary>
        public static string GetDefaultExecName()
        {
            // Loop through each exec and find the default
            foreach(var name in GetSavedExecs())
            {
                var exec = GetExec(name);
                if(exec.IsDefault)
                {
                    return name;
                }
            }

            return null;
        }

        /// <summary>
        /// Set default exec
        /// </summary>
        public static void SetDefaultExecByName(string target)
        {
            // TODO: Note that this function is not very efficient. Investigate different solutions

            // Loop through each exec
            foreach (var name in GetSavedExecs())
            {
                var exec = GetExec(name);

                // If exec is default and name is different, remove default and save
                if (exec.IsDefault && name != target)
                {
                    exec.IsDefault = false;
                    SaveExecFile(exec);
                }

                // If exec is not default and name is same, make default and save
                else if (name.Equals(target) && !exec.IsDefault)
                {
                    exec.IsDefault = true;
                    SaveExecFile(exec);
                }
            }
        }

        public static string UpdateLeaguePath(string target)
        {
            // Get exec
            var exec = GetExec(target);

            // If the target path does not exist
            if(!File.Exists(exec.TargetPath))
            {
                // If target starting folder exists
                if(Directory.Exists(exec.StartFolder))
                {
                    try
                    {
                        // Find executable
                        exec.TargetPath = GameLocator.FindLeagueExecutable(exec.StartFolder);
                        SaveExecFile(exec);

                        return "TRUE";
                    }
                    catch (Exception ex)
                    {
                        // Error trying to navigate and find league exec
                        return "FALSE: Exception - " + ex.ToString();
                    }
                }
                else
                {
                    // No information to continue
                    return "FALSE: Start folder does not exist";
                }
            }

            return "FALSE: League path already exists";
        }
    }
}
