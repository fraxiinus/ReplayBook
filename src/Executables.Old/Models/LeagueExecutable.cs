using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using System;
using System.ComponentModel;

namespace Fraxiinus.ReplayBook.Executables.Old.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LeagueExecutable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        [JsonProperty("name")]
        public string Name 
        { 
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Name)));
            } 
        }

        private string _targetPath;
        [JsonProperty("targetPath")]
        public string TargetPath
        {
            get { return _targetPath; }
            set
            {
                _targetPath = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(TargetPath)));
            }
        }

        private string _startFolder;
        [JsonProperty("startFolder")]
        public string StartFolder
        {
            get { return _startFolder; }
            set
            {
                _startFolder = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(StartFolder)));
            }
        }

        private string _patchNumber;
        [JsonProperty("patchNumber")]
        public string PatchNumber
        {
            get { return _patchNumber; }
            set
            {
                _patchNumber = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(PatchNumber)));
            }
        }

        private string _launchArgs;
        [JsonProperty("launchArgs")]
        public string LaunchArguments
        {
            get { return _launchArgs; }
            set
            {
                _launchArgs = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(LaunchArguments)));
            }
        }

        private LeagueLocale _locale;
        [JsonProperty("locale")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LeagueLocale Locale
        {
            get { return _locale; }
            set
            {
                _locale = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Locale)));
            }
        }

        private string _customLocale;
        [JsonProperty("customLocale")]
        public string CustomLocale 
        { 
            get { return _customLocale; }
            set
            {
                _customLocale = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(CustomLocale)));
            }
        }

        private DateTime _modifiedDate;
        [JsonProperty("modifiedDate")]
        public DateTime ModifiedDate
        {
            get { return _modifiedDate; }
            set
            {
                _modifiedDate = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ModifiedDate)));
            }
        }
    }
}
