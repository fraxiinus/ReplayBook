namespace Fraxiinus.ReplayBook.Files.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class FileResult
{
    // Blank constructor for LiteDB
    public FileResult() { }

    public FileResult(ReplayFileInfo fileInfo, ReplayFile replayFile)
    {
        FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        ReplayFile = replayFile ?? throw new ArgumentNullException(nameof(replayFile));

        Id = FileInfo.Path;

        SearchKeywords = new List<string>
            {
                FileInfo.Name.ToUpper(CultureInfo.InvariantCulture),
                ReplayFile.AlternativeName.ToUpper(CultureInfo.InvariantCulture)
            };
        SearchKeywords.AddRange(ReplayFile.Players.Select(x => x.Name.ToUpper(CultureInfo.InvariantCulture)));
        SearchKeywords.AddRange(ReplayFile.Players.Select(x => x.Skin.ToUpper(CultureInfo.InvariantCulture)));

        FileName = FileInfo.Name;
        AlternativeName = replayFile.AlternativeName;
        FileSizeBytes = FileInfo.FileSizeBytes;
        FileCreationTime = FileInfo.CreationTime;
    }

    public ReplayFileInfo FileInfo { get; set; }

    public ReplayFile ReplayFile { get; set; }

    /// <summary>
    /// Full file path used as ID
    /// </summary>
    public string Id { get; set; }

    public bool IsNewFile { get; set; }

    // The following fields are used to allow for fast indexing
    // Placing them on the root level object makes creating indexes very easy and clear.
    public List<string> SearchKeywords { get; set; }

    // The following fields are only used for sorting
    public string FileName { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime FileCreationTime { get; set; }
    public string AlternativeName { get; set; }
}
