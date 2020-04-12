using Rofl.Logger.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Rofl.Logger
{
    /// <summary>
    /// Logs messages and errors.
    /// Will dump logs in "logs" folder when an error is logged.
    /// </summary>
    public class Scribe
    {
        private readonly List<LogEntry> _entryList;
        private bool _errorFlag;

        public string OutputDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        public Scribe()
        {
            _entryList = new List<LogEntry>();
            _errorFlag = false;

            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
        }

        public void WriteToFile()
        {
            if (!_errorFlag) { return; } // No errors occured, skip

            string outputFileName = Path.Combine(OutputDirectory, $"Error_{ DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture)}.log");
            string logOutput = "";

            lock (_entryList)
            {
                foreach (LogEntry entry in _entryList)
                {
                    if (entry == null) { continue; }
                    logOutput += $"{entry.Timestamp} | {entry.Level} | {entry.ClassName}.{entry.MethodName} | {entry.Message}\n";
                }
            }

            File.WriteAllText(outputFileName, logOutput);
        }

        public void Debug(string className, string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            LogEntry newEntry = new LogEntry()
            {
                ClassName = className,
                MethodName = memberName,
                Level = LogLevel.DEBUG,
                Message = message,
                Timestamp = DateTime.Now
            };

            _entryList.Add(newEntry);
        }

        public void Information(string className, string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            LogEntry newEntry = new LogEntry()
            {
                ClassName = className,
                MethodName = memberName,
                Level = LogLevel.INFO,
                Message = message,
                Timestamp = DateTime.Now
            };

            _entryList.Add(newEntry);
        }

        public void Warning(string className, string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            LogEntry newEntry = new LogEntry()
            {
                ClassName = className,
                MethodName = memberName,
                Level = LogLevel.WARN,
                Message = message,
                Timestamp = DateTime.Now
            };

            _entryList.Add(newEntry);
        }

        public void Error(string className, string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            LogEntry newEntry = new LogEntry()
            {
                ClassName = className,
                MethodName = memberName,
                Level = LogLevel.ERROR,
                Message = message,
                Timestamp = DateTime.Now
            };

            _errorFlag = true;  // Set the error flag, logs will be created
            _entryList.Add(newEntry);
        }
    }
}
