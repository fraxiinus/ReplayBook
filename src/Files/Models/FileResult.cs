using Fraxiinus.ReplayBook.Files.Utilities;
using Fraxiinus.Rofl.Extract.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Fraxiinus.ReplayBook.Files.Models
{
    public class FileResult
    {
        // Blank constructor for LiteDB
        public FileResult() { }

        public FileResult(ReplayFileInfo fileInfo, ROFL replayFile)
        {
            FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            ReplayFile = replayFile ?? throw new ArgumentNullException(nameof(replayFile));

            Id = FileInfo.Path;
            MapBestGuess = replayFile.GuessMap();
            BlueTeamVictory = replayFile.DidBlueTeamWin();

            SearchKeywords = new List<string>
            {
                FileInfo.Name.ToUpper(CultureInfo.InvariantCulture)
            };
            SearchKeywords.AddRange(ReplayFile.Metadata.PlayerStatistics.Select(x => x.Name.ToUpper(CultureInfo.InvariantCulture)));
            SearchKeywords.AddRange(ReplayFile.Metadata.PlayerStatistics.Select(x => x.Skin.ToUpper(CultureInfo.InvariantCulture)));

            FileName = FileInfo.Name;
            AlternativeName = FileInfo.Name; // set to initial file name
            FileSizeBytes = FileInfo.FileSizeBytes;
            FileCreationTime = FileInfo.CreationTime;
        }
        
        public ReplayFileInfo FileInfo { get; set; }

        public ROFL ReplayFile { get; set; }

        public string Id { get; set; }

        public bool IsNewFile { get; set; }

        public MapCode MapBestGuess { get; set; }

        public bool BlueTeamVictory { get; set; }

        // The following fields are used to allow for fast indexing
        // Placing them on the root level object makes creating indexes very easy and clear.
        public List<string> SearchKeywords { get; set; }

        // The following fields are only used for sorting
        public string FileName { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime FileCreationTime { get; set; }
        public string AlternativeName { get; set; }
    }
}
