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
    public class FolderWatcher
    {

        private IConfiguration _config;
        private ReplayReader _replayReader;

        private List<string> folderPaths;

        public FolderWatcher(IConfiguration config, ReplayReader replayReader)
        {
            _config = config;
            _replayReader = replayReader;

            folderPaths = _config.GetSection("folder-watcher:folders").Get<List<string>>();
        }

        public async Task<ReplayFile[]> GetReplayFiles()
        {
            List<ReplayFile> replayFiles = new List<ReplayFile>();

            foreach (string path in folderPaths)
            {

                DirectoryInfo DirInfo = new DirectoryInfo(path);
                var files = DirInfo.EnumerateFiles().OrderByDescending(f => f.CreationTime);

                foreach (var file in files)
                {
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

                    newReplay = await _replayReader.ReadFile(newReplay);

                    replayFiles.Add(newReplay);
                }

                //var matchingFiles = Directory
                //    .EnumerateFiles(path)
                //    .Where(file => file.EndsWith(".rofl", StringComparison.OrdinalIgnoreCase) || 
                //                   file.EndsWith(".lrf", StringComparison.OrdinalIgnoreCase))
                //    .Select(file => new ReplayFile()
                //    {
                //        Location = file,
                //        Name = file,
                //    })
                //    .ToList();

                //replayFiles.AddRange(matchingFiles);
            }

            //replayFiles.ForEach(async x => x = await _replayReader.ReadFile(x));

            return replayFiles.ToArray();
        }
    }
}
