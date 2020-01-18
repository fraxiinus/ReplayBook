using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Files.Models
{
    public class ReplayFileInfo
    {
        public string Path { get; set; }

        public DateTime CreationTime { get; set; }

        public long FileSizeBytes { get; set; }
    }
}
