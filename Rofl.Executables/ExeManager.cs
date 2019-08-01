using Newtonsoft.Json;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Requests.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rofl.Executables
{
    public class ExeManager
    {
        private string _exeInfoFilePath;

        /// <summary>
        /// Contains all executables except for the default
        /// </summary>
        private readonly List<LeagueExecutable> _executables;

        private LeagueExecutable _defaultExecutable;

        public ExeTools ExeTools { get; private set; }

        private readonly string _exceptionOriginName = "Rofl.Executables.ExeManager";

        public ExeManager()
        {
            string fileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            _exeInfoFilePath = Path.Combine(fileDir, "executables.json");
            ExeTools = new ExeTools();

            if (!File.Exists(_exeInfoFilePath))
            {
                _executables = new List<LeagueExecutable>();
                // Exe file is missing, create it
                _defaultExecutable = SetupFirstExe();
            }
            else
            {
                InfoFile savedExeObject = JsonConvert.DeserializeObject<InfoFile>(File.ReadAllText(_exeInfoFilePath));

                _executables = savedExeObject.Executables;

                _defaultExecutable = savedExeObject.DefaultExecutable;
            }
        }

        // This is a backup constructor that should only be called when the original fails when first launched
        public ExeManager(LeagueExecutable manualDefault)
        {
            string fileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            _exeInfoFilePath = Path.Combine(fileDir, "executables.json");
            ExeTools = new ExeTools();
            _executables = new List<LeagueExecutable>();
            _defaultExecutable = manualDefault;
        }

        public void Save()
        {
            string outputFile = JsonConvert.SerializeObject(new InfoFile()
            {
                Executables = _executables,
                DefaultExecutable = _defaultExecutable
            });

            File.WriteAllText(_exeInfoFilePath, outputFile);
        }

        /// <summary>
        /// Given executable name, return corresponding LeagueExecutable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LeagueExecutable GetExecutable(string name)
        {
            if(String.IsNullOrEmpty(name)) { return null; }

            if(_defaultExecutable.Name.ToUpper().Equals(name.ToUpper())) {
                return _defaultExecutable;
            }

            return (from exe in _executables
                    where exe.Name.ToUpper().Equals(name.ToUpper())
                    select exe).FirstOrDefault();
        }

        public LeagueExecutable[] GetExecutables()
        {
            return _executables.ToArray();
        }

        public LeagueExecutable GetDefaultExecutable()
        {
            return _defaultExecutable;
        }

        /// <summary>
        /// Given executable name, delete LeagueExecutable
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void DeleteExecutable(string name)
        {
            LeagueExecutable target = GetExecutable(name);

            if(target == null)
            {
                if(_defaultExecutable.Name.ToUpper().Equals(name.ToUpper()))
                {
                    // Deleting the default exe, set the first in the list as the default
                    LeagueExecutable oldDefault = _defaultExecutable;

                    SetDefaultExectuable(_executables.First().Name);

                    _executables.Remove(oldDefault);
                }
                else
                {
                    throw new KeyNotFoundException($"{_exceptionOriginName} - No executable \"{name}\" found");
                }
            }

            _executables.Remove(target);
        }

        /// <summary>
        /// Adds newExe to exe list. Assigns default if new exe has isdefault set to true
        /// </summary>
        /// <param name="newExe"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddExecutable(LeagueExecutable newExe)
        {
            // Validate file
            if(newExe == null) { throw new ArgumentNullException($"{_exceptionOriginName} - Cannot save null executable"); }

            ValidateExecutable(newExe);

            if(newExe.IsDefault)
            {
                // Swap default exes
                _defaultExecutable.IsDefault = false;
                _executables.Add(_defaultExecutable);

                _defaultExecutable = newExe;
            }
            else
            {
                _executables.Add(newExe);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void SetDefaultExectuable(string name)
        {
            LeagueExecutable newDefault = GetExecutable(name);
            LeagueExecutable oldDefault = GetDefaultExecutable();

            if(oldDefault.Name.ToUpper().Equals(name.ToUpper()))
            {
                return;
            }

            if (newDefault == null) { throw new KeyNotFoundException($"{_exceptionOriginName} - No executable \"{name}\" found"); }

            newDefault.IsDefault = true;
            _defaultExecutable = newDefault;
            _executables.Remove(newDefault);

            oldDefault.IsDefault = false;
            _executables.Add(oldDefault);
        }

        public void ReplaceDefaultExecutable(LeagueExecutable exe)
        {
            _defaultExecutable = exe;
        }

        public void UpdateExecutableTarget(string name)
        {
            LeagueExecutable targetExe = GetExecutable(name);

            if(targetExe == null) { throw new KeyNotFoundException($"{_exceptionOriginName} - No executable \"{name}\" found"); }

            if(!Directory.Exists(targetExe.StartFolder))
            {
                throw new DirectoryNotFoundException($"{_exceptionOriginName} - Start directory does not exist");
            }

            targetExe.TargetPath = ExeTools.FindLeagueExecutablePath(targetExe.StartFolder);
        }

        private LeagueExecutable SetupFirstExe()
        {
            LeagueExecutable returnExe = new LeagueExecutable();

            returnExe.StartFolder = ExeTools.GetLeagueInstallPathFromRegistry();

            returnExe.TargetPath = ExeTools.FindLeagueExecutablePath(returnExe.StartFolder);

            returnExe.ModifiedDate = ExeTools.GetLastModifiedDate(returnExe.TargetPath);
            returnExe.PatchVersion = ExeTools.GetLeagueVersion(returnExe.TargetPath);
            returnExe.AllowUpdates = true;
            returnExe.UseOldLaunchArguments = false;
            returnExe.IsDefault = true;
            returnExe.Name = "Default";

            return returnExe;
        }

        /// <summary>
        /// Throws an exception if one of the following assertions fail
        /// </summary>
        /// <param name="exe"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public void ValidateExecutable(LeagueExecutable exe, bool requireUniqueName = true)
        {
            // Name must not already exist
            // Start folder must exist
            // Target file must exist and be contained in start folder
            // patch version must pass versionsubstring
            // allowupdates...
            // isdefault...
            // ModifiedDate must not be null

            // Check all properties if they are null
            if (String.IsNullOrEmpty(exe.Name) ||
                String.IsNullOrEmpty(exe.TargetPath) ||
                String.IsNullOrEmpty(exe.StartFolder) ||
                String.IsNullOrEmpty(exe.PatchVersion) ||
                exe.ModifiedDate == null)
            {
                throw new ArgumentNullException($"{_exceptionOriginName} - Property set to null");
            }

            // Check if executable by the same name already exists
            LeagueExecutable matchingExe = (from e in _executables
                                            where e.Name.ToUpper().Equals(exe.Name.ToUpper())
                                            select e).FirstOrDefault();

            bool defaultMatches = _defaultExecutable.Name.ToUpper().Equals(exe.Name);

            if(requireUniqueName)
            {
                if (matchingExe != null || defaultMatches) { throw new ArgumentException($"{_exceptionOriginName} - Executable by \"{exe.Name}\" already exists"); }
            }

            // Check if start folder exists
            if (!Directory.Exists(exe.StartFolder)) { throw new DirectoryNotFoundException($"{_exceptionOriginName} - Start folder \"{exe.StartFolder}\" does not exist"); }

            // Check if target path begins with stater path
            if (!exe.TargetPath.StartsWith(exe.StartFolder)) { throw new FileNotFoundException($"{_exceptionOriginName} - Target file \"{exe.TargetPath}\" cannot be found from start folder \"{exe.StartFolder}\""); }

            // Check if target path exists
            if (!File.Exists(exe.TargetPath)) { throw new FileNotFoundException($"{_exceptionOriginName} - Target file \"{exe.TargetPath}\" not found"); }

            // Check if patch version is properly formatted
            if (exe.PatchVersion.VersionSubstring() == null) { throw new ArgumentException($"{_exceptionOriginName} - Version string \"{exe.PatchVersion}\" not proper format"); }

        }
    }
}
