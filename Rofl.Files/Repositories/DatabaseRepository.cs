using LiteDB;
using Rofl.Files.Models;
using Rofl.Logger;
using Rofl.Reader.Models;
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
        private readonly Scribe _log;
        private readonly string _myName;
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "replayCache.db");

        public DatabaseRepository(Scribe log)
        {
            _log = log;
            _myName = this.GetType().ToString();

            try
            {
                InitializeDatabase();
            }
            catch (Exception)
            {
                _log.Warning(_myName, "Database file is invalid, deleting and trying again");
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

                fileResults.EnsureIndex(x => x.FileInfo.Name);
                fileResults.EnsureIndex(x => x.FileInfo.FileSizeBytes);
                fileResults.EnsureIndex(x => x.FileInfo.CreationTime);
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
                if (fileResults.FindById(_filePath) == null)
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

        public FileResult GetFileResult(string path)
        {
            if (string.IsNullOrEmpty(path)) { throw new ArgumentNullException(path); }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileResults = db.GetCollection<FileResult>("fileResults");

                var result = fileResults
                    .IncludeAll()
                    .FindById(path);

                return result;
            }
        }

        public IReadOnlyCollection<FileResult> QueryReplayFiles(string[] keywords, SortMethod sort, int maxEntries, int skip)
        {
            if (keywords == null) { throw new ArgumentNullException(nameof(keywords)); }

            Query sortQuery;
            switch (sort)
            {
                default:
                    sortQuery = Query.All("FileInfo.CreationTime", Query.Ascending);
                    break;
                case SortMethod.DateDesc:
                    sortQuery = Query.All("FileInfo.CreationTime", Query.Descending);
                    break;
                case SortMethod.SizeAsc:
                    sortQuery = Query.All("FileInfo.FileSizeBytes", Query.Ascending);
                    break;
                case SortMethod.SizeDesc:
                    sortQuery = Query.All("FileInfo.FileSizeBytes", Query.Descending);
                    break;
                case SortMethod.NameAsc:
                    sortQuery = Query.All("FileInfo.Name", Query.Ascending);
                    break;
                case SortMethod.NameDesc:
                    sortQuery = Query.All("FileInfo.Name", Query.Descending);
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
    }
}
