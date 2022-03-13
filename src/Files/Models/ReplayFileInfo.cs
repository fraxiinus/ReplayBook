using System;

namespace Fraxiinus.ReplayBook.Files.Models
{
    public class ReplayFileInfo
    {
        public string Path { get; set; }

        public DateTime CreationTime { get; set; }

        public long FileSizeBytes { get; set; }

        public string Name { get; set; }
    }
}
