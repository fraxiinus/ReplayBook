using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.Executables.Old.Models;

public class ExecutableSettings
{

    [JsonPropertyName("executables")]
    public ObservableCollection<LeagueExecutable> Executables { get; set; }

    [JsonPropertyName("sourceFolders")]
    public ObservableCollection<string> SourceFolders { get; set; }

}
