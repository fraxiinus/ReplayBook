using System;
using System.IO;
using System.Threading.Tasks;
using Rofl.Reader.Parsers;
using Rofl.Reader.Models;

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

            switch (file.Type)
            {
                case REPLAYTYPES.ROFL:
                    file.Data = await ReadROFL(file.Location);
                    break;
                case REPLAYTYPES.LRF:
                    file.Data = await ReadLRF(file.Location);
                    break;
                case REPLAYTYPES.LPR:
                    file.Data = await ReadLPR(file.Location);
                    break;
            }

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
