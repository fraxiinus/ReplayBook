using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class ExportPreset
    {
        public string PresetName { get; set; }

        public bool AlwaysIncludeMarked { get; set; }

        public bool ExportAsCSV { get; set; }

        public bool NormalizeAttributeNames { get; set; }

        public bool IncludeMatchID { get; set; }

        public bool IncludeMatchDuration { get; set; }

        public bool IncludePatchVersion { get; set; }

        public List<string> SelectedPlayers { get; set; }

        public List<string> SelectedAttributes { get; set; }
    }
}
