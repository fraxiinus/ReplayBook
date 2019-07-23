using System;
using System.IO;
using System.Threading.Tasks;
using Rofl.Parsers.Models;

namespace Rofl.Parsers.Parsers
{
    public interface IReplayParser
    {
        Task<ReplayHeader> ReadReplayAsync(FileStream fileStream);
    }
}
