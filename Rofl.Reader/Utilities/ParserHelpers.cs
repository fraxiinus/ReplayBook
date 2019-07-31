using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.Reader.Utilities
{
    public static class ParserHelpers
    {
        public static async Task ReadBytes(string logLocation, FileStream fileStream, byte[] buffer, int count)
        {
            try
            {
                await fileStream.ReadAsync(buffer, 0, count);
            }
            catch (Exception ex)
            {
                throw new IOException(logLocation + " - " + ex.Message);
            }
        }

        public static async Task ReadBytes(string logLocation, FileStream fileStream, byte[] buffer, int count, int startOffset)
        {
            try
            {
                fileStream.Seek(startOffset, SeekOrigin.Current);
                await fileStream.ReadAsync(buffer, 0, count);
            }
            catch (Exception ex)
            {
                throw new IOException(logLocation + " - " + ex.Message);
            }
        }
    }
}
