using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Parsers.Models
{
    /// <summary>
    /// ROFL (.rofl) files are replays from Riot's official replay system
    /// BARON (.lpr) files are replays from the unofficial Baron Replays
    /// LOLR (.lrf) files are replays from the unofficial LoLReplay
    /// </summary>
    public enum REPLAYTYPES { ROFL, BARON, LOLR }

    public class ReplayFile
    {
        public REPLAYTYPES Type { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ReplayHeader Data { get; set; }
    }
}
