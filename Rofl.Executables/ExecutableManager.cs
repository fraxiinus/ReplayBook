using Newtonsoft.Json;
using Rofl.Executables;
using Rofl.Executables.Utilities;
using Rofl.Logger;
using Rofl.Executables.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Rofl.Executables
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class ExecutableManager
    {
        private readonly string _exeInfoFilePath;

        private readonly Scribe _log;

        private readonly string _myName;

        public ExecutableSettings Settings { get; private set; }

        public ExecutableManager(Scribe log)
        {
            _log = log;
            _myName = this.GetType().ToString();

            // Get the exe directory or create it
            string fileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            if (!Directory.Exists(fileDir))
            {
                _log.Information(_myName, $"Executable directory does not exist, creating");
                Directory.CreateDirectory(fileDir);
            }

            // Get the exeInfoFile or create new one
            _exeInfoFilePath = Path.Combine(fileDir, "executableSettings.json");
            if (!File.Exists(_exeInfoFilePath))
            {
                // Exe file is missing, set up defaults
                _log.Information(_myName, $"Executable file does not exist, creating");
                Settings = new ExecutableSettings();
            }
            else
            {
                // Exe file found, load it
                try
                {
                    Settings = JsonConvert.DeserializeObject<ExecutableSettings>(File.ReadAllText(_exeInfoFilePath));
                }
                catch (Exception parseEx)
                {
                    // Failed loading, create new one instead
                    _log.Error(_myName, $"Error reading executable info file, creating new one. {parseEx.ToString()}");

                    Settings = new ExecutableSettings();
                }
            }
        }

        ~ExecutableManager()
        {
            Save();
        }

        public void SearchFolderForExecutables(string startPath)
        {
            if (!Directory.Exists(startPath))
            {
                _log.Warning(_myName, $"Input path {startPath} does not exist");
                throw new DirectoryNotFoundException($"Input path {startPath} does not exist");
            }

            try
            {
                var exeFiles = Directory.EnumerateFiles(startPath, "League of Legends.exe", SearchOption.AllDirectories);

                foreach (string exePath in exeFiles)
                {
                    LeagueExecutable newExe = ExeTools.CreateNewLeagueExecutable(exePath);

                    AddExecutable(newExe);
                }
            }
            catch (Exception e)
            {
                _log.Error(_myName, e.ToString());
                throw;
            }
        }

        public IEnumerable<LeagueExecutable> GetExecutables()
        {
            return Settings.Executables;
        }

        /// <summary>
        /// Returns <see cref="LeagueExecutable"/> with matching <paramref name="executableName"/>.
        /// If no matching item is found, returns null.
        /// If <paramref name="executableName"/> is null or empty then returns null.
        /// </summary>
        /// <param name="executableName"></param>
        /// <returns></returns>
        public LeagueExecutable GetExecutable(string executableName)
        {
            if (string.IsNullOrEmpty(executableName))
            {
                return null;
            }

            return Settings.Executables
                .Where(x => x.Name.Equals(executableName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public string GetDefaultExecutableName()
        {
            return Settings.DefaultExecutableName;
        }

        public LeagueExecutable GetDefaultExecutable()
        {
            return GetExecutable(Settings.DefaultExecutableName);
        }

        /// <summary>
        /// Sets default executable. Throws <see cref="ArgumentException"/> if <paramref name="executableName"/> does not exist.
        /// </summary>
        /// <param name="executableName"></param>
        public void SetDefaultExectuable(string executableName)
        {
            // Set default to nothing if null is given
            if(executableName == null)
            {
                Settings.DefaultExecutableName = null;
                return;
            }

            // Does an executable with that name exist?
            if (GetExecutable(executableName) == null)
            {
                _log.Warning(_myName, $"Executable by name {executableName} does not exist");
                throw new ArgumentException("No executable with matching name found", nameof(executableName));
            }

            var oldDefault = GetDefaultExecutable();

            if (oldDefault == null)
            {
                // Old is null, so we always set it
                _log.Information(_myName, $"Current default is null, setting to {executableName}");
                Settings.DefaultExecutableName = executableName;
            }
            else
            {
                if (oldDefault.Name.Equals(executableName, StringComparison.OrdinalIgnoreCase))
                {
                    // Default is already set
                    _log.Information(_myName, $"Default is already set to {executableName}");
                    return;
                }

                // Replace old default
                _log.Information(_myName, $"Changing default from {oldDefault.Name} to {executableName}");
                Settings.DefaultExecutableName = executableName;
            }
        }

        public void AddExecutable(LeagueExecutable newExecutable)
        {
            // Validate file
            if (newExecutable == null) 
            {
                _log.Warning(_myName, $"Given Executable is null");
                throw new ArgumentNullException(nameof(newExecutable));
            }

            // Add character to the end of the name if already exists
            string name = newExecutable.Name;
            while (!CheckExecutableName(name))
            {
                name += "+";
            }
            newExecutable.Name = name;

            // Will throw exception if executable is invalid
            ExeTools.ValidateLeagueExecutable(newExecutable);

            Settings.Executables.Add(newExecutable);

            if (Settings.Executables.Count < 1)
            {
                SetDefaultExectuable(newExecutable.Name);
            }
        }

        /// <summary>
        /// Deletes the item with name <paramref name="name"/>. Throws <see cref="ArgumentException"/> if name does not exist.
        /// If default is deleted, default is set to null.
        /// </summary>
        /// <param name="name"></param>
        public void DeleteExecutable(string name)
        {
            LeagueExecutable target = GetExecutable(name);
            if (target == null)
            {
                _log.Warning(_myName, $"Executable with name {name} does not exist");
                throw new ArgumentException($"Executable with name {name} does not exist");
            }

            // Delete the executable
            _log.Information(_myName, $"Deleting executable {target.Name}");
            Settings.Executables.Remove(target);

            // Did we delete the default?
            if (GetDefaultExecutable().Name.Equals(target.Name, StringComparison.OrdinalIgnoreCase))
            {
                // If we did we need to reset the default
                SetDefaultExectuable(null);
            }
        }

        /// <summary>
        /// Write list of executables and default to file
        /// </summary>
        public void Save()
        {
            string outputFile = JsonConvert.SerializeObject(Settings);

            File.WriteAllText(_exeInfoFilePath, outputFile);
        }

        public bool CheckExecutableName(string name)
        {
            if(GetExecutable(name) != null) { return false; }
            return true;
        }

    }
}
