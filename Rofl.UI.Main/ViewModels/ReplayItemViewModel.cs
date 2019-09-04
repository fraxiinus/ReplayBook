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

        private FolderWatcher _folderWatcher;
        private ReplayReader _replayReader;
        private TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        public ObservableCollection<ReplayItemModel> Replays { get; private set; }

        public ReplayItemViewModel(FolderWatcher folderWatcher, ReplayReader replayReader)
        {
            _folderWatcher = folderWatcher;
            _replayReader = replayReader;
        }

        public void LoadReplays()
        {
            // Kick off background task which loads files
            Task<ReplayFile[]> loadReplayTask = Task.Run(async () =>
            {
                var replayFiles = _folderWatcher.GetReplayFiles();

                var readTasks = (from file in replayFiles
                                 select _replayReader.ReadFile(file));

                await Task.Delay(5000);

                return await Task.WhenAll(readTasks);
            });

            Replays = new ObservableCollection<ReplayItemModel>();

            // When the background task finishes, add them all
            loadReplayTask.ContinueWith(x =>
            {
                foreach (var file in x.Result)
                {
                    Replays.Add(new ReplayItemModel(file));
                }
            }, _uiScheduler);
        }
    }
}
