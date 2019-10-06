using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Reader;
using Rofl.Reader.Models;
using Rofl.Requests;
using Rofl.Requests.Models;
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
        private RequestManager _requests;
        private TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        /// <summary>
        /// Smaller, preview objects of replays
        /// </summary>
        public ObservableCollection<ReplayListItemModel> PreviewReplays { get; private set; }

        /// <summary>
        /// Full replay objects
        /// </summary>
        public List<FileResult> FileResults { get; private set; }

        public ReplayItemViewModel(FileManager files, RequestManager requests)
        {
            _files = files;
            _requests = requests;

            PreviewReplays = new ObservableCollection<ReplayListItemModel>();
            FileResults = new List<FileResult>();
        }

        public async Task InitialLoadReplays()
        {
            FileResults.AddRange(await _files.GetReplayFilesAsync());

            foreach (var file in FileResults)
            {
                ReplayListItemModel newItem = new ReplayListItemModel(file.ReplayFile, file.FileInfo.CreationTime, file.IsNewFile);

                PreviewReplays.Add(newItem);
            }
        }

        public async Task LoadThumbnails()
        {
            foreach (var item in PreviewReplays)
            {
                string dataVersion = await _requests.GetDataDragonVersionAsync(item.GameVersion);

                // Image tasks
                List<Task> imageTasks = new List<Task>();

                // Create requests for player images
                foreach (var player in item.BluePreviewPlayers)
                {
                    imageTasks.Add(Task.Run(async () =>
                    {
                        var response = await _requests.MakeRequestAsync(new ChampionRequest
                        {
                            DataDragonVersion = dataVersion,
                            ChampionName = player.ChampionName
                        });
                        player.ImageSource = response.ResponsePath;
                    }));
                }

                foreach (var player in item.RedPreviewPlayers)
                {
                    imageTasks.Add(Task.Run(async () =>
                    {
                        var response = await _requests.MakeRequestAsync(new ChampionRequest
                        {
                            DataDragonVersion = dataVersion,
                            ChampionName = player.ChampionName
                        });
                        player.ImageSource = response.ResponsePath;
                    }));
                }

                // Wait for all images to finish before doing the next replay
                await Task.WhenAll(imageTasks);
            }
        }
    }
}
