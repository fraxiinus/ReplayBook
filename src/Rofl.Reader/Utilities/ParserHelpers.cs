using Rofl.Reader.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rofl.Reader.Utilities
{
    public static class ParserHelpers
    {
        private static readonly byte[] ROFLMagic = { 0x52, 0x49, 0x4F, 0x54 };
        private static readonly byte[] LPRMagic = { 0x04, 0x00, 0x00, 0x00 };
        private static readonly byte[] LRFMagic = { 0x00, 0x0B, 0x01, 0x00 };

        public static async Task<ReplayType> GetReplayTypeAsync(string file)
        {
            using (FileStream fileStream = new FileStream(file, FileMode.Open))
            {
                return await GetReplayTypeAsync(fileStream);
            }
        }

        public static async Task<ReplayType> GetReplayTypeAsync(FileStream fileStream)
        {
            byte[] magicBuffer = await ReadBytesAsync(fileStream, 4);

            if (magicBuffer.SequenceEqual(ROFLMagic))
            {
                return ReplayType.ROFL;
            }
            else if (magicBuffer.SequenceEqual(LRFMagic))
            {
                return ReplayType.LRF;
            }
            else if (magicBuffer.SequenceEqual(LPRMagic))
            {
                return ReplayType.LPR;
            }
            else
            {
                return ReplayType.NONE;
            }
        }

        public static async Task<byte[]> ReadBytesAsync(FileStream fileStream, int count)
        {
            byte[] buffer = new byte[count];

            await fileStream.ReadAsync(buffer, 0, count);

            return buffer;
        }

        public static async Task<byte[]> ReadBytesAsync(FileStream fileStream, int count, int startOffset, SeekOrigin origin)
        {
            byte[] buffer = new byte[count];

            fileStream.Seek(startOffset, origin);
            await fileStream.ReadAsync(buffer, 0, count);

            return buffer;
        }
    }
}
