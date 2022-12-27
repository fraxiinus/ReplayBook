using System.IO;
using System.Threading.Tasks;
using Fraxiinus.ReplayBook.Reader.Models.Internal;

namespace Fraxiinus.ReplayBook.Reader.Parsers
{
    public interface IReplayParser
    {
        Task<IReplayHeader> ReadReplayAsync(FileStream fileStream);
    }
}
