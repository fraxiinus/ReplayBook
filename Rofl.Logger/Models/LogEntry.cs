using System;

namespace Rofl.Logger.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }
    }
}
