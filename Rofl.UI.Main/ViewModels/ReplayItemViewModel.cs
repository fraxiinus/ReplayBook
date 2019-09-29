using Rofl.Files;
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

        public ObservableCollection<ReplayListItemModel> Replays { get; private set; }

        public ReplayItemViewModel(FileManager files)
        {
            _files = files;
        }

        public async Task LoadReplays()
        {
            Replays = new ObservableCollection<ReplayListItemModel>();

            var results = await _files.GetReplayFilesAsync();

            foreach (var file in results)
            {
                Replays.Add(new ReplayListItemModel(file.ReplayFile, file.FileInfo.CreationTime, file.IsNewFile));
            }
        }
    }
}
