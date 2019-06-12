using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Rofl.Parser
{

    public class ReplayReader
    {
        public static byte[] MagicBytes = new byte[] { 0x52, 0x49, 0x4F, 0x54 };    // R I O T

        /// <summary>
        /// Parse a given replay file into a ReplayHeader
        /// </summary>
        /// <param name="replaypath"></param>
        /// <returns></returns>
        public static ReplayHeader ReadReplayFile(string replaypath)
        {

            if (!File.Exists(replaypath)) { throw new FileNotFoundException("Selected file not found"); }

            LengthFields replayLengthFields;
            MatchMetadata replayMatchMetadata;
            PayloadFields replayPayloadHeader;

            using (var filestream = new FileStream(replaypath, FileMode.Open))
            {

                byte[] magicbuffer = new byte[4];
                try
                {
                    filestream.Read(magicbuffer, 0, 4);
                    if(!magicbuffer.SequenceEqual(MagicBytes))
                    {
                        throw new Exception("Selected file is not in valid replay format");
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading Magic Number: " + ex.Message);
                }

                byte[] lengthHeaderBytes = new byte[26];
                try
                {
                    filestream.Seek(262, SeekOrigin.Begin);
                    filestream.Read(lengthHeaderBytes, 0, 26);
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading Length Header: " + ex.Message);
                }
                replayLengthFields = ParseLengthFields(lengthHeaderBytes);

                var metadataPayloadLength = replayLengthFields.MetadataLength + replayLengthFields.PayloadHeaderLength;
                byte[] replayMetadataPayloadBytes = new byte[metadataPayloadLength];

                try
                {
                    filestream.Seek(replayLengthFields.MetadataOffset, SeekOrigin.Begin);
                    filestream.Read(replayMetadataPayloadBytes, 0, (int)metadataPayloadLength);
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading Metadata + Payload Header: " + ex.Message);
                }

                replayMatchMetadata = ParseMetadata(replayMetadataPayloadBytes.Take((int)replayLengthFields.MetadataLength).ToArray());
                replayPayloadHeader = ParseMatchHeader(replayMetadataPayloadBytes.Skip((int)replayLengthFields.PayloadHeaderOffset).Take((int)replayLengthFields.PayloadHeaderLength).ToArray());

            }

            return new ReplayHeader { LengthFields = replayLengthFields, MatchMetadata = replayMatchMetadata, MatchHeader = replayPayloadHeader };
        }

        /// <summary>
        /// Parse a given replay file into a ReplayHeader
        /// </summary>
        /// <param name="replaypath"></param>
        /// <returns></returns>
        public async static Task<ReplayHeader> ReadReplayFileAsync(string replaypath)
        {

            if (!File.Exists(replaypath)) { throw new FileNotFoundException("Selected file not found"); }

            LengthFields replayLengthFields;
            MatchMetadata replayMatchMetadata;
            PayloadFields replayPayloadHeader;

            using (var filestream = new FileStream(replaypath, FileMode.Open))
            {

                byte[] magicbuffer = new byte[4];
                try
                {
                    await filestream.ReadAsync(magicbuffer, 0, 4);
                    if (!magicbuffer.SequenceEqual(MagicBytes))
                    {
                        throw new Exception("Selected file is not in valid replay format");
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading Magic Number: " + ex.Message);
                }

                byte[] lengthHeaderBytes = new byte[26];
                try
                {
                    filestream.Seek(262, SeekOrigin.Begin);
                    await filestream.ReadAsync(lengthHeaderBytes, 0, 26);
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading Length Header: " + ex.Message);
                }

                replayLengthFields = ParseLengthFields(lengthHeaderBytes);
                byte[] metadataBytes = new byte[replayLengthFields.MetadataLength];

                try
                {
                    filestream.Seek(replayLengthFields.MetadataOffset, SeekOrigin.Begin);
                    await filestream.ReadAsync(metadataBytes, 0, (int)replayLengthFields.MetadataLength);
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading JSON Metadata: " + ex.Message);
                }

                replayMatchMetadata = ParseMetadata(metadataBytes);

                byte[] matchheaderbytes = new byte[replayLengthFields.PayloadHeaderLength];
                try
                {
                    filestream.Seek(replayLengthFields.PayloadHeaderOffset, SeekOrigin.Begin);
                    await filestream.ReadAsync(matchheaderbytes, 0, (int)replayLengthFields.PayloadHeaderLength);
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading Match Header: " + ex.Message);
                }

                replayPayloadHeader = ParseMatchHeader(matchheaderbytes);
            }

            return new ReplayHeader { LengthFields = replayLengthFields, MatchMetadata = replayMatchMetadata, MatchHeader = replayPayloadHeader };
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

            result.Players = JArray.Parse(((string)jsonobject["statsJson"]).Replace(@"\", ""));

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
