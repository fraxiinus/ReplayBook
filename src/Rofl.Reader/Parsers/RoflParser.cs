using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rofl.Reader.Models;
using Rofl.Reader.Models.Internal;
using Rofl.Reader.Models.Internal.ROFL;
using Rofl.Reader.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.Reader.Parsers
{
    /// <summary>
    /// Parses Official League of Legends Replays
    /// </summary>
    public class ROFLParser : IReplayParser
    {
        private readonly string exceptionOriginName = "RoflParser";

        public async Task<IReplayHeader> ReadReplayAsync(FileStream fileStream)
        {
            if(!fileStream.CanRead)
            {
                throw new IOException($"{exceptionOriginName} - Stream does not support reading");
            }

            // Read and check Magic Numbers
            ReplayType type;
            try
            {
                type = await ParserHelpers.GetReplayTypeAsync(fileStream);
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading Magic Number: " + ex.Message, ex);
            }

            if(type != ReplayType.ROFL)
            {
                throw new Exception($"{exceptionOriginName} - Selected file is not in valid ROFL format");
            }

            // Read and deserialize length fields
            byte[] lengthFieldBuffer;
            try
            {
                lengthFieldBuffer = await ParserHelpers.ReadBytesAsync(fileStream, 26, 262, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading Length Header: " + ex.Message, ex);
            }

            LengthFields lengthFields;
            try
            {
                lengthFields = ParseLengthFields(lengthFieldBuffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"{exceptionOriginName} - Parsing Length Header: " + ex.Message, ex);
            }


            // Read and deserialize metadata
            byte[] metadataBuffer;
            try
            {
                metadataBuffer = await ParserHelpers.ReadBytesAsync(fileStream, (int)lengthFields.MetadataLength, (int)lengthFields.MetadataOffset, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading JSON Metadata: " + ex.Message, ex);
            }

            MatchMetadata metadataFields;
            try
            {
                metadataFields = ParseMetadata(metadataBuffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"{exceptionOriginName} - Parsing Metadata Header: " + ex.Message, ex);
            }

            // Read and deserialize payload fields
            byte[] payloadBuffer;
            try
            {
                payloadBuffer = await ParserHelpers.ReadBytesAsync(fileStream, (int)lengthFields.PayloadHeaderLength, (int)lengthFields.PayloadHeaderOffset, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading Match Header: " + ex.Message, ex);
            }

            PayloadFields payloadFields;
            try
            {
                payloadFields = ParsePayloadHeader(payloadBuffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"{exceptionOriginName} - Parsing Payload Header: " + ex.Message, ex);
            }
            

            // Combine objects to create header
            ROFLHeader result = new ROFLHeader
            {
                LengthFields = lengthFields,
                MatchMetadata = metadataFields,
                PayloadFields = payloadFields
            };

            // Create json of entire contents
            string jsonString = JsonConvert.SerializeObject(result);

            // throw it back on the object and return
            result.RawJsonString = jsonString;
            return result;
        }

        private static PayloadFields ParsePayloadHeader(byte[] bytedata)
        {
            var result = new PayloadFields { };

            result.MatchId = BitConverter.ToUInt64(bytedata, 0);
            result.MatchLength = BitConverter.ToUInt32(bytedata, 8);
            result.KeyframeAmount = BitConverter.ToUInt32(bytedata, 12);
            result.ChunkAmount = BitConverter.ToUInt32(bytedata, 16);
            result.EndChunkID = BitConverter.ToUInt32(bytedata, 20);
            result.StartChunkID = BitConverter.ToUInt32(bytedata, 24);
            result.KeyframeInterval = BitConverter.ToUInt32(bytedata, 28);
            result.EncryptionKeyLength = BitConverter.ToUInt16(bytedata, 32);
            result.EncryptionKey = Encoding.UTF8.GetString(bytedata, 34, result.EncryptionKeyLength);

            return result;
        }

        private static MatchMetadata ParseMetadata(byte[] bytedata)
        {
            var result = new MatchMetadata { };
            var jsonstring = Encoding.UTF8.GetString(bytedata);

            var jsonobject = JObject.Parse(jsonstring);

            result.GameDuration = (ulong)jsonobject["gameLength"];
            result.GameVersion = (string)jsonobject["gameVersion"];
            result.LastGameChunkID = (uint)jsonobject["lastGameChunkId"];
            result.LastKeyframeID = (uint)jsonobject["lastKeyFrameId"];

            // Create new lists of player dictionaries for sorting
            var blueTeam = new List<Dictionary<string, string>>();
            var redTeam = new List<Dictionary<string, string>>();

            // Read player stats
            string playerJson = ((string)jsonobject["statsJson"]).Replace(@"\", "");
            var players = JsonConvert.DeserializeObject<Player[]>(playerJson);

            // Sort players into teams
            var teams = (from player in players
                         group player by player.TEAM into t
                         orderby t.Key
                         select t);

            // Set teams by key
            foreach (var team in teams)
            {
                if(team.Key == "100")
                {
                    result.BluePlayers = team.ToArray();
                }
                if (team.Key == "200")
                {
                    result.RedPlayers = team.ToArray();
                }
            }
            
            return result;
        }

        private static LengthFields ParseLengthFields(byte[] bytedata)
        {
            var result = new LengthFields { };
            result.HeaderLength = BitConverter.ToUInt16(bytedata, 0);
            result.FileLength = BitConverter.ToUInt32(bytedata, 2);
            result.MetadataOffset = BitConverter.ToUInt32(bytedata, 6);
            result.MetadataLength = BitConverter.ToUInt32(bytedata, 10);
            result.PayloadHeaderOffset = BitConverter.ToUInt32(bytedata, 14);
            result.PayloadHeaderLength = BitConverter.ToUInt32(bytedata, 18);
            result.PayloadOffset = BitConverter.ToUInt32(bytedata, 22);

            return result;
        }
    }
}
