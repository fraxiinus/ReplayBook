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

        public static ReplayHeader ReadReplayFile(string replaypath)
        {

            if (!File.Exists(replaypath)) { throw new FileNotFoundException("Selected file not found"); }

            ReplayLengthFields replayLengthFields;
            ReplayMatchMetadata replayMatchMetadata;
            ReplayPayloadHeader replayPayloadHeader;

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
                byte[] metadataBytes = new byte[replayLengthFields.MetadataLength];

                try
                {
                    filestream.Seek(replayLengthFields.MetadataOffset, SeekOrigin.Begin);
                    filestream.Read(metadataBytes, 0, (int)replayLengthFields.MetadataLength);
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
                    filestream.Read(matchheaderbytes, 0, (int)replayLengthFields.PayloadHeaderLength);
                }
                catch (Exception ex)
                {
                    throw new IOException("Reading Match Header: " + ex.Message);
                }

                replayPayloadHeader = ParseMatchHeader(matchheaderbytes);
            }

            return new ReplayHeader { LengthFields = replayLengthFields, MatchMetadata = replayMatchMetadata, MatchHeader = replayPayloadHeader };
        }

        public async static Task<ReplayHeader> ReadReplayFileAsync(string replaypath)
        {

            if (!File.Exists(replaypath)) { throw new FileNotFoundException("Selected file not found"); }

            ReplayLengthFields replayLengthFields;
            ReplayMatchMetadata replayMatchMetadata;
            ReplayPayloadHeader replayPayloadHeader;

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

        private static ReplayPayloadHeader ParseMatchHeader(byte[] bytedata)
        {
            var result = new ReplayPayloadHeader { };

            //byte[] ulong_bytes = bytedata.Take(8).ToArray();
            result.MatchID = BitConverter.ToUInt64(bytedata, 0);

            //byte[] uint_bytes = bytedata.Skip(8).Take(4).ToArray();
            result.MatchLength = BitConverter.ToUInt32(bytedata, 8);

            //uint_bytes = bytedata.Skip(12).Take(4).ToArray();
            result.KeyframeAmount = BitConverter.ToUInt32(bytedata, 12);

            //uint_bytes = bytedata.Skip(16).Take(4).ToArray();
            result.ChunkAmount = BitConverter.ToUInt32(bytedata, 16);

            //uint_bytes = bytedata.Skip(20).Take(4).ToArray();
            result.EndChunkID = BitConverter.ToUInt32(bytedata, 20);

            //uint_bytes = bytedata.Skip(24).Take(4).ToArray();
            result.StartChunkID = BitConverter.ToUInt32(bytedata, 24);

            //uint_bytes = bytedata.Skip(28).Take(4).ToArray();
            result.KeyframeInterval = BitConverter.ToUInt32(bytedata, 28);

            //uint_bytes = bytedata.Skip(32).Take(2).ToArray();
            result.EncryptionKeyLength = BitConverter.ToUInt16(bytedata, 32);

            //byte[] uint_bytes = bytedata.Skip(34).Take(result.EncryptionKeyLength).ToArray();
            result.EncryptionKey = Encoding.UTF8.GetString(bytedata, 34, result.EncryptionKeyLength);

            return result;

        }

        private static ReplayMatchMetadata ParseMetadata(byte[] bytedata)
        {
            var result = new ReplayMatchMetadata { };
            var jsonstring = Encoding.UTF8.GetString(bytedata);

            var jsonobject = JObject.Parse(jsonstring);

            result.GameDuration = (ulong)jsonobject["gameLength"];
            result.GameVersion = (string)jsonobject["gameVersion"];
            result.LastGameChunkID = (uint)jsonobject["lastGameChunkId"];
            result.LastKeyframeID = (uint)jsonobject["lastKeyFrameId"];

            result.Players = JArray.Parse(((string)jsonobject["statsJson"]).Replace(@"\", ""));

            return result;
        }

        private static ReplayLengthFields ParseLengthFields(byte[] bytedata)
        {
            var result = new ReplayLengthFields { };

            //byte[] ushort_bytes = bytedata.Take(2).ToArray();
            result.HeaderLength = BitConverter.ToUInt16(bytedata, 0);

            //byte[] uint_bytes = bytedata.Skip(2).Take(4).ToArray();
            result.FileLength = BitConverter.ToUInt32(bytedata, 2);

            //uint_bytes = bytedata.Skip(6).Take(4).ToArray();
            result.MetadataOffset = BitConverter.ToUInt32(bytedata, 6);

            //uint_bytes = bytedata.Skip(10).Take(4).ToArray();
            result.MetadataLength = BitConverter.ToUInt32(bytedata, 10);

            //uint_bytes = bytedata.Skip(14).Take(4).ToArray();
            result.PayloadHeaderOffset = BitConverter.ToUInt32(bytedata, 14);

            //uint_bytes = bytedata.Skip(18).Take(4).ToArray();
            result.PayloadHeaderLength = BitConverter.ToUInt32(bytedata, 18);

            //uint_bytes = bytedata.Skip(22).Take(4).ToArray();
            result.PayloadOffset = BitConverter.ToUInt32(bytedata, 22);

            return result;
        }
    }
}
