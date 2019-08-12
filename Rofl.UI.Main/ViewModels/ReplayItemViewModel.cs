using Rofl.Files;
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

        private FolderWatcher _folderWatcher;

        public ObservableCollection<ReplayItemModel> Replays { get; private set; }

        public ReplayItemViewModel(FolderWatcher folderWatcher)
        {
            _folderWatcher = folderWatcher;
        }

        public void LoadReplays()
        {
            var data = Task.Run(() => _folderWatcher.GetReplayFiles());

            data.Wait();

            var length = data.Result[0].Data.MatchMetadata.GameDuration;

            Replays = new ObservableCollection<ReplayItemModel>(from replayFile in data.Result
                                                                select new ReplayItemModel(replayFile));

        }
    }
}
