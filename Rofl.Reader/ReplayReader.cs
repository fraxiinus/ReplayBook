using Etirps.RiZhi;
using System;
using System.IO;
using System.Threading.Tasks;
using Rofl.Reader.Parsers;
using Rofl.Reader.Models;
using Rofl.Reader.Utilities;
using Rofl.Reader.Models.Internal.ROFL;
using System.Linq;

namespace Rofl.Reader
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class ReplayReader
    {
        private readonly RiZhi _log;

        public ReplayReader(RiZhi log)
        {
            _log = log;
        }

        public async Task<ReplayFile> ReadFile(string filePath)
        {
            // Make sure file exists
            if (String.IsNullOrEmpty(filePath))
            {
                _log.Error("File reference is null");
                throw new ArgumentNullException($"File reference is null");
            }

            if (!File.Exists(filePath))
            {
                _log.Error("File path not found, does the file exist?");
                throw new FileNotFoundException($"File path not found, does the file exist?");
            }

            // Reads the first 4 bytes and tries to find out the replay type
            ReplayType type = await ParserHelpers.GetReplayTypeAsync(filePath);
            
            // Match parsers to file types
            ReplayFile result;
            switch (type)
            {
                case ReplayType.ROFL:   // Official Replays
                    result = await ReadROFL(filePath);
                    break;
                //case ReplayType.LRF:    // LOLReplay
                //    file.Type = ReplayType.LRF;
                //    file.Data = await ReadLRF(file.Location);
                //    break;
                //case ReplayType.LPR:    // BaronReplays
                //    file.Type = ReplayType.LPR;
                //    file.Data = null;
                //    break;
                default:
                    _log.Error($"File {filePath} is not an accepted format: rofl");
                    return null;
            }

            // Make some educated guesses
            GameDetailsInferrer detailsInferrer = new GameDetailsInferrer();

            result.Players = result.BluePlayers.Union(result.RedPlayers).ToArray();

            try
            {
                result.MapId = detailsInferrer.InferMap(result.Players);
            }
            catch (ArgumentNullException ex)
            {
                _log.Warning("Could not infer map type\n" + ex.ToString());
                result.MapId = MapCode.Unknown;
            }
            
            result.MapName = detailsInferrer.GetMapName(result.MapId);
            result.IsBlueVictorious = detailsInferrer.InferBlueVictory(result.BluePlayers, result.RedPlayers);

            foreach (var player in result.Players)
            {
                player.Id = $"{result.MatchId}_{player.PlayerID}";
            }

            // Set the alternate name to the default
            result.AlternativeName = result.Name;

            return result;
        }

        public async Task<ReplayFile> ReadROFL(string filePath)
        {
            // Create a new parser
            ROFLParser roflParser = new ROFLParser();

            // Open file stream and parse
            ROFLHeader parseResult = null;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                parseResult = (ROFLHeader) await roflParser.ReadReplayAsync(fileStream);
            }

            // Create replay file based on header
            return new ReplayFile
            {
                Type = ReplayType.ROFL,
                Name = Path.GetFileNameWithoutExtension(filePath),
                Location = filePath,
                MatchId = parseResult.PayloadFields.MatchId.ToString(),
                GameDuration = TimeSpan.FromMilliseconds(parseResult.MatchMetadata.GameDuration),
                GameVersion = parseResult.MatchMetadata.GameVersion,
                BluePlayers = parseResult.MatchMetadata.BluePlayers ?? new Player[0],
                RedPlayers = parseResult.MatchMetadata.RedPlayers ?? new Player[0],
                RawJsonString = parseResult.RawJsonString
            };
        }

        //public async Task<ReplayHeader> ReadLRF(string filePath)
        //{
        //    var lrfParser = new LrfParser();

        //    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        //    {
        //        return await lrfParser.ReadReplayAsync(fileStream);
        //    }
        //}

        //// Broken, do not use
        //public async Task<ReplayHeader> ReadLPR(string filePath)
        //{
        //    var lprParser = new LprParser();

        //    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        //    {
        //        return await lprParser.ReadReplayAsync(fileStream);
        //    }
        //}
    }
}
