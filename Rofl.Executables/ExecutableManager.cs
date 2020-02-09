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

        private readonly Scribe _log;

        private readonly string _myName;

        private string _exeInfoFilePath;

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
            _exeInfoFilePath = Path.Combine(fileDir, "executablesettings.json");
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

            // Refresh all known executables versions
            foreach (var executable in Settings.Executables)
            {
                UpdateExecutableVersion(executable);
            }
        }

        ~ExecutableManager()
        {
            Save();
        }

        public int SearchAllFoldersForExecutablesAndAddThemAll()
        {
            int counter = 0;
            foreach (var path in Settings.SourceFolders)
            {
                var foundExes = SearchFolderForExecutables(path);
                foreach (var exe in foundExes)
                {
                    if (GetExecutableByTarget(exe.TargetPath) == null)
                    {
                        AddExecutable(exe);
                        counter++;
                    }
                }
            }
            return counter;
        }

        public IList<LeagueExecutable> SearchFolderForExecutables(string startPath)
        {
            List<LeagueExecutable> foundExecutables = new List<LeagueExecutable>();

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

                    // Do we already have an exe with the same target?
                    if (!foundExecutables.Exists(x => x.TargetPath.Equals(newExe.TargetPath, StringComparison.OrdinalIgnoreCase)))
                    {
                        foundExecutables.Add(newExe);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(_myName, e.ToString());
                throw;
            }

            return foundExecutables;
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

        public LeagueExecutable GetExecutableByTarget(string target)
        {
            if (string.IsNullOrEmpty(target)) { return null; }

            return Settings.Executables
                .Where(x => x.TargetPath.Equals(target, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public IReadOnlyCollection<LeagueExecutable> GetExecutablesByPatch(string patchNumber)
        {
            var matching = Settings.Executables
                .Where(x => x.PatchNumber.VersionSubstring()
                    .Equals(patchNumber.VersionSubstring(), StringComparison.OrdinalIgnoreCase))
                .ToArray();

            return matching;
        }

        public bool DoesVersionExist(string patchNumber)
        {
            return GetExecutablesByPatch(patchNumber).Any();
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
        }

        /// <summary>
        /// Write list of executables and default to file
        /// </summary>
        public void Save()
        {
            string outputFile = JsonConvert.SerializeObject(Settings, Formatting.Indented);

            File.WriteAllText(_exeInfoFilePath, outputFile);
        }

        public bool CheckExecutableName(string name)
        {
            if(GetExecutable(name) != null) { return false; }
            return true;
        }

        public void UpdateExecutableVersion(LeagueExecutable executable)
        {
            if(executable == null) { throw new ArgumentNullException(nameof(executable)); }

            var currentVersion = ExeTools.GetLeagueVersion(executable.TargetPath);

            if (!executable.PatchNumber.Equals(currentVersion, StringComparison.OrdinalIgnoreCase))
            {
                _log.Information(_myName, $"Updating executable {executable.Name} from {executable.PatchNumber} -> {currentVersion}");
                executable.PatchNumber = currentVersion;
            }
        }
    }
}
