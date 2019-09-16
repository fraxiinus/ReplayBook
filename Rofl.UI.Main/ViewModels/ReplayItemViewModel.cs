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

        public void LoadReplays()
        {
            Replays = new ObservableCollection<ReplayListItemModel>();

            var loadReplaysTask = _files.GetReplayFiles();

            // When the background task finishes, add them all
            loadReplaysTask.ContinueWith(x =>
            {
                foreach (var file in x.Result)
                {
                    Replays.Add(new ReplayListItemModel(file.ReplayFile, file.FileInfo.CreationTime, file.IsNewFile));
                }
            }, _uiScheduler);
        }
    }
}
