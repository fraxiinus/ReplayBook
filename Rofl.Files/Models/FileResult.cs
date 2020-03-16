using Rofl.Reader.Models;
using System;
using System.Globalization;
using System.Linq;

namespace Rofl.Files.Models
{
    public class FileResult
    {
        // Blank constructor for LiteDB
        public FileResult() { }

        public FileResult(ReplayFileInfo fileInfo, ReplayFile replayFile)
        {
            FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            ReplayFile = replayFile ?? throw new ArgumentNullException(nameof(replayFile));

            Id = FileInfo.Path;
            PlayerNames = String.Join("|", ReplayFile.Players.Select(x => x.NAME.ToUpper(CultureInfo.InvariantCulture)));
            ChampionNames = String.Join("|", ReplayFile.Players.Select(x => x.SKIN.ToUpper(CultureInfo.InvariantCulture)));
        }
        
        public ReplayFileInfo FileInfo { get; set; }

        public ReplayFile ReplayFile { get; set; }

        public string Id { get; set; }

        public bool IsNewFile { get; set; }

        public string PlayerNames { get; set; }

        public string ChampionNames { get; set; }
    }
}
