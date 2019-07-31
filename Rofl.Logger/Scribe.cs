using Rofl.Logger.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rofl.Logger
{
    public class Scribe
    {
        private List<LogEntry> _entryList;
        private bool _errorFlag;

        public string OutputDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");


        public Scribe()
        {
            _entryList = new List<LogEntry>();
            _errorFlag = false;
        }

        ~Scribe()
        {
            WriteToFile();
        }

        public void WriteToFile()
        {
            string outputFileName = Path.Combine(OutputDirectory, $"Error_{ DateTime.Now.ToString("yyyyMMdd_HHmm")}.log");
            if (_errorFlag)
            {
                string logOutput = "";

                foreach (LogEntry entry in _entryList)
                {
                    logOutput += $"{entry.Timestamp}\t|\t{entry.ClassName} -> {entry.MethodName}\t|\t{entry.Level}\t|\t{entry.Message}\n";
                }

                File.WriteAllText(outputFileName, logOutput);
            }
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

        public void Info(string className, string message,
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

            _errorFlag = true;
            _entryList.Add(newEntry);
        }
    }
}
