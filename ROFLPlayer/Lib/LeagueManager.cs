using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ROFLPlayer.Lib
{

    public class LeagueManager
    {

        private static Int32 replayreaduntil = 0x3000;

        public static bool CheckLeagueExecutable()
        {
            var lolpath = RoflSettings.Default.LoLExecLocation;

            // Check the name of the file
            if (string.IsNullOrEmpty(lolpath))
            {
                return false;
            }

            if (!lolpath.Contains("League of Legends.exe"))
            {
                return false;
            }

            // Check file exists
            if (!File.Exists(lolpath))
            {
                return false;
            }

            // Check the description of the file
            if (!string.Equals(FileVersionInfo.GetVersionInfo(lolpath).FileDescription, @"League of Legends (TM) Client"))
            {
                return false;
            }

            return true;
        }

        public static bool CheckLeagueExecutable(string lolpath)
        {

            // Check the name of the file
            if (string.IsNullOrEmpty(lolpath))
            {
                return false;
            }

            if (!lolpath.Contains("League of Legends.exe"))
            {
                return false;
            }

            // Check file exists
            if (!File.Exists(lolpath))
            {
                return false;
            }

            // Check the description of the file
            if (!string.Equals(FileVersionInfo.GetVersionInfo(lolpath).FileDescription, @"League of Legends (TM) Client"))
            {
                return false;
            }

            return true;
        }

        public static bool CheckReplayFile(string replaypath)
        {
            if (!File.Exists(replaypath)) { return false; }
            using (var filestream = new FileStream(replaypath, FileMode.Open))
            {
                var magicbuffer = new byte[4];
                var magicnumber = new byte[] { 0x52, 0x49, 0x4F, 0x54 };    // R I O T
                filestream.Read(magicbuffer, 0, 4);
                if (magicbuffer.SequenceEqual(magicnumber))
                {
                    return true;
                }
            }
            return false;
        }

        public static RunResult<ReplayHeader> LoadAndParseReplayHeaders(string replaypath)
        {
            var result = new RunResult<ReplayHeader> { Success = false, Message = "", Result = null };

            if(!CheckReplayFile(replaypath))
            {
                result.Message = "File is not a valid League of Legends replay.";
                return result;
            }

            ReplayLengthFields replayLengthFields;
            ReplayMatchMetadata replayMatchMetadata;
            ReplayMatchHeader replayMatchHeader;

            using (var filestream = new FileStream(replaypath, FileMode.Open))
            {
                byte[] lengthHeaderBytes = new byte[26];
                try
                {
                    filestream.Seek(262, SeekOrigin.Begin);
                    filestream.Read(lengthHeaderBytes, 0, 26);
                }
                catch(Exception ex)
                {
                    result.Message = "Reading Length Header: " + ex.Message;
                    return result;
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
                    result.Message = "Reading JSON Metadata: " + ex.Message;
                    return result;
                }

                replayMatchMetadata = ParseMetadata(metadataBytes);

                byte[] matchheaderbytes = new byte[replayLengthFields.MatchHeaderLength];
                try
                {
                    filestream.Seek(replayLengthFields.MatchHeaderOffset, SeekOrigin.Begin);
                    filestream.Read(matchheaderbytes, 0, (int)replayLengthFields.MatchHeaderLength);
                }
                catch(Exception ex)
                {
                    result.Message = "Reading Match Header: " + ex.Message;
                    return result;
                }

                replayMatchHeader = ParseMatchHeader(matchheaderbytes);
            }
            result.Success = true;
            result.Message = "Replay Headers Parsed!";
            result.Result = new ReplayHeader { LengthFields = replayLengthFields, MatchMetadata = replayMatchMetadata, MatchHeader = replayMatchHeader};
            return result;
        }
        
        /*
        public static RunResult DumpReplayJSON(string replaypath)
        {
            if (!File.Exists(replaypath)) { return new RunResult { Success = false, Message = "Replay file does not exist." }; }

            var outputpath = Path.Combine(Path.GetDirectoryName(replaypath), Path.GetFileNameWithoutExtension(replaypath) + ".txt");

            using (var filestream = new FileStream(replaypath, FileMode.Open))
            {
                var initialbuffer = new byte[39680];
                // First read 0x9A00 bytes into the array
                filestream.Seek(0x120, SeekOrigin.Begin);
                filestream.Read(initialbuffer, 0, replayreaduntil);

                ///* // Read the rest of the file 2 bytes at a time, checking for blank byte buffer after json data
                var bufferoffset = replayreaduntil;
                var lastbracketpos = 0;
                while (initialbuffer[bufferoffset - 1] != 0x0 && initialbuffer[bufferoffset - 2] != 0x0)
                {
                    filestream.Read(initialbuffer, bufferoffset, 2);
                    if(initialbuffer[bufferoffset] == 0x7D)
                    {
                        lastbracketpos = bufferoffset;
                    }
                    if(initialbuffer[bufferoffset + 1] == 0x7D)
                    {
                        lastbracketpos = bufferoffset + 1;
                    }
                    bufferoffset += 2;
                }//

                var outputsize = lastbracketpos + 1;

                Array.Resize(ref initialbuffer, outputsize);

                using (var outputstream = new FileStream(outputpath, FileMode.Create))
                {
                    outputstream.Write(initialbuffer, 0, outputsize);
                }
            }

            return new RunResult { Success = true, Message = "Replay JSON dumped!" };
        }
        */

        public static string GetReplayJSON(string path)
        {
            if (!File.Exists(path)) { return null; }
            using (var filestream = new FileStream(path, FileMode.Open))
            {
                var initialbuffer = new byte[39680];
                // First read 0x9A00 bytes into the array
                filestream.Seek(0x120, SeekOrigin.Begin);
                filestream.Read(initialbuffer, 0, replayreaduntil);

                ///* // Read the rest of the file 2 bytes at a time, checking for blank byte buffer after json data
                var bufferoffset = replayreaduntil;
                var lastbracketpos = 0;
                while (initialbuffer[bufferoffset - 1] != 0x0 && initialbuffer[bufferoffset - 2] != 0x0)
                {
                    filestream.Read(initialbuffer, bufferoffset, 2);
                    if (initialbuffer[bufferoffset] == 0x7D)
                    {
                        lastbracketpos = bufferoffset;
                    }
                    if (initialbuffer[bufferoffset + 1] == 0x7D)
                    {
                        lastbracketpos = bufferoffset + 1;
                    }
                    bufferoffset += 2;
                }//*/

                var outputsize = lastbracketpos + 1;

                Array.Resize(ref initialbuffer, outputsize);

                return Encoding.UTF8.GetString(initialbuffer);
            }
        }

        public static ReplayMatchHeader ParseMatchHeader(byte[] bytedata)
        {
            var result = new ReplayMatchHeader { };

            byte[] ulong_bytes = bytedata.Take(8).ToArray();
            result.MatchID = BitConverter.ToUInt64(ulong_bytes, 0);

            byte[] uint_bytes = bytedata.Skip(8).Take(4).ToArray();
            result.MatchLength = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(12).Take(4).ToArray();
            result.KeyframeAmount = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(16).Take(4).ToArray();
            result.ChunkAmount = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(20).Take(4).ToArray();
            result.EndChunkID = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(24).Take(4).ToArray();
            result.StartChunkID = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(28).Take(4).ToArray();
            result.KeyframeInterval = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(32).Take(2).ToArray();
            result.EncryptionKeyLength = BitConverter.ToUInt16(uint_bytes, 0);

            uint_bytes = bytedata.Skip(34).Take(result.EncryptionKeyLength).ToArray();
            result.EncryptionKey = Encoding.UTF8.GetString(uint_bytes);

            return result;

        }

        public static ReplayMatchMetadata ParseMetadata(byte[] bytedata)
        {
            var result = new ReplayMatchMetadata { };
            var jsonstring = Encoding.UTF8.GetString(bytedata);

            var jsonobject = JObject.Parse(jsonstring);

            result.GameDuration = (ulong)jsonobject["gameLength"];
            result.GameVersion = (string)jsonobject["gameVersion"];
            result.LastGameChunkID = (uint)jsonobject["lastGameChunkId"];
            result.LastKeyframeID = (uint)jsonobject["lastKeyFrameId"];
            
            result.PlayerStatsObject = JArray.Parse(((string)jsonobject["statsJson"]).Replace(@"\", ""));

            return result;
        }

        private static ReplayLengthFields ParseLengthFields(byte[] bytedata)
        {
            var result = new ReplayLengthFields { };

            byte[] ushort_bytes = bytedata.Take(2).ToArray();
            result.HeaderLength = BitConverter.ToUInt16(ushort_bytes, 0);

            byte[] uint_bytes = bytedata.Skip(2).Take(4).ToArray();
            result.FileLength = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(6).Take(4).ToArray();
            result.MetadataOffset = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(10).Take(4).ToArray();
            result.MetadataLength = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(14).Take(4).ToArray();
            result.MatchHeaderOffset = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(18).Take(4).ToArray();
            result.MatchHeaderLength = BitConverter.ToUInt32(uint_bytes, 0);

            uint_bytes = bytedata.Skip(22).Take(4).ToArray();
            result.MatchOffset = BitConverter.ToUInt32(uint_bytes, 0);

            return result;
        }
    }
}
