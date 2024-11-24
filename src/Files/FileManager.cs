namespace Fraxiinus.ReplayBook.Files;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Models.Search;
using Fraxiinus.ReplayBook.Files.Repositories;
using Fraxiinus.Rofl.Extract.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class FileManager
{
    private readonly FolderRepository _fileSystem;
    private readonly DatabaseRepository _db;
    private readonly SearchRepository _search;

    private readonly RiZhi _log;
    private readonly List<string> _deletedFiles;
    private readonly ObservableConfiguration _config;
    private readonly ReplayReaderOptions _readerOptions;

    public FileManager(ObservableConfiguration config, RiZhi log)
    {
        _fileSystem = new FolderRepository(config, log);
        _db = new DatabaseRepository(config, log);
        _search = new SearchRepository(config, log);

        _log = log ?? throw new ArgumentNullException(nameof(log));
        _config = config;
        _deletedFiles = new List<string>();
        _readerOptions = new ReplayReaderOptions
        {
            LoadPayload = false,
            Type = ReplayType.Unknown
        };
    }

    public string DatabasePath { get => _db.GetDatabasePath(); }

    public string SearchIndexPath { get => _search.SearchIndexDirectory; }

    public void DeleteDatabase() => _db.DeleteDatabase();

    public void DeleteSearchIndex() => _search.DeleteIndex();

    /// <summary>
    /// This function is responsible for finding and loading in new replays
    /// </summary>
    public async Task<IEnumerable<ReplayErrorInfo>> InitialLoadAsync()
    {
        _log.Information("Starting initial load of replays");

        //List<ReplayFileInfo> newFiles = new List<ReplayFileInfo>();

        var errorResults = new List<ReplayErrorInfo>();

        // Get all files from all defined replay folders
        IReadOnlyCollection<ReplayFileInfo> allFiles = _fileSystem.GetAllReplayFileInfo();

        // Check if file exists in the database
        foreach (ReplayFileInfo file in allFiles)
        {
            if (_db.GetFileResult(file.Path) == null)
            {
                try
                {
                    var parseResult = await ReplayReader.ReadReplayAsync(file.Path, _readerOptions);
                    var replayFile = new ReplayFile(file.Path, parseResult);
                    var newResult = new FileResult(file, replayFile)
                    {
                        IsNewFile = false
                    };

                    _db.AddFileResult(newResult);
                    _search.AddDocument(newResult);
                }
                catch (Exception ex)
                {
                    // if parsing file failed for any reason, save info
                    _log.Warning($"Failed to parse file: {file.Path}");
                    _log.Warning(ex.ToString());
                    var errorInfo = new ReplayErrorInfo
                    {
                        FilePath = file.Path,
                        ExceptionType = ex.GetType().FullName,
                        ExceptionString = ex.ToString(),
                        ExceptionCallStack = ex.StackTrace
                    };
                    var errorFileResult = new FileResult(file, errorInfo);
                    _db.AddFileResult(errorFileResult);
                    _search.AddDocument(errorFileResult);

                    errorResults.Add(errorInfo);
                }
            }
        }

        _search.CommitIndex();
        _log.Information("Initial load of replays complete");
        return errorResults;
    }

    public async Task<FileResult> GetSingleFile(string path)
    {
        if (!File.Exists(path)) return null;

        FileResult returnValue = _db.GetFileResult(path);

        // File exists in the database, return now
        if (returnValue != null)
        {
            _log.Information($"File {path} already exists in database. Match ID: {returnValue.ReplayFile.MatchId}");
            return returnValue;
        }

        var replayFileInfo = _fileSystem.GetSingleReplayFileInfo(path);
        var parseResult = await ReplayReader.ReadReplayAsync(path, _readerOptions);

        if (parseResult is null) return null;

        var replayFile = new ReplayFile(path, parseResult);
        var newResult = new FileResult(replayFileInfo, replayFile)
        {
            IsNewFile = false
        };

        _db.AddFileResult(newResult);

        return newResult;
    }

    /// <summary>
    /// Checks all entries and deletes if they do not exist in the file system.
    /// </summary>
    /// <returns></returns>
    public void PruneDatabaseEntries()
    {
        _log.Information($"Pruning database...");

        var entries = _db.GetReplayFiles();

        if (!entries.Any()) return;

        foreach (var entry in entries)
        {
            // Files does not exist! (Technically this is the same as id, but it's more clear)
            // or File is not part of the current source folder collection 
            if (!File.Exists(entry.FileInfo.Path) || !_fileSystem.IsPathInSourceFolders(entry.FileInfo.Path))
            {
                _log.Information($"File {entry.Id} is no longer valid, removing from database...");
                _db.RemoveFileResult(entry.Id);
            }
        }

        _log.Information($"Pruning complete");
    }

    /// <summary>
    /// searchResultCount is -1 if results are direct from database
    /// </summary>
    /// <param name="searchParameters"></param>
    /// <param name="maxEntries"></param>
    /// <param name="skip"></param>
    /// <param name="resetSearch"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public (IReadOnlyCollection<FileResult>, int searchResultCount) GetReplays(SearchParameters searchParameters, int maxEntries, int skip, bool resetSearch = false)
    {
        if (searchParameters == null) { throw new ArgumentNullException(nameof(searchParameters)); }

        if (string.IsNullOrEmpty(searchParameters.QueryString))
        {
            return (_db.QueryReplayFiles(Array.Empty<string>(), searchParameters.SortMethod, maxEntries, skip), -1);
        }

        var (results, searchResultCount) = _search.Query(searchParameters, maxEntries, skip, _config.SearchMinimumScore, resetSearch);

        return (_db.GetFileResults(results.Select(x => x.Id)), searchResultCount);
    }

    public string RenameReplay(FileResult file, string newName)
    {
        return _config.RenameFile
            ? RenameReplayInFileSystem(file, newName)
            : RenameReplayInDatabase(file, newName);
    }

    private string RenameReplayInDatabase(FileResult file, string newName)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (String.IsNullOrEmpty(newName)) throw new Exception("{EMPTY ERROR}");

        try
        {
            _db.UpdateAlternativeName(file.Id, newName);
            _search.UpdateDocumentName(file, newName);
        }
        catch (KeyNotFoundException ex)
        {
            _log.Information(ex.ToString());
            throw new Exception("{NOT FOUND ERROR}", ex);
        }

        // Return value file path, no changes made to filesystem so return same id
        return file.Id;
    }

    private string RenameReplayInFileSystem(FileResult file, string newName)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (String.IsNullOrEmpty(newName)) throw new Exception("{EMPTY ERROR}");

        var nameWithExtension = newName.EndsWith(".rofl")
            ? newName
            : newName + ".rofl";

        var newPath = Path.Combine(Path.GetDirectoryName(file.Id), nameWithExtension);

        _log.Information($"Renaming {file.Id} -> {newPath}");
        // Rename the file
        try
        {
            File.Move(file.Id, newPath);
        }
        catch (Exception ex)
        {
            _log.Information(ex.ToString());
            throw new Exception("{FAILED TO WRITE}", ex);
        }

        // delete the database entry
        _db.RemoveFileResult(file.Id);
        _search.RemoveDocument(file.Id);

        // Update new values
        var fileInfo = file.FileInfo;
        fileInfo.Name = nameWithExtension;
        fileInfo.Path = newPath;

        var replayFile = file.ReplayFile;
        replayFile.Name = nameWithExtension;
        replayFile.Location = newPath;

        var newFileResult = new FileResult(fileInfo, replayFile);
        _db.AddFileResult(newFileResult);
        _search.AddDocument(newFileResult);
        _search.CommitIndex();

        // return new file location
        return newPath;
    }

    /// <summary>
    /// Doesn't actually delete, but moves it to the cache folder, in case they didnt mean to delete it
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public string DeleteFile(FileResult file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));

        _log.Information($"Moving {file.Id} to cache folder - to be deleted when ReplayBook closes");

        var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "deletedReplays");
        Directory.CreateDirectory(newPath);

        newPath = Path.Combine(newPath, file.FileInfo.Name + ".rofl");

        File.Move(file.Id, newPath);

        _db.RemoveFileResult(file.Id);

        _deletedFiles.Add(newPath);
        return newPath;
    }

    public void ClearDeletedFiles()
    {
        foreach (var file in _deletedFiles)
        {
            _log.Information($"Deleting file {file}");

            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        _deletedFiles.Clear();
    }
}
