namespace Fraxiinus.ReplayBook.Files.Repositories;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Models.Search;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class DatabaseRepository
{
    private readonly RiZhi _log;
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "replayCache.db");
    private readonly ObservableConfiguration _config;

    public DatabaseRepository(ObservableConfiguration config, RiZhi log)
    {
        _config = config;
        _log = log;

        try
        {
            InitializeDatabase();
        }
        catch (Exception ex)
        {
            _log.Warning($"Database file is invalid, deleting and trying again... exception:{ex}");
            File.Delete(_filePath);
            InitializeDatabase();
        }
    }

    public string GetDatabasePath()
    {
        return _filePath;
    }

    public void DeleteDatabase()
    {
        File.Delete(_filePath);
    }

    private void InitializeDatabase()
    {
        if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
        }

        using var db = new LiteDatabase(_filePath);
        // Prevent empty string values from being lost!
        BsonMapper.Global.EmptyStringToNull = false;

        // Create and verify file results collection
        ILiteCollection<FileResult> fileResults = db.GetCollection<FileResult>("fileResults");

        _ = BsonMapper.Global.Entity<FileResult>()
            .Id(r => r.Id)
            .DbRef(r => r.FileInfo, "replayFileInfo")
            .DbRef(r => r.ReplayFile, "replayFiles")
            .DbRef(r => r.ErrorInfo, "replayErrorInfo");

        _ = BsonMapper.Global.Entity<ReplayFileInfo>()
            .Id(r => r.Path);

        _ = BsonMapper.Global.Entity<ReplayErrorInfo>()
            .Id(r => r.FilePath);

        _ = BsonMapper.Global.Entity<PlayerStats2>()
            .Id(r => r.UniqueId);

        _ = BsonMapper.Global.Entity<ReplayFile>()
            .Id(r => r.Location)
            .DbRef(r => r.RedPlayers, "players")
            .DbRef(r => r.BluePlayers, "players");

        _ = fileResults.EnsureIndex(x => x.FileName);
        _ = fileResults.EnsureIndex(x => x.AlternativeName);
        _ = fileResults.EnsureIndex(x => x.FileSizeBytes);
        _ = fileResults.EnsureIndex(x => x.FileCreationTime);
        _ = fileResults.EnsureIndex(x => x.SearchKeywords);
    }

    public void AddFileResult(FileResult result)
    {
        if (result == null) { throw new ArgumentNullException(nameof(result)); }
        //if (result.ReplayFile == null) { throw new ArgumentNullException(nameof(result)); }
        if (result.FileInfo == null) { throw new ArgumentNullException(nameof(result)); }

        using var db = new LiteDatabase(_filePath);

        var fileResults = db.GetCollection<FileResult>("fileResults");
        var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
        var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
        var replayErrors = db.GetCollection<ReplayErrorInfo>("replayErrorInfo");
        var players = db.GetCollection<PlayerStats2>("players");

        // If we already have the file, do nothing
        if (fileResults.FindById(result.Id) == null)
        {
            fileResults.Insert(result);
        }

        // Only add if it doesnt exist, and fileInfo exists
        if (fileInfos.FindById(result.FileInfo.Path) == null && result.FileInfo != null)
        {
            fileInfos.Insert(result.FileInfo);
        }

        if (result.ReplayFile != null && replayFiles.FindById(result.ReplayFile.Location) == null)
        {
            replayFiles.Insert(result.ReplayFile);
        }

        if (result.ErrorInfo != null && replayErrors.FindById(result.ErrorInfo.FilePath) == null)
        {
            replayErrors.Insert(result.ErrorInfo);
        }

        if (result.ReplayFile != default)
        {
            foreach (var player in result.ReplayFile?.Players)
            {
                // If the player already exists, do nothing
                if (players.FindById(player.UniqueId) == null)
                {
                    players.Insert(player);
                }
            }
        }
    }

    public void RemoveFileResult(string id)
    {
        if (string.IsNullOrEmpty(id)) { throw new ArgumentNullException(id); }

        using var db = new LiteDatabase(_filePath);

        var fileResults = db.GetCollection<FileResult>("fileResults");
        var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
        var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
        var replayErrors = db.GetCollection<ReplayErrorInfo>("replayErrorInfo");
        var players = db.GetCollection<PlayerStats2>("players");

        fileResults.Delete(id);

        fileInfos.Delete(id);

        replayFiles.Delete(id);

        replayErrors.Delete(id);

        // Rip player data is being orphaned...lol
    }

    public FileResult GetFileResult(string id)
    {
        if (string.IsNullOrEmpty(id)) { throw new ArgumentNullException(id); }

        using var db = new LiteDatabase(_filePath);

        return db.GetCollection<FileResult>("fileResults")
            .Include("$.FileInfo")
            .Include("$.ReplayFile")
            .Include("$.ErrorInfo")
            .Include("$.ReplayFile.Players[*]")
            .Include("$.ReplayFile.BluePlayers[*]")
            .Include("$.ReplayFile.RedPlayers[*]")
            .FindById(id);
    }

    public IReadOnlyCollection<FileResult> GetFileResults(IEnumerable<string> ids)
    {
        return ids.Select(x => GetFileResult(x)).ToList();
    }

    public IEnumerable<FileResult> GetReplayFiles()
    {
        using var db = new LiteDatabase(_filePath);
        var fileResults = db.GetCollection<FileResult>("fileResults")
            .Include("$.FileInfo")
            .Include("$.ReplayFile")
            .Include("$.ErrorInfo")
            .Include("$.ReplayFile.Players[*]")
            .Include("$.ReplayFile.BluePlayers[*]")
            .Include("$.ReplayFile.RedPlayers[*]");

        return fileResults.FindAll().ToList();
    }

    public IReadOnlyCollection<FileResult> QueryReplayFiles(string[] keywords, SortMethod sort, int maxEntries, int skip)
    {
        if (keywords == null) { throw new ArgumentNullException(nameof(keywords)); }

        using var db = new LiteDatabase(_filePath);

        // include all references
        var fileResultsQueryable = db.GetCollection<FileResult>("fileResults")
            .Include("$.FileInfo")
            .Include("$.ReplayFile")
            .Include("$.ErrorInfo")
            .Include("$.ReplayFile.Players[*]")
            .Include("$.ReplayFile.BluePlayers[*]")
            .Include("$.ReplayFile.RedPlayers[*]")
            .Query();

        // apply sort method to query
        fileResultsQueryable = sort switch
        {
            // sort by name depends on if we are using file names, or alternative names
            SortMethod.NameAsc => fileResultsQueryable.OrderBy(x => _config.RenameFile ? x.FileName : x.AlternativeName),
            SortMethod.NameDesc => fileResultsQueryable.OrderByDescending(x => _config.RenameFile ? x.FileName : x.AlternativeName),
            SortMethod.DateAsc => fileResultsQueryable.OrderBy(x => x.FileCreationTime),
            SortMethod.DateDesc => fileResultsQueryable.OrderByDescending(x => x.FileCreationTime),
            SortMethod.SizeAsc => fileResultsQueryable.OrderBy(x => x.FileSizeBytes),
            SortMethod.SizeDesc => fileResultsQueryable.OrderByDescending(x => x.FileSizeBytes),
            _ => fileResultsQueryable.OrderBy(x => x.FileCreationTime)
        };

        // apply filter based on keywords
        if (keywords.Length > 0)
        {
            // this filters on ANY, we want to filter on ALL but not sure how to accomplish that (LiteDB issue #1885)
            fileResultsQueryable = fileResultsQueryable.Where("$.SearchKeywords[*] ANY IN @0", BsonMapper.Global.Serialize(keywords));
        }

        // apply skip amount and limit results
        return fileResultsQueryable.Offset(skip).Limit(maxEntries).ToList();
    }

    public void UpdateAlternativeName(string id, string newName)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(id);

        using var db = new LiteDatabase(_filePath);

        var fileResults = db.GetCollection<FileResult>("fileResults")
            .Include("$.FileInfo")
            .Include("$.ReplayFile")
            .Include("$.ErrorInfo")
            .Include("$.ReplayFile.Players[*]")
            .Include("$.ReplayFile.BluePlayers[*]")
            .Include("$.ReplayFile.RedPlayers[*]");

        var result = fileResults.FindById(id);

        if (result == null)
        {
            throw new KeyNotFoundException($"Could not find FileResult by id {id}");
        }
        else
        {
            _log.Information($"Db updating {result.AlternativeName} to {newName}");

            // Update the file results (for indexing/search)
            result.AlternativeName = newName;
            result.SearchKeywords = new List<string>
            {
                result.FileInfo.Name.ToUpper(CultureInfo.InvariantCulture),
                result.AlternativeName.ToUpper(CultureInfo.InvariantCulture)
            };
            fileResults.Update(result);

            // Update the replay entry
            //var replays = db.GetCollection<ReplayFile>("replayFiles");
            //var replayEntry = replays.FindById(id);
            //replayEntry.AlternativeName = newName;
            //replays.Update(replayEntry);
        }
    }
}