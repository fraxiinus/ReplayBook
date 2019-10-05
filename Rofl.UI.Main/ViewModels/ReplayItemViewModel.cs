using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Reader;
using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.ViewModels
{
    public class ReplayItemViewModel
    {

        private FileManager _files;
        private TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        /// <summary>
        /// Smaller, preview objects of replays
        /// </summary>
        public ObservableCollection<ReplayListItemModel> PreviewReplays { get; private set; }

        /// <summary>
        /// Full replay objects
        /// </summary>
        public List<FileResult> FileResults { get; private set; }

        public ReplayItemViewModel(FileManager files)
        {
            _files = files;
            PreviewReplays = new ObservableCollection<ReplayListItemModel>();
            FileResults = new List<FileResult>();
        }

        public async Task InitialLoadReplays()
        {
            FileResults.AddRange(await _files.GetReplayFilesAsync());

            foreach (var file in FileResults)
            {
                PreviewReplays.Add(new ReplayListItemModel(file.ReplayFile, file.FileInfo.CreationTime, file.IsNewFile));
            }
        }
    }
}
