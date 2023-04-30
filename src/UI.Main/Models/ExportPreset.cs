using System.Collections.Generic;

namespace Fraxiinus.ReplayBook.UI.Main.Models
{
    public class ExportPreset
    {

        public string PresetName { get; set; }

        public bool ManualPlayerSelection { get; set; }

        public bool AlwaysIncludeMarked { get; set; }

        public bool IncludeAllPlayers { get; set; }

        public bool ExportAsCSV { get; set; }

        public bool NormalizeAttributeNames { get; set; }

        public bool ApplyStaticData { get; set; }

        public bool IncludeMatchID { get; set; }

        public bool IncludeMatchDuration { get; set; }

        public bool IncludePatchVersion { get; set; }

        public List<string> SelectedPlayers { get; set; }

        public List<string> SelectedAttributes { get; set; }
    }
}
