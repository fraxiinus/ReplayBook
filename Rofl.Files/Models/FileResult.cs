using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Files.Models
{
    public class FileResult
    {
        public bool IsNewFile { get; set; }

        public ReplayFileInfo FileInfo { get; set; }

        public ReplayFile ReplayFile { get; set; }
    }
}
