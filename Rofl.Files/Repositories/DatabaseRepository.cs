//using LiteDB;
//using Rofl.Logger;
//using Rofl.Reader.Models;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Rofl.Files.Repositories
//{
//    /// <summary>
//    /// Acts as the cache for replay parsing.
//    /// </summary>
//    public class DatabaseRepository
//    {
//        // Program flow will be like this:
//        // Open program -> check folders for new files to parse -> send files to rofl.reader 
//        // -> add results to database -> notify ..something? that there are new entries to display

//        private Scribe _log;

//        private LiteDatabase _cacheDb;

//        private LiteCollection<ReplayFile> _replayCollection;

//        private LiteCollection<ReplayHeader> _headerCollection;

//        private LiteCollection<LengthFields> _lengthHeaderFields;

//        private LiteCollection<MatchMetadata> _matchHeaderFields;

//        private LiteCollection<PayloadFields> _payloadHeaderFields;

//        private LiteCollection<InferredData> _inferredDataFields;

//        public DatabaseRepository(Scribe log)
//        {
//            _log = log;

//            Directory.CreateDirectory("data");

//            _cacheDb = new LiteDatabase(@"data/cache.db");
//            _lengthHeaderFields = _cacheDb.GetCollection<LengthFields>("length_header_cache");
//            _matchHeaderFields = _cacheDb.GetCollection<MatchMetadata>("match_header_cache");
//            _payloadHeaderFields = _cacheDb.GetCollection<PayloadFields>("payload_header_cache");
//            _inferredDataFields = _cacheDb.GetCollection<InferredData>("inferred_data_cache");
//            _headerCollection = _cacheDb.GetCollection<ReplayHeader>("replay_header_cache");

//            _replayCollection = _cacheDb.GetCollection<ReplayFile>("replay_file_cache")
//                .Include(x => x.Data)
//                .Include(x => x.Data.LengthFields)
//                .Include(x => x.Data.MatchMetadata)
//                .Include(x => x.Data.PayloadFields)
//                .Include(x => x.Data.InferredData);

//            BsonMapper.Global.Entity<LengthFields>()
//                .Id(e => e.Id);

//            BsonMapper.Global.Entity<MatchMetadata>()
//                .Id(e => e.Id);

//            BsonMapper.Global.Entity<PayloadFields>()
//                .Id(e => e.MatchId);

//            BsonMapper.Global.Entity<InferredData>()
//                .Id(e => e.Id);

//            BsonMapper.Global.Entity<ReplayFile>()
//                .Id(e => e.Location)
//                .DbRef(e => e.Data, "replay_header_cache");

//            BsonMapper.Global.Entity<ReplayHeader>()
//                .Id(e => e.Id)
//                .DbRef(e => e.LengthFields, "length_header_cache")
//                .DbRef(e => e.MatchMetadata, "match_header_cache")
//                .DbRef(e => e.PayloadFields, "payload_header_cache")
//                .DbRef(e => e.InferredData, "inferred_data_cache");

//        }

//        ~DatabaseRepository()
//        {
//            _log.Error(this.GetType().Name, "lol");
//            _cacheDb.Dispose();
//        }

//        public ReplayFile GetReplayFile(string filePath)
//        {
//            var replayResult = _replayCollection.Find(Query.EQ("_id", filePath)).FirstOrDefault();

//            return replayResult;
//        }

//        public void UpdateOrInsertReplayFile(ReplayFile file)
//        {
//            if (!_replayCollection.Update(file))
//            {
//                _replayCollection.Insert(file);
//            }

//            if(!_headerCollection.Update(file.Data))
//            {
//                _headerCollection.Insert(file.Data);
//            }

//            if (!_lengthHeaderFields.Update(file.Data.LengthFields))
//            {
//                _lengthHeaderFields.Insert(file.Data.LengthFields);
//            }

//            if(!_matchHeaderFields.Update(file.Data.MatchMetadata))
//            {
//                _matchHeaderFields.Insert(file.Data.MatchMetadata);
//            }

//            if (!_payloadHeaderFields.Update(file.Data.PayloadFields))
//            {
//                _payloadHeaderFields.Insert(file.Data.PayloadFields);
//            }

//            if (!_inferredDataFields.Update(file.Data.InferredData))
//            {
//                _inferredDataFields.Insert(file.Data.InferredData);
//            }
//        }
//    }
//}
