using System;
using System.IO;
using System.Threading.Tasks;
using Rofl.Reader.Parsers;
using Rofl.Reader.Models;
using Rofl.Reader.Utilities;

namespace Rofl.Reader
{
    public class ReplayReader
    {
        private readonly string exceptionOriginName = "ReplayReader";

        /// <summary>
        /// Given non-null ReplayFile object with valid Location, Name, and Type - 
        /// Returns ReplayFile object with filled out Data.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<ReplayFile> ReadFile(ReplayFile file)
        {
            if (file == null || String.IsNullOrEmpty(file.Location) || String.IsNullOrEmpty(file.Name))
            {
                throw new ArgumentNullException($"{exceptionOriginName} - File reference is null");
            }

            if (!File.Exists(file.Location))
            {
                throw new FileNotFoundException($"{exceptionOriginName} - File path not found, does the file exist?");
            }

            // Unsure if "ToLower" needs to be specified, just to be safe...
            switch (Path.GetExtension(file.Location).ToLower())
            {
                case ".rofl":
                    file.Type = REPLAYTYPES.ROFL;
                    file.Data = await ReadROFL(file.Location);
                    break;
                case ".lrf":
                    file.Type = REPLAYTYPES.LRF;
                    file.Data = await ReadLRF(file.Location);
                    break;
                // LPR is baronreplays, which is not working currently
                case ".lpr":
                    file.Type = REPLAYTYPES.LPR;
                    file.Data = null;
                    break;
                default:
                    throw new NotSupportedException($"{exceptionOriginName} - File is not an accepted format: (rofl, lrf)");
            }

            // Make some educated guesses
            GameDetailsInferrer detailsInferrer = new GameDetailsInferrer();

            file.Data.InferredData = new InferredData()
            {
                Id = file.Data.PayloadFields.MatchId,
                MapID = detailsInferrer.InferMap(file.Data.MatchMetadata),
                BlueVictory = detailsInferrer.InferBlueVictory(file.Data.MatchMetadata)
            };

            return file;
        }

        public async Task<ReplayHeader> ReadROFL(string filePath)
        {
            var roflParser = new RoflParser();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                return await roflParser.ReadReplayAsync(fileStream);
            }
        }

        public async Task<ReplayHeader> ReadLRF(string filePath)
        {
            var lrfParser = new LrfParser();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                return await lrfParser.ReadReplayAsync(fileStream);
            }
        }

        // Broken, do not use
        public async Task<ReplayHeader> ReadLPR(string filePath)
        {
            var lprParser = new LprParser();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                return await lprParser.ReadReplayAsync(fileStream);
            }
        }
    }
}
