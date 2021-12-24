using System.IO;
using System.Threading.Tasks;
using Rofl.Reader.Models.Internal;

namespace Rofl.Reader.Parsers
{
    public interface IReplayParser
    {
        Task<IReplayHeader> ReadReplayAsync(FileStream fileStream);
    }
}
