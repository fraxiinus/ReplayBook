using System;

namespace Fraxiinus.ReplayBook.Files.Models
{
    public class ReplayErrorInfo
    {
        /// <summary>
        /// Offending file path
        /// </summary>
        public string FilePath { get; set; }

        public Exception Exception { get; set; }
    }
}
