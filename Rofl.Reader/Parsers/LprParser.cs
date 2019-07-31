using Rofl.Reader.Models;
using Rofl.Reader.Utilities;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.Reader.Parsers
{
    public class LprParser : IReplayParser
    {
        private readonly string _exceptionOriginName = "LprParser";

        /// <summary>
        /// DO NOT USE
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public async Task<ReplayHeader> ReadReplayAsync(FileStream fileStream)
        {
            if (!fileStream.CanRead)
            {
                throw new IOException($"{_exceptionOriginName} - Stream does not support reading");
            }

            // Create ne LprHeader
            LprHeader header = new LprHeader();

            // These buffers will be used and reused to read data
            byte[] int32ByteBuffer = new byte[4];
            byte[] int64ByteBuffer = new byte[8];

            //// Try reading the "Magic number" it's actually the file version
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Magic Number", fileStream, int32ByteBuffer, 4);

            header.LprFileVersion = BitConverter.ToInt32(int32ByteBuffer, 0);

            // BaronReplay implements this check, not sure why
            if(!(header.LprFileVersion >= 0))
            {
                throw new IOException($"{_exceptionOriginName} - Lpr File Version Unsupported");
            }


            //// Try reading the length of the next section, so we can skip it
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Length of Spectator client version", fileStream, int32ByteBuffer, 4);

            //// Use the result of previous operation to make new array and pull string
            byte[] spectatorClientVersionBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Spectator client version", fileStream, spectatorClientVersionBytes, BitConverter.ToInt32(int32ByteBuffer, 0));

            header.SpectatorClientVersion = Encoding.UTF8.GetString(spectatorClientVersionBytes);
            
            
            //// Read the game ID
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Game ID", fileStream, int64ByteBuffer, 8);
            header.GameID = BitConverter.ToInt64(int64ByteBuffer, 0);


            //// Read Chunk data
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.GameEndStartupChunk = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.StartChunk = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.EndChunk = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.EndKeyframe = BitConverter.ToInt32(int32ByteBuffer, 0);


            //// Read game length
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Game length", fileStream, int32ByteBuffer, 4);
            header.GameLength = BitConverter.ToInt32(int32ByteBuffer, 0);


            //// Read more chunk/replay information
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.GameDelayTime = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.ClientAddLag = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.ChunkTimeInterval = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.KeyframeTimeInterval = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.ELOLevel = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.LastChunkTime = BitConverter.ToInt32(int32ByteBuffer, 0);

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Chunks", fileStream, int32ByteBuffer, 4);
            header.LastChunkDuration = BitConverter.ToInt32(int32ByteBuffer, 0);


            //// Read game region length
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Game region length", fileStream, int32ByteBuffer, 4);

            //// Use the length to make a new byte array to read the region string
            byte[] gameRegionBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];

            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Game region", fileStream, gameRegionBytes, BitConverter.ToInt32(int32ByteBuffer, 0));
            header.GamePlatform = Encoding.UTF8.GetString(gameRegionBytes);


            //// Get length of spectator encryption key
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Spectator encryption key length", fileStream, int32ByteBuffer, 4);

            //// Use the length of spectator encryption key to make a new array to put the encryption key
            byte[] spectatorEncryptionKeyBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading Spectator encryption key", fileStream, spectatorEncryptionKeyBytes, BitConverter.ToInt32(int32ByteBuffer, 0));

            header.SpectatorEncryptionKey = Encoding.UTF8.GetString(spectatorEncryptionKeyBytes);


            //// Get length of creation time
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading creation time length", fileStream, int32ByteBuffer, 4);

            //// Use the length of creation time to make a new array to put the creation time 
            byte[] creationTimeBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading creation time", fileStream, creationTimeBytes, BitConverter.ToInt32(int32ByteBuffer, 0));

            header.CreateTime = Encoding.UTF8.GetString(creationTimeBytes);


            //// Get length of start time
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading start time length", fileStream, int32ByteBuffer, 4);

            //// Use the length of start time to make a new array to put the start time
            byte[] startTimeBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading creation time", fileStream, startTimeBytes, BitConverter.ToInt32(int32ByteBuffer, 0));

            header.StartTime = Encoding.UTF8.GetString(startTimeBytes);


            //// Get length of end time
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading end time length", fileStream, int32ByteBuffer, 4);

            //// Use the length of start time to make a new array to put the start time
            byte[] endTimeBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading end time", fileStream, endTimeBytes, BitConverter.ToInt32(int32ByteBuffer, 0));

            header.EndTime = Encoding.UTF8.GetString(endTimeBytes);


            //// Get length of league of legends version
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading league of legends version length", fileStream, int32ByteBuffer, 4);

            //// Use the length of start time to make a new array to put the start time
            byte[] leagueVersionBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading end time", fileStream, leagueVersionBytes, BitConverter.ToInt32(int32ByteBuffer, 0));
            // TODO
            header.LeagueVersion = Encoding.UTF8.GetString(leagueVersionBytes);


            //// Do end game result exists?
            byte[] resultsExistByte = new byte[1];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading result boolean", fileStream, resultsExistByte, 1);
            // TODO
            bool resultsExist = BitConverter.ToBoolean(resultsExistByte, 0);

            if(resultsExist)
            {
                //await ReadEndGameResults(fileStream);
            }
            else
            {
                header.OldResults = await ReadOldEndGameResults(fileStream);
            }


            // TODO BROKEN
            return null;
        }

        /*
        private async Task ReadEndGameResults(FileStream fileStream)
        {
            //// Read all the result bytes
            byte[] lengthOfResultsBytes = new byte[4];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading results length", fileStream, lengthOfResultsBytes, 4);

            byte[] resultsBytes = new byte[BitConverter.ToInt32(lengthOfResultsBytes, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading results", fileStream, resultsBytes, BitConverter.ToInt32(lengthOfResultsBytes, 0));

            string test = Encoding.UTF8.GetString(resultsBytes);

            SerializationContext context = new SerializationContext(new Type[] { typeof(LprEndOfGameStats), typeof(LprPlayerParticipantStatsSummary), typeof(LprRawStatDTO) });

            AmfReader amfReader = new AmfReader(resultsBytes, context);

            var stats = (LprEndOfGameStats)amfReader.ReadAmf3Object();
        }
        */

        private async Task<LprOldResults> ReadOldEndGameResults(FileStream fileStream)
        {
            byte[] int32ByteBuffer = new byte[4];
            LprOldResults oldResults = new LprOldResults();

            //// Reading number of players
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading number of players", fileStream, int32ByteBuffer, 4);
            oldResults.NumberOfPlayers = BitConverter.ToInt32(int32ByteBuffer, 0);

            //// Read length of player name, use to grab player name
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading player name length", fileStream, int32ByteBuffer, 4);

            byte[] playerNameBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading player name", fileStream, playerNameBytes, BitConverter.ToInt32(int32ByteBuffer, 0));
            oldResults.PlayerName = Encoding.UTF8.GetString(playerNameBytes);


            //// Read length of champion name, use to grab champion name
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading champion name length", fileStream, int32ByteBuffer, 4);

            byte[] championNameBytes = new byte[BitConverter.ToInt32(int32ByteBuffer, 0)];
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading champion name", fileStream, championNameBytes, BitConverter.ToInt32(int32ByteBuffer, 0));
            oldResults.ChampionName = Encoding.UTF8.GetString(championNameBytes);


            //// Reading team
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading team", fileStream, int32ByteBuffer, 4);
            oldResults.Team = BitConverter.ToInt32(int32ByteBuffer, 0);


            //// Reading client id
            await ParserHelpers.ReadBytes($"{_exceptionOriginName} - Reading client id", fileStream, int32ByteBuffer, 4);
            oldResults.ClientID = BitConverter.ToInt32(int32ByteBuffer, 0);

            return oldResults;
        }
    }
}
