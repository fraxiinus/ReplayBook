using System;

namespace Fraxiinus.ReplayBook.Files.Models
{
    public class ReplayErrorInfo
    {
        /// <summary>
        /// Offending file path
        /// </summary>
        public string FilePath { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionString { get; set; }

        public string ExceptionCallStack { get; set; }
        //
        //public Exception Exception { get; set; }
    }
}
