using System;

namespace Rofl.Parsers.Models
{
    public class LprHeader
    {
        public int LprFileVersion { get; set; }

        //public int SpectatorClientVersionLength { get; set; }
        public string SpectatorClientVersion { get; set; }

        public Int64 GameID { get; set; }
        public int GameEndStartupChunk { get; set; }
        public int StartChunk { get; set; }
        public int EndChunk { get; set; }
        public int EndKeyframe { get; set; }
        public int GameLength { get; set; }
        public int GameDelayTime { get; set; }
        public int ClientAddLag { get; set; }
        public int ChunkTimeInterval { get; set; }
        public int KeyframeTimeInterval { get; set; }
        public int ELOLevel { get; set; }
        public int LastChunkTime { get; set; }
        public int LastChunkDuration { get; set; }

        //public int GamePlatformLength { get; set; }
        public string GamePlatform { get; set; }

        //public int SpectatorEncryptionKeyLength { get; set; }
        public string SpectatorEncryptionKey { get; set; }

        //public int CreateTimeLength { get; set; }
        public string CreateTime { get; set; }

        //public int StartTimeLength { get; set; }
        public string StartTime { get; set; }

        //public int EndTimeLength { get; set; }
        public string EndTime { get; set; }

        //public int LeagueVersionLength { get; set; }
        public string LeagueVersion { get; set; }
        

        public LprOldResults OldResults { get; set; }
    }
}
