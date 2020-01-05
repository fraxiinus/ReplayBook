using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using Rofl.Logger;
using System.Globalization;

namespace Rofl.Executables.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class ExecutableSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ExecutableSettings()
        {
            Executables = new ObservableCollection<LeagueExecutable>();
            SourceFolders = new ObservableCollection<string>();
        }

        private string _defaultExecutableName;
        [JsonProperty("defaultExecutable")]
        public string DefaultExecutableName 
        {
            get { return _defaultExecutableName; }
            set 
            {
                _defaultExecutableName = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(DefaultExecutableName)));
            }
        }

        [JsonProperty("executables")]
        public ObservableCollection<LeagueExecutable> Executables { get; private set; }

        [JsonProperty("sourceFolders")]
        public ObservableCollection<string> SourceFolders { get; private set; }
    }
}
