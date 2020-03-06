using LiteDB;
using Microsoft.Extensions.Configuration;
using Rofl.Files.Models;
using Rofl.Files.Repositories;
using Rofl.Logger;
using Rofl.Reader;
using Rofl.Reader.Models;
using Rofl.Settings;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.Files
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class FileManager
    {
        private readonly FolderRepository _fileSystem;
        private readonly DatabaseRepository _db;
        private readonly Scribe _log;
        private readonly ReplayReader _reader;
        private readonly string _myName;

        public FileManager(ObservableSettings settings, Scribe log)
        {
            _log = log;
            _myName = this.GetType().ToString();

            _fileSystem = new FolderRepository(settings, log);
            _db = new DatabaseRepository(log);

            _reader = new ReplayReader(log);
        }

        /// <summary>
        /// This function is responsible for finding and loading in new replays
        /// </summary>
        public async Task InitialLoadAsync()
        {
            _log.Information(_myName, "Starting initial load of replays");

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

            _log.Information(_myName, $"Discovered {newFiles.Count} new files");

            // Files not in the database are parsed and added
            foreach (var file in newFiles)
            {
                var parseResult = await _reader.ReadFile(file.Path).ConfigureAwait(false);

                FileResult newResult = new FileResult
                {
                    ReplayFile = parseResult,
                    FileInfo = file,
                    IsNewFile = true
                };

                _db.AddFileResult(newResult);
            }

            _log.Information(_myName, "Initial load of replays complete");
        }

        public IReadOnlyCollection<FileResult> GetReplays(SortPropertiesModel sort, int maxEntries, int skip)
        {
            if (sort == null) { throw new ArgumentNullException(nameof(sort)); }

            Query query;
            switch (sort.SortMethod)
            {
                default:
                    query = Query.All("CreationTime", Query.Ascending);
                    break;
                case SortMethod.DateDesc:
                    query = Query.All("CreationTime", Query.Descending);
                    break;
                case SortMethod.SizeAsc:
                    query = Query.All("FileSizeBytes", Query.Ascending);
                    break;
                case SortMethod.SizeDesc:
                    query = Query.All("FileSizeBytes", Query.Descending);
                    break;
                case SortMethod.NameAsc:
                    query = Query.All("Name", Query.Ascending);
                    break;
                case SortMethod.NameDesc:
                    query = Query.All("Name", Query.Descending);
                    break;
            }

            return _db.QueryReplayFiles(query, maxEntries, skip);
        }

        /// <summary>
        /// Gets all <see cref="ReplayFile"/> objects paired with a flag indicating if it was a cache miss or not.
        /// </summary>
        /// <returns></returns>
        //public async Task<FileResult[]> GetReplayFilesAsync()
        //{

        //    // Get all replay file infos...
        //    ReplayFileInfo[] allFiles = _fileSystem.GetAllReplayFileInfo();

        //    List<FileResult> totalReplays = new List<FileResult>();

        //    foreach (var fileInfo in allFiles)
        //    {
        //        string replayName = Path.GetFileNameWithoutExtension(fileInfo.Path);
        //        string replayPath = fileInfo.Path;

        //        // Ask database repository if file exists, using file path as the key
        //        // If hit: Use database entry to create new ReplayFile
        //        // If not hit:
        //        FileResult item = _db.GetFileResult(replayPath);
        //        if (item != null)
        //        {
        //            _log.Information(_myName, $"Database hit: {replayPath}");
        //            totalReplays.Add(item);
        //        }
        //        else
        //        {
        //            _log.Information(_myName, $"Database miss: {replayPath}");
        //            // create new tasks to read replays
        //            FileResult newResult = new FileResult
        //            {
        //                ReplayFile = await _reader.ReadFile(replayPath).ConfigureAwait(false),
        //                FileInfo = fileInfo,
        //                IsNewFile = true
        //            };
        //            // add to cache

        //            _db.AddFileResult(newResult);
        //            totalReplays.Add(newResult);
        //        }
        //    }

        //    return (from result in totalReplays
        //            orderby result.FileInfo.CreationTime descending
        //            select result).ToArray();
        //}
    }
}
