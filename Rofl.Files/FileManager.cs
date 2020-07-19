using Etirps.RiZhi;
using Rofl.Files.Models;
using Rofl.Files.Repositories;
using Rofl.Reader;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rofl.Files
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class FileManager
    {
        private readonly FolderRepository _fileSystem;
        private readonly DatabaseRepository _db;
        private readonly RiZhi _log;
        private readonly ReplayReader _reader;

        public FileManager(ObservableSettings settings, RiZhi log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _fileSystem = new FolderRepository(settings, log);
            _db = new DatabaseRepository(log);

            _reader = new ReplayReader(log);
        }

        /// <summary>
        /// This function is responsible for finding and loading in new replays
        /// </summary>
        public async Task InitialLoadAsync()
        {
            _log.Information("Starting initial load of replays");

            List<ReplayFileInfo> newFiles = new List<ReplayFileInfo>();
            
            // Get all files from all defined replay folders
            IReadOnlyCollection<ReplayFileInfo> allFiles = _fileSystem.GetAllReplayFileInfo();
            
            // Check if file exists in the database
            foreach (var file in allFiles)
            {
                if (_db.GetFileResult(file.Path) == null)
                {
                    newFiles.Add(file);
                }
            }

            _log.Information($"Discovered {newFiles.Count} new files");

            // Files not in the database are parsed and added
            foreach (var file in newFiles)
            {
                var parseResult = await _reader.ReadFile(file.Path).ConfigureAwait(false);

                FileResult newResult = new FileResult(file, parseResult)
                {
                    IsNewFile = false
                };

                _db.AddFileResult(newResult);
            }

            _log.Information("Initial load of replays complete");
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
            var parseResult = await _reader.ReadFile(path).ConfigureAwait(false);
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

            foreach(var entry in entries)
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

        public string RenameFile(FileResult file, string newName)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (String.IsNullOrEmpty(newName)) throw new ArgumentNullException(nameof(newName));

            var newPath = Path.Combine(Path.GetDirectoryName(file.Id), newName + ".rofl");

            _log.Information($"Renaming {file.Id} -> {newPath}");
            // Rename the file
            File.Move(file.Id, newPath);

            // delete the database entry
            _db.RemoveFileResult(file.Id);

            // Update new values
            var fileInfo = file.FileInfo;
            fileInfo.Name = newName;
            fileInfo.Path = newPath;

            var replayFile = file.ReplayFile;
            replayFile.Name = newName;
            replayFile.Location = newPath;

            var newFileResult = new FileResult(fileInfo, replayFile);
            _db.AddFileResult(newFileResult);

            return newPath;
        }
    }
}
