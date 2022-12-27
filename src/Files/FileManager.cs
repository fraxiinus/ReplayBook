using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Repositories;
using Fraxiinus.Rofl.Extract.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.Files
{
    public class FileManager
    {
        private readonly FolderRepository _fileSystem;
        private readonly DatabaseRepository _db;
        private readonly RiZhi _log;
        private readonly List<string> _deletedFiles;
        private readonly ObservableConfiguration _config;

        public FileManager(ObservableConfiguration config, RiZhi log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _config = config;
            _fileSystem = new FolderRepository(config, log);
            _db = new DatabaseRepository(config, log);

            _deletedFiles = new List<string>();
        }

        public string DatabasePath { get => _db.GetDatabasePath(); }

        public void DeleteDatabase() => _db.DeleteDatabase();

        /// <summary>
        /// This function is responsible for finding and loading in new replays
        /// </summary>
        public async Task<IEnumerable<FileErrorResult>> InitialLoadAsync()
        {
            _log.Information("Starting initial load of replays");

            //List<ReplayFileInfo> newFiles = new List<ReplayFileInfo>();

            var errorResults = new List<FileErrorResult>();
            int newCount = 0;

            // Get all files from all defined replay folders
            IReadOnlyCollection<ReplayFileInfo> allFiles = _fileSystem.GetAllReplayFileInfo();

            // Check if file exists in the database
            foreach (ReplayFileInfo file in allFiles)
            {
                if (_db.GetFileResult(file.Path) == null)
                {
                    try
                    {
                        // Reader.Models.ReplayFile parseResult = await _reader.ReadFile(file.Path).ConfigureAwait(false);

                        var parseResult = new ROFL(file.Path);
                        await parseResult.LoadAsync(false);

                        var newResult = new FileResult(file, parseResult)
                        {
                            IsNewFile = false
                        };

                        _db.AddFileResult(newResult);
                        newCount++;
                    }
                    catch (Exception ex)
                    {
                        _log.Warning($"Failed to parse file: {file.Path}");
                        _log.Warning(ex.ToString());
                        errorResults.Add(new FileErrorResult
                        {
                            FilePath = file.Path,
                            Exception = ex
                        });
                    }
                }
            }

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
                _log.Information($"File {path} already exists in database. Match ID: {returnValue.ReplayFile.PayloadHeader.GameId}");
                return returnValue;
            }

            var replayFileInfo = _fileSystem.GetSingleReplayFileInfo(path);
            // var parseResult = await _reader.ReadFile(path).ConfigureAwait(false);

            var parseResult = new ROFL(path);
            await parseResult.LoadAsync(false);

            if (parseResult is null) return null;

            var newResult = new FileResult(replayFileInfo, parseResult)
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

        public IReadOnlyCollection<FileResult> GetReplays(QueryProperties sort, int maxEntries, int skip)
        {
            if (sort == null) { throw new ArgumentNullException(nameof(sort)); }

            var keywords = sort.SearchTerm.Split('"')       // split the string by quotes
                .Select((element, index) => // select the substring, and the index of the substring
                    index % 2 == 0  // If the index is even (after a close quote)
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) // split by space
                    : new string[] { element }) // return the string enclosed by quotes
                .SelectMany(element => element).ToArray();

            return _db.QueryReplayFiles(keywords, sort.SortMethod, maxEntries, skip);
        }

        public string RenameReplay(FileResult file, string newName)
        {
            if (_config.RenameFile)
            {
                return RenameFile(file, newName);
            }
            else
            {
                return RenameAlternative(file, newName);
            }
        }

        private string RenameAlternative(FileResult file, string newName)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (String.IsNullOrEmpty(newName)) return "{EMPTY ERROR}";

            try
            {
                _db.UpdateAlternativeName(file.Id, newName);
            }
            catch (KeyNotFoundException ex)
            {
                _log.Information(ex.ToString());
                return "{NOT FOUND ERROR}";
            }

            // Return value is an error message, no message means no error
            return null;
        }

        private string RenameFile(FileResult file, string newName)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (String.IsNullOrEmpty(newName)) return "{EMPTY ERROR}";

            var newPath = Path.Combine(Path.GetDirectoryName(file.Id), newName + ".rofl");

            _log.Information($"Renaming {file.Id} -> {newPath}");
            // Rename the file
            try
            {
                File.Move(file.Id, newPath);
            }
            catch (Exception e)
            {
                return e.Message.Trim();
            }

            // delete the database entry
            _db.RemoveFileResult(file.Id);

            // Update new values
            var fileInfo = file.FileInfo;
            fileInfo.Name = newName;
            fileInfo.Path = newPath;

            //var replayFile = file.ReplayFile;
            //replayFile.Name = newName;
            //replayFile.Location = newPath;

            var newFileResult = new FileResult(fileInfo, file.ReplayFile);
            _db.AddFileResult(newFileResult);

            // Return value is an error message, no message means no error
            return null;
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
}
