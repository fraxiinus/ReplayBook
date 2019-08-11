using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Rofl.Files
{
    public class FileManager
    {

        private IConfiguration _config;

        private List<string> folders;

        public FileManager(IConfiguration config)
        {
            _config = config;

            folders = _config.GetSection("file-manager:folders").Get<List<string>>();
        }
    }
}
