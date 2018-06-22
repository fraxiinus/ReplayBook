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
        public static bool CheckLeagueExecutable()
        {
            var lolpath = RoflSettings.Default.LoLExecLocation;

            // Check the name of the file
            if (string.IsNullOrEmpty(lolpath))
            {
                return false;
            }

            if(!lolpath.Contains("League of Legends.exe"))
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

        public static void DumpJSON(string replaypath)
        {
            if (!File.Exists(replaypath)) { return; }
            using (var filestream = new FileStream(replaypath, FileMode.Open))
            {
                var initialbuffer = new byte[39680];
                // First read 0x9A00 bytes into the array
                filestream.Seek(0x120, SeekOrigin.Begin);
                filestream.Read(initialbuffer, 0, 39424);

                ///* // Read the rest of the file 2 bytes at a time, checking for blank byte buffer after json data
                var bufferoffset = 39424;
                while(initialbuffer[bufferoffset - 1] != 0x0 && initialbuffer[bufferoffset - 2] != 0x0)
                {
                    filestream.Read(initialbuffer, bufferoffset, 2);
                    bufferoffset += 2;
                }//*/

                var outputsize = bufferoffset - 5;

                Array.Resize(ref initialbuffer, outputsize);
                

                using (var outputstream = new FileStream("out.txt", FileMode.Create))
                {
                    outputstream.Write(initialbuffer, 0, outputsize);
                }
            }
        }
    }
}
