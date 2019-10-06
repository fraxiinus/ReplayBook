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
    public class ReplayReader
    {
        private readonly string exceptionOriginName = "ReplayReader";

        public async Task<ReplayFile> ReadFile(string filePath)
        {
            // Make sure file exists
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException($"{exceptionOriginName} - File reference is null");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{exceptionOriginName} - File path not found, does the file exist?");
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
                    throw new NotSupportedException($"{exceptionOriginName} - File is not an accepted format: (rofl, lrf)");
            }

            // Make some educated guesses
            GameDetailsInferrer detailsInferrer = new GameDetailsInferrer();

            result.Players = result.BluePlayers.Union(result.RedPlayers).ToArray();
            result.MapId = detailsInferrer.InferMap(result.Players);
            result.MapName = detailsInferrer.GetMapName(result.MapId);
            result.IsBlueVictorious = detailsInferrer.InferBlueVictory(result.BluePlayers, result.RedPlayers);

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
                MatchId = parseResult.PayloadFields.MatchId,
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
