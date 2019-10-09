using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Requests;
using Rofl.Requests.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rofl.UI.Main.ViewModels
{
    public class MainWindowViewModel
    {
        private FileManager _fileManager;
        private RequestManager _requestManager;
        private TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        /// <summary>
        /// 
        /// </summary>
        public SortToolModel SortParameters { get; private set; }

        /// <summary>
        /// Smaller, preview objects of replays
        /// </summary>
        public ObservableCollection<ReplayListItemModel> PreviewReplays { get; private set; }

        /// <summary>
        /// Full replay objects
        /// </summary>
        public List<FileResult> FileResults { get; private set; }

        public MainWindowViewModel(FileManager files, RequestManager requests)
        {
            _fileManager = files;
            _requestManager = requests;

            PreviewReplays = new ObservableCollection<ReplayListItemModel>();
            FileResults = new List<FileResult>();

            SortParameters = new SortToolModel
            {
                SearchTerm = String.Empty,
                SortMethod = SortMethod.DateDesc
            };
        }

        public async Task LoadReplays()
        {
            FileResults.AddRange(await _fileManager.GetReplayFilesAsync());

            foreach (var file in FileResults)
            {
                ReplayListItemModel newItem = new ReplayListItemModel(file.ReplayFile, file.FileInfo.CreationTime, file.IsNewFile);

                PreviewReplays.Add(newItem);
            }
        }

        public async Task LoadPreviewPlayerThumbnails()
        {
            foreach (var item in PreviewReplays)
            {
                string dataVersion = await _requestManager.GetDataDragonVersionAsync(item.GameVersion);

                // Image tasks
                List<Task> imageTasks = new List<Task>();

                // Create requests for player images
                foreach (var player in item.BluePreviewPlayers)
                {
                    imageTasks.Add(Task.Run(async () =>
                    {
                        var response = await _requestManager.MakeRequestAsync(new ChampionRequest
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
                        var response = await _requestManager.MakeRequestAsync(new ChampionRequest
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
