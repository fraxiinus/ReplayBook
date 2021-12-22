using Etirps.RiZhi;
using Newtonsoft.Json;
using Rofl.Executables;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rofl.Settings
{
    public class SettingsManager
    {

        public ObservableSettings Settings { get; private set; }

        public Dictionary<string, object> TemporaryValues { get; private set; }

        public ExecutableManager Executables { get; private set; }

        // private SettingsModel _rawSettings;

        private readonly RiZhi _log;
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        private readonly string _tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "stash.json");

        public SettingsManager(RiZhi log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            Executables = new ExecutableManager(log);

            if (File.Exists(_configPath))
            {
                LoadConfigFile();
            }
            else
            {
                _log.Information("No config file found, creating new defaults");
                Settings = new ObservableSettings();
            }

            LoadTemporaryValues();
        }

        public void SaveConfigFile()
        {
            // Write the file result
            using (StreamWriter file = File.CreateText(_configPath))
            {
                var serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, new SettingsModel(Settings));
            }

            // Save Executables
            Executables.Save();
        }

        public string[] RemoveInvalidReplayPaths()
        {
            var missingList = new List<string>();
            foreach (var path in Settings.SourceFolders)
            {
                if (!Directory.Exists(path))
                {
                    _log.Warning($"Replay folder {path} no longer exists, deleting...");
                    missingList.Add(path);
                }
            }

            foreach (var path in missingList)
            {
                Settings.SourceFolders.Remove(path);
            }

            return missingList.ToArray();
        }

        public ObservableSettings LoadConfigFile()
        {
            SettingsModel rawSettings;
            using (StreamReader file = File.OpenText(_configPath))
            {
                var serializer = JsonSerializer.Create();
                rawSettings = serializer.Deserialize(file, typeof(SettingsModel)) as SettingsModel;
            }

            // Validate that nothing is null
            if (rawSettings == null) { throw new Exception("Parsed config file is null"); }

            var nullSubObjects = typeof(SettingsModel).GetProperties()
                .Where(prop => prop.GetValue(rawSettings) == null)
                .Select(prop => prop.Name);

            if(nullSubObjects.Any())
            {
                throw new Exception($"Parsed config file contained null subobject(s): {string.Join(",", nullSubObjects)}");
            }

            var nullGeneralSettingsCount = typeof(GeneralSettings).GetProperties()
                .Select(prop => prop.GetValue(rawSettings.GeneralSettings))
                .Where(value => value == null)
                .Count();

            if (nullGeneralSettingsCount > 0)
            {
                throw new Exception("Parsed config file general settings contains null");
            }

            var nullReplaySettingsCount = typeof(ReplaySettings).GetProperties()
                .Select(prop => prop.GetValue(rawSettings.ReplaySettings))
                .Where(value => value == null)
                .Count();

            if (nullGeneralSettingsCount > 0)
            {
                throw new Exception("Parsed config file replay settings contains null");
            }

            var nullRequestSettingsCount = typeof(RequestSettings).GetProperties()
                .Select(prop => prop.GetValue(rawSettings.RequestSettings))
                .Where(value => value == null)
                .Count();

            if (nullRequestSettingsCount > 0)
            {
                throw new Exception("Parsed config file request settings contains null");
            }

            Settings = new ObservableSettings(rawSettings);
            return Settings;
        }

        public void LoadTemporaryValues()
        {
            if (!File.Exists(_tempPath))
            {
                TemporaryValues = new Dictionary<string, object>();
                return;
            }

            Dictionary<string, object> values;
            using (StreamReader file = File.OpenText(_tempPath))
            {
                var serializer = JsonSerializer.Create();
                values = serializer.Deserialize(file, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            }

            TemporaryValues = values;
        }

        public void SaveTemporaryValues()
        {
            // Write the file result
            using (StreamWriter file = File.CreateText(_tempPath))
            {
                var serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, TemporaryValues);
            }
        }
    }
}
