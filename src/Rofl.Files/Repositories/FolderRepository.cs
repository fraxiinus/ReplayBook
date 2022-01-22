using Etirps.RiZhi;
using Rofl.Configuration.Models;
using Rofl.Files.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private readonly ObservableConfiguration _config;
        private readonly RiZhi _log;

        public FolderRepository(ObservableConfiguration config, RiZhi log)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>
        /// Returns full paths of every replay file to show
        /// </summary>
        /// <returns></returns>
        public ReplayFileInfo[] GetAllReplayFileInfo()
        {
            var returnList = new List<ReplayFileInfo>();

            foreach (string folder in _config.ReplayFolders)
            {
                // Grab the contents of the folder
                var dirInfo = new DirectoryInfo(folder);
                var innerFiles = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);

                foreach (var file in innerFiles)
                {
                    // If the file is not supported, skip it
                    if (!(file.Name.EndsWith(".rofl", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    returnList.Add(new ReplayFileInfo()
                    {
                        Path = file.FullName,
                        CreationTime = file.CreationTime,
                        FileSizeBytes = file.Length,
                        Name = Path.GetFileNameWithoutExtension(file.FullName)
                    });
                }
            }

            return returnList.ToArray();
        }

        public ReplayFileInfo GetSingleReplayFileInfo(string path)
        {
            var file = new FileInfo(path);

            // If the file is not supported, skip it
            if (!(file.Name.EndsWith(".rofl", StringComparison.OrdinalIgnoreCase)))
            {
                _log.Warning($"File {path} is not supported, cannot get file info");
                return null;
            }

            return new ReplayFileInfo()
            {
                Path = file.FullName,
                CreationTime = file.CreationTime,
                FileSizeBytes = file.Length,
                Name = Path.GetFileNameWithoutExtension(file.FullName)
            };
        }

        public bool IsPathInSourceFolders(string path)
        {
            return _config.ReplayFolders.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
