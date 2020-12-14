using Etirps.RiZhi;
using LiteDB;
using Rofl.Files.Models;
using Rofl.Reader.Models;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rofl.Files.Repositories
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class DatabaseRepository
    {
        private readonly RiZhi _log;
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "replayCache.db");
        private readonly ObservableSettings _settings;

        public DatabaseRepository(ObservableSettings settings, RiZhi log)
        {
            _settings = settings;
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

        private void InitializeDatabase()
        {
            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");

                BsonMapper.Global.Entity<FileResult>()
                    .Id(r => r.Id)
                    .DbRef(r => r.FileInfo, "replayFileInfo")
                    .DbRef(r => r.ReplayFile, "replayFiles");

                BsonMapper.Global.Entity<ReplayFileInfo>()
                    .Id(r => r.Path);

                BsonMapper.Global.Entity<Player>()
                    .Id(r => r.Id);

                BsonMapper.Global.Entity<ReplayFile>()
                    .Id(r => r.Location)
                    .DbRef(r => r.Players, "players");

                fileResults.EnsureIndex(x => x.FileName);
                fileResults.EnsureIndex(x => x.AlternativeName);
                fileResults.EnsureIndex(x => x.FileSizeBytes);
                fileResults.EnsureIndex(x => x.FileCreationTime);
                fileResults.EnsureIndex(x => x.PlayerNames);
                fileResults.EnsureIndex(x => x.ChampionNames);
            }
        }

        public void AddFileResult(FileResult result)
        {
            if (result == null) { throw new ArgumentNullException(nameof(result)); }
            if (result.ReplayFile == null) { throw new ArgumentNullException(nameof(result)); }
            if (result.FileInfo == null) { throw new ArgumentNullException(nameof(result)); }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");
                var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
                var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
                var players = db.GetCollection<Player>("players");

                // If we already have the file, do nothing
                if (fileResults.FindById(result.Id) == null)
                {
                    fileResults.Insert(result);
                }

                // Only add if it doesnt exist
                if (fileInfos.FindById(result.FileInfo.Path) == null)
                {
                    fileInfos.Insert(result.FileInfo);
                }

                if (replayFiles.FindById(result.ReplayFile.Location) == null)
                {
                    replayFiles.Insert(result.ReplayFile);
                }

                foreach (var player in result.ReplayFile.Players)
                {
                    // If the player already exists, do nothing
                    if (players.FindById(player.Id) == null)
                    {
                        players.Insert(player);
                    }
                }
            }
        }

        public void RemoveFileResult(string id)
        {
            if (string.IsNullOrEmpty(id)) { throw new ArgumentNullException(id); }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");
                var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
                var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
                var players = db.GetCollection<Player>("players");

                fileResults.Delete(id);

                fileInfos.Delete(id);

                replayFiles.Delete(id);

                // Rip player data is being orphaned...
            }
        }

        public FileResult GetFileResult(string id)
        {
            if (string.IsNullOrEmpty(id)) { throw new ArgumentNullException(id); }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");

                var result = fileResults
                    .IncludeAll()
                    .FindById(id);

                return result;
            }
        }

        public IEnumerable<FileResult> GetReplayFiles()
        {
            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");

                return fileResults.FindAll();
            }
        }

        public IReadOnlyCollection<FileResult> QueryReplayFiles(string[] keywords, SortMethod sort, int maxEntries, int skip)
        {
            if (keywords == null) { throw new ArgumentNullException(nameof(keywords)); }

            Query sortQuery;
            switch (sort)
            {
                default:
                    sortQuery = Query.All("FileCreationTime", Query.Ascending);
                    break;
                case SortMethod.DateDesc:
                    sortQuery = Query.All("FileCreationTime", Query.Descending);
                    break;
                case SortMethod.SizeAsc:
                    sortQuery = Query.All("FileSizeBytes", Query.Ascending);
                    break;
                case SortMethod.SizeDesc:
                    sortQuery = Query.All("FileSizeBytes", Query.Descending);
                    break;
                case SortMethod.NameAsc:
                    // Query either filename or alternative name
                    sortQuery = Query.All(_settings.RenameAction == RenameAction.File ? "FileName" : "AlternativeName", Query.Ascending);
                    break;
                case SortMethod.NameDesc:
                    sortQuery = Query.All(_settings.RenameAction == RenameAction.File ? "FileName" : "AlternativeName", Query.Descending);
                    break;
            }

            List<Query> playerQueries = new List<Query>();
            foreach (var word in keywords)
            {
                playerQueries.Add
                (
                    Query.Or
                    (
                        Query.Where("PlayerNames", players => players.AsString.Contains(word.ToUpper(CultureInfo.InvariantCulture))),
                        Query.Where("ChampionNames", champions => champions.AsString.Contains(word.ToUpper(CultureInfo.InvariantCulture)))
                    )
                );
            }
            
            Query endQuery;
            if (playerQueries.Any())
            {
                if (playerQueries.Count == 1)
                {
                    endQuery = Query.And(sortQuery, playerQueries[0]);
                }
                else
                {
                    var combinedPlayerQuery = Query.And(playerQueries.ToArray());
                    endQuery = Query.And(sortQuery, combinedPlayerQuery);
                }
            }
            else
            {
                endQuery = sortQuery;
            }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");

                return fileResults.IncludeAll().Find(endQuery, limit: maxEntries, skip: skip).ToList();
            }
        }

        public void UpdateAlternativeName(string id, string newName)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(id);

            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");

                var result = fileResults
                    .IncludeAll()
                    .FindById(id);

                if (result == null)
                {
                    throw new KeyNotFoundException($"Could not find FileResult by id {id}");
                }
                else
                {
                    _log.Information($"Db updating {result.AlternativeName} to {newName}");

                    // Update the file results (for indexing/search)
                    result.AlternativeName = newName;
                    fileResults.Update(result);

                    // Update the replay entry
                    var replays = db.GetCollection<ReplayFile>("replayFiles");
                    var replayEntry = replays
                        .IncludeAll()
                        .FindById(id);
                    replayEntry.AlternativeName = newName;
                    replays.Update(replayEntry);
                }
            }
        }
    }
}
