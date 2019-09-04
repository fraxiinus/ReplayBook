using Microsoft.Extensions.Configuration;
using Rofl.Reader;
using Rofl.Reader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rofl.Files
{
    /// <summary>
    /// Watches given folders defined in config file for new replay files
    /// Once a new file is detected, parse the data and place it in the database
    /// (depends on if database supports on update)
    /// ViewModel asks the database(wrapper???) for the replay data
    /// </summary>
    public class FolderWatcher
    {

        private IConfiguration _config;

        private List<string> folderPaths;

        public FolderWatcher(IConfiguration config)
        {
            _config = config;

            folderPaths = _config.GetSection("folder-watcher:folders").Get<List<string>>();
        }

        /// <summary>
        /// Returns array of unparsed ReplayFiles
        /// </summary>
        /// <returns></returns>
        public ReplayFile[] GetReplayFiles()
        {
            List<ReplayFile> replayFiles = new List<ReplayFile>();

            foreach (string path in folderPaths)
            {
                // Grab the contents of the folder, sorted newest first
                DirectoryInfo DirInfo = new DirectoryInfo(path);
                var files = DirInfo.EnumerateFiles().OrderByDescending(f => f.CreationTime);

                foreach (var file in files)
                {
                    // If the file is not supported, skip it
                    if (!(file.Name.EndsWith(".rofl", StringComparison.OrdinalIgnoreCase) ||
                          file.Name.EndsWith(".lrf", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    var newReplay = new ReplayFile()
                    {
                        Location = file.FullName,
                        Name = Path.GetFileNameWithoutExtension(file.Name),
                    };

                    replayFiles.Add(newReplay);
                }
            }

            return replayFiles.ToArray();
        }
    }
}
