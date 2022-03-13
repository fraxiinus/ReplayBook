using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;

namespace Fraxiinus.ReplayBook.Executables.Old.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class ExecutableSettings
    {
        public ExecutableSettings()
        {
            Executables = new ObservableCollection<LeagueExecutable>();
            SourceFolders = new ObservableCollection<string>();
        }

        [JsonProperty("executables")]
        public ObservableCollection<LeagueExecutable> Executables { get; private set; }

        [JsonProperty("sourceFolders")]
        public ObservableCollection<string> SourceFolders { get; private set; }

    }
}
