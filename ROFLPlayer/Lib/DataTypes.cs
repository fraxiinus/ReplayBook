using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROFLPlayer.Lib
{
    public struct RunResult
    {
        public bool Success;
        public string Message;
    }

    public struct PlayerInfo
    {
        public string Champion;
        public string Name;
        public string Team;
        public string Win;
    }

    public struct FileBaseData
    {
        public long GameLength;
        public string GameVersion;
        public PlayerInfo[] BluePlayers;
        public PlayerInfo[] PurplePlayers;
        public string WonGame;
    }

}
