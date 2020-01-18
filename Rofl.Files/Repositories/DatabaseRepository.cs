using LiteDB;
using Rofl.Files.Models;
using Rofl.Logger;
using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            using (var db = new LiteDatabase(_filePath))
            {
                var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
                var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
                var players = db.GetCollection<Player>("players");

                replayFiles.EnsureIndex(x => x.Players, "$.Players[*].NAME");
                replayFiles.EnsureIndex(x => x.Players, "$.Players[*].SKIN");

                BsonMapper.Global.Entity<ReplayFileInfo>()
                    .Id(r => r.Path);

                BsonMapper.Global.Entity<Player>()
                    .Id(r => r.Id);

                BsonMapper.Global.Entity<ReplayFile>()
                    .Id(r => r.Location)
                    .DbRef(r => r.Players, "players");
            }
        }

        public bool AddFileResult(FileResult result)
        {
            if (result == null) { throw new ArgumentNullException(nameof(result)); }
            if (result.ReplayFile == null) { throw new ArgumentNullException(nameof(result)); }
            if (result.FileInfo == null) { throw new ArgumentNullException(nameof(result)); }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
                var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
                var players = db.GetCollection<Player>("players");

                // If we already have the file, do nothing
                if (fileInfos.Exists(
                        x => x.Path.Equals(result.FileInfo.Path, StringComparison.OrdinalIgnoreCase))
                    )
                {
                    return false;
                }

                foreach (var player in result.ReplayFile.Players)
                {
                    // If the player already exists, do nothing
                    if(!players.Exists(
                            x => x.Id.Equals(player.Id, StringComparison.OrdinalIgnoreCase))
                       )
                    {
                        players.Insert(player);
                    }
                }

                fileInfos.Insert(result.FileInfo);
                replayFiles.Insert(result.ReplayFile);

                return true;
            }
        }

        public FileResult GetFileResult(string path)
        {
            if (string.IsNullOrEmpty(path)) { throw new ArgumentNullException(path); }

            using (var db = new LiteDatabase(_filePath))
            {
                var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
                var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
                var players = db.GetCollection<Player>("players");

                if (fileInfos.Exists(
                        x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
                    )
                {
                    return new FileResult
                    {
                        FileInfo = fileInfos
                            .FindOne(
                                x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase)
                            ),

                        ReplayFile = replayFiles
                            .Include(
                                x => x.Players
                            )
                            .FindOne(
                                x => x.Location.Equals(path, StringComparison.OrdinalIgnoreCase)
                            ),

                        IsNewFile = false
                    };
                }

                return null;
            }
        }

        private void TestDb()
        {
            var replayFileInfo = new ReplayFileInfo
            {
                CreationTime = DateTime.Now,
                Path = "D:\\Etirps\\Documents\\League of Legends\\Replays\\NA1-3260770635.rofl",
                FileSizeBytes = 20522894
            };

            var replayFile = new ReplayFile
            {
                Location = replayFileInfo.Path,
                Players = new Player[]
                {
                    new Player
                    {
                        Id = "3260770635_41257309"
                    },
                    new Player
                    {
                        Id = "3260770635_33251497"
                    }
                }
            };

            using (var db = new LiteDatabase(_filePath))
            {
                var fileInfos = db.GetCollection<ReplayFileInfo>("replayFileInfo");
                var replayFiles = db.GetCollection<ReplayFile>("replayFiles");
                var players = db.GetCollection<Player>("players");

                players.Insert(replayFile.Players[0]);
                players.Insert(replayFile.Players[1]);

                replayFiles.Insert(replayFile);

                var test = replayFiles.Include(x => x.Players).FindAll().ToList();
            }
        }
    }
}
