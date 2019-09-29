using Microsoft.Extensions.Configuration;
using Rofl.Files.Models;
using Rofl.Files.Repositories;
using Rofl.Logger;
using Rofl.Reader;
using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.Files
{
    public class FileManager
    {
        private FolderRepository _fileSystem;
        //private DatabaseRepository _database;

        private IConfiguration _config;

        private Scribe _log;

        private ReplayReader _reader;

        public FileManager(IConfiguration config, Scribe log)
        {
            _config = config;
            _log = log;

            _fileSystem = new FolderRepository(config, log);
            //_database = new DatabaseRepository(log);

            _reader = new ReplayReader();
        }

        /// <summary>
        /// Gets all <see cref="ReplayFile"/> objects paired with a flag indicating if it was a cache miss or not.
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult[]> GetReplayFilesAsync()
        {

            // Get all replay file infos...
            ReplayFileInfo[] allFiles = _fileSystem.GetAllReplayFileInfo();

            List<FileResult> totalReplays = new List<FileResult>();

            foreach (var fileInfo in allFiles)
            {
                string replayName = Path.GetFileNameWithoutExtension(fileInfo.Path);
                string replayPath = fileInfo.Path;

                // Ask database repository if file exists, using file path as the key
                // If hit: Use database entry to create new ReplayFile
                // If not hit:
                ReplayFile cacheResult = null;// _database.GetReplayFile(replayPath);
                if (false)//(cacheResult != null)
                {
                    _log.Info(this.GetType().Name, $"Database hit: {replayPath}");

                    totalReplays.Add(new FileResult()
                    {
                        ReplayFile = cacheResult,
                        FileInfo = fileInfo,
                        IsNewFile = false
                    });
                }
                else
                {
                    _log.Info(this.GetType().Name, $"Database miss: {replayPath}");
                    // create new tasks to read replays
                    totalReplays.Add(new FileResult()
                    {
                        ReplayFile = await _reader.ReadFile(replayPath),
                        FileInfo = fileInfo,
                        IsNewFile = true
                    });
                }
            }

            return (from result in totalReplays
                    orderby result.FileInfo.CreationTime descending
                    select result).ToArray();
        }
    }
}
