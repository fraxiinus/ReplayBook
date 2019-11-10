using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rofl.Logger;
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

        private SettingsModel _rawSettings;

        private readonly Scribe _log;

        private readonly string _myName;

        public SettingsManager(Scribe log)
        {
            _log = log;
            _myName = this.GetType().ToString();
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            if (File.Exists(configPath))
            {
                LoadConfigFile(configPath);
            }
            else
            {
                _log.Information(_myName, "No config file found, creating new defaults");
                CreateDefaultConfigFile();
            }
        }

        public void SaveConfigFile()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

            // Write the file result
            using (StreamWriter file = File.CreateText(configPath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Settings);
            }
        }

        public ObservableSettings CreateDefaultConfigFile()
        {
            _rawSettings = new SettingsModel
            {
                GeneralSettings = new GeneralSettings(),
                ReplaySettings = new ReplaySettings(),
                RequestSettings = new RequestSettings
                {
                    DataDragonBaseUrl = @"http://ddragon.leagueoflegends.com/cdn/",
                    ChampionRelativeUrl = @"/img/champion/",
                    MapRelativeUrl = @"/img/map/map",
                    ItemRelativeUrl = @"/img/item/"
                }
            };
            Settings = new ObservableSettings(_rawSettings);

            return Settings;
        }

        public ObservableSettings LoadConfigFile(string configPath)
        {
            using (StreamReader file = File.OpenText(configPath))
            {
                var serializer = JsonSerializer.Create();
                _rawSettings = serializer.Deserialize(file, typeof(SettingsModel)) as SettingsModel;
            }

            // Validate that nothing is null
            if (_rawSettings == null) { throw new Exception("Parsed config file is null"); }

            var nullSubObjectCount = typeof(SettingsModel).GetProperties()
                .Select(prop => prop.GetValue(_rawSettings))
                .Where(value => value == null)
                .Count();

            if(nullSubObjectCount > 0)
            {
                throw new Exception("Parsed config file contained null subobject");
            }

            var nullGeneralSettingsCount = typeof(GeneralSettings).GetProperties()
                .Select(prop => prop.GetValue(_rawSettings.GeneralSettings))
                .Where(value => value == null)
                .Count();

            if (nullGeneralSettingsCount > 0)
            {
                throw new Exception("Parsed config file general settings contains null");
            }

            var nullReplaySettingsCount = typeof(ReplaySettings).GetProperties()
                .Select(prop => prop.GetValue(_rawSettings.ReplaySettings))
                .Where(value => value == null)
                .Count();

            if (nullGeneralSettingsCount > 0)
            {
                throw new Exception("Parsed config file replay settings contains null");
            }

            var nullRequestSettingsCount = typeof(RequestSettings).GetProperties()
                .Select(prop => prop.GetValue(_rawSettings.RequestSettings))
                .Where(value => value == null)
                .Count();

            if (nullRequestSettingsCount > 0)
            {
                throw new Exception("Parsed config file request settings contains null");
            }

            Settings = new ObservableSettings(_rawSettings);
            return Settings;
        }
    }
}
