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

            SearchKeywords = $"{FileInfo.Name.ToUpper(CultureInfo.InvariantCulture)};" +
                $"{ReplayFile.AlternativeName.ToUpper(CultureInfo.InvariantCulture)};" +
                $"{string.Join("|", ReplayFile.Players.Select(x => x.NAME.ToUpper(CultureInfo.InvariantCulture)))};" +
                $"{string.Join("|", ReplayFile.Players.Select(x => x.SKIN.ToUpper(CultureInfo.InvariantCulture)))}";

            FileName = FileInfo.Name;
            AlternativeName = replayFile.AlternativeName;
            FileSizeBytes = FileInfo.FileSizeBytes;
            FileCreationTime = FileInfo.CreationTime;
            //PlayerNames = string.Join("|", ReplayFile.Players.Select(x => x.NAME.ToUpper(CultureInfo.InvariantCulture)));
            //ChampionNames = string.Join("|", ReplayFile.Players.Select(x => x.SKIN.ToUpper(CultureInfo.InvariantCulture)));
        }
        
        public ReplayFileInfo FileInfo { get; set; }

        public ReplayFile ReplayFile { get; set; }

        public string Id { get; set; }

        public bool IsNewFile { get; set; }

        // The following fields are used to allow for fast indexing
        // Placing them on the root level object makes creating indexes very easy and clear.

        /// <summary>
        /// Contains all search keywords, delimited by semicolon
        /// Only field used by search
        /// </summary>
        public string SearchKeywords { get; set; }

        // The following fields are only used for sorting
        public string FileName { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime FileCreationTime { get; set; }
        public string AlternativeName { get; set; }

        ///// <summary>
        ///// User assigned name alternative
        ///// </summary>
        //
        //public string PlayerNames { get; set; }
        //public string ChampionNames { get; set; }
    }
}
