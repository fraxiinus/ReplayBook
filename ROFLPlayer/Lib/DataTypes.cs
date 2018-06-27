using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ROFLPlayer.Lib
{

    public enum Maps
    {
        TwistedTreeline = 10, SummonersRift = 11, HowlingAbyss = 12
    }

    public struct RunResult<T>
    {
        public bool Success;
        public string Message;
        public T Result;
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
