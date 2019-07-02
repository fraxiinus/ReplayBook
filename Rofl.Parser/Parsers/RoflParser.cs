using System.IO;
using System.Threading.Tasks;
using Rofl.Parsers.Parsers;
using Rofl.Parsers.Models;
using System.Linq;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Rofl.Parsers.Parsers
{
    /// <summary>
    /// Parses Official League of Legends Replays
    /// </summary>
    public class RoflParser : IReplayParser
    {
        private readonly string exceptionOriginName = "RoflParser";
        private readonly byte[] _magicNumbers = new byte[] { 0x52, 0x49, 0x4F, 0x54 };

        public async Task<ReplayHeader> ReadReplayAsync(FileStream fileStream)
        {
            if(!fileStream.CanRead)
            {
                throw new IOException($"{exceptionOriginName} - Stream does not support reading");
            }

            // Read and check Magic Numbers
            byte[] magicbuffer = new byte[4];
            try
            {
                await fileStream.ReadAsync(magicbuffer, 0, 4);
                if (!magicbuffer.SequenceEqual(_magicNumbers))
                {
                    throw new Exception($"{exceptionOriginName} - Selected file is not in valid ROFL format");
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading Magic Number: " + ex.Message);
            }


            // Read and deserialize length fields
            byte[] lengthFieldBuffer = new byte[26];
            try
            {
                fileStream.Seek(262, SeekOrigin.Begin);
                await fileStream.ReadAsync(lengthFieldBuffer, 0, 26);
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading Length Header: " + ex.Message);
            }

            var replayLengthFields = ParseLengthFields(lengthFieldBuffer);


            // Read and deserialize metadata
            byte[] metadataBuffer = new byte[replayLengthFields.MetadataLength];
            try
            {
                fileStream.Seek(replayLengthFields.MetadataOffset, SeekOrigin.Begin);
                await fileStream.ReadAsync(metadataBuffer, 0, (int)replayLengthFields.MetadataLength);
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading JSON Metadata: " + ex.Message);
            }

            var replayMatchMetadata = ParseMetadata(metadataBuffer);


            // Read and deserialize payload fields
            byte[] payloadFieldBuffer = new byte[replayLengthFields.PayloadHeaderLength];
            try
            {
                fileStream.Seek(replayLengthFields.PayloadHeaderOffset, SeekOrigin.Begin);
                await fileStream.ReadAsync(payloadFieldBuffer, 0, (int)replayLengthFields.PayloadHeaderLength);
            }
            catch (Exception ex)
            {
                throw new IOException($"{exceptionOriginName} - Reading Match Header: " + ex.Message);
            }

            var replayPayloadFields = ParseMatchHeader(payloadFieldBuffer);

            return new ReplayHeader
            {
                LengthFields = replayLengthFields,
                MatchMetadata = replayMatchMetadata,
                PayloadFields = replayPayloadFields
            };
        }

        private static PayloadFields ParseMatchHeader(byte[] bytedata)
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

            // Sort blue and red teams
            foreach (JObject player in JArray.Parse(((string)jsonobject["statsJson"]).Replace(@"\", "")))
            {
                if(player["TEAM"].ToString() == "100")
                {
                    blueTeam.Add(player.ToObject<Dictionary<string, string>>());
                }
                else if (player["TEAM"].ToString() == "200")
                {
                    redTeam.Add(player.ToObject<Dictionary<string, string>>());
                }
            }

            result.BlueTeam = blueTeam.ToArray();
            result.RedTeam = redTeam.ToArray();

            //result.Players = JArray.Parse(((string)jsonobject["statsJson"]).Replace(@"\", ""));

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
