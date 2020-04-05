using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rofl.Files.Models;
using Rofl.Reader.Models;
using Rofl.Settings.Models;

namespace Rofl.UI.Main.Models
{
    public class ExportContext
    {
        public IEnumerable<ReplayFile> Replays { get; set; }

        public IEnumerable<PlayerMarker> Markers { get; set; }
    }
}
