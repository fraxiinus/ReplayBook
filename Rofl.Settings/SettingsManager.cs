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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class SettingsManager
    {

        public ObservableSettings Settings { get; private set; }

        public ExecutableManager Executables { get; private set; }

        // private SettingsModel _rawSettings;

        private readonly RiZhi _log;

        public SettingsManager(RiZhi log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            Executables = new ExecutableManager(log);

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            if (File.Exists(configPath))
            {
                LoadConfigFile(configPath);
            }
            else
            {
                _log.Information("No config file found, creating new defaults");
                Settings = new ObservableSettings();
            }
        }

        public void SaveConfigFile()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            // Write the file result
            using (StreamWriter file = File.CreateText(configPath))
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

        public ObservableSettings LoadConfigFile(string configPath)
        {
            SettingsModel rawSettings;
            using (StreamReader file = File.OpenText(configPath))
            {
                var serializer = JsonSerializer.Create();
                rawSettings = serializer.Deserialize(file, typeof(SettingsModel)) as SettingsModel;
            }

            // Validate that nothing is null
            if (rawSettings == null) { throw new Exception("Parsed config file is null"); }

            var nullSubObjectCount = typeof(SettingsModel).GetProperties()
                .Select(prop => prop.GetValue(rawSettings))
                .Where(value => value == null)
                .Count();

            if(nullSubObjectCount > 0)
            {
                throw new Exception("Parsed config file contained null subobject");
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
    }
}
