using System;
using System.IO;
using System.Threading.Tasks;
using Rofl.Parsers.Parsers;
using Rofl.Parsers.Models;

namespace Rofl.Parsers
{
    public class ReplayReader
    {
        private readonly string exceptionOriginName = "ReplayReader";

        /// <summary>
        /// Given non-null ReplayFile object with valid Location and Name,
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
                case REPLAYTYPES.LOLR:
                    throw new NotImplementedException($"{exceptionOriginName} - LoLReplay support not yet implemented");
                case REPLAYTYPES.BARON:
                    throw new NotImplementedException($"{exceptionOriginName} - BaronReplay support not yet implemented");
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
    }
}
