using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;


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
                }//*/

                var outputsize = lastbracketpos + 1;

                Array.Resize(ref initialbuffer, outputsize);

                using (var outputstream = new FileStream(outputpath, FileMode.Create))
                {
                    outputstream.Write(initialbuffer, 0, outputsize);
                }
            }

            return new RunResult { Success = true, Message = "Replay JSON dumped!" };
        }

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
    }
}
