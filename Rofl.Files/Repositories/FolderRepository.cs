using Microsoft.Extensions.Configuration;
using Rofl.Files.Models;
using Rofl.Logger;
using Rofl.Reader;
using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rofl.Files.Repositories
{
    /// <summary>
    /// Watches given folders defined in config file for new replay files
    /// Once a new file is detected, parse the data and place it in the database
    /// (depends on if database supports on update)
    /// ViewModel asks the database(wrapper???) for the replay data
    /// </summary>
    public class FolderRepository
    {

        private IConfiguration _config;

        private Scribe _log;

        private List<string> _folderPaths;

        public FolderRepository(IConfiguration config, Scribe log)
        {
            _config = config;

            _log = log;

            _folderPaths = _config.GetSection("folder-watcher:folders").Get<List<string>>();
        }

        /// <summary>
        /// Returns full paths of every replay file to show
        /// </summary>
        /// <returns></returns>
        public ReplayFileInfo[] GetAllReplayFileInfo()
        {
            List<ReplayFileInfo> returnList = new List<ReplayFileInfo>();

            foreach (string folder in _folderPaths)
            {
                // Grab the contents of the folder
                DirectoryInfo dirInfo = new DirectoryInfo(folder);
                var innerFiles = dirInfo.EnumerateFiles();

                foreach (var file in innerFiles)
                {
                    // If the file is not supported, skip it
                    if (!(file.Name.EndsWith(".rofl", StringComparison.OrdinalIgnoreCase) ||
                          file.Name.EndsWith(".lrf", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    returnList.Add(new ReplayFileInfo()
                    {
                        Path = file.FullName,
                        CreationTime = file.CreationTime
                    });
                }
            }

            return returnList.ToArray();
        }
    }
}
