using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Requests;
using Rofl.Requests.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

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

        public void SortPreviewReplays(CollectionViewSource replayView)
        {
            SortMethod sort = SortParameters.SortMethod;

            switch (sort)
            {
                case SortMethod.NameAsc:
                    replayView.SortDescriptions.Clear();
                    replayView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    break;

                case SortMethod.NameDesc:
                    replayView.SortDescriptions.Clear();
                    replayView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
                    break;

                case SortMethod.DateAsc:
                    replayView.SortDescriptions.Clear();
                    replayView.SortDescriptions.Add(new SortDescription("CreationDate", ListSortDirection.Ascending));
                    break;

                case SortMethod.DateDesc:
                    replayView.SortDescriptions.Clear();
                    replayView.SortDescriptions.Add(new SortDescription("CreationDate", ListSortDirection.Descending));
                    break;

                case SortMethod.LengthAsc:
                    replayView.SortDescriptions.Clear();
                    replayView.SortDescriptions.Add(new SortDescription("GameDuration", ListSortDirection.Ascending));
                    break;

                case SortMethod.LengthDesc:
                    replayView.SortDescriptions.Clear();
                    replayView.SortDescriptions.Add(new SortDescription("GameDuration", ListSortDirection.Descending));
                    break;

                default:
                    break;
            }
        }

        public bool FilterPreviewReplay(ReplayListItemModel replayItem)
        {
            string searchTerm = SortParameters.SearchTerm.ToUpper();

            // Minimum requirement
            if(searchTerm.Length < 3) { return true; }

            if (replayItem.MapName.ToUpper().Contains(searchTerm)) { return true; }

            foreach (var bluePlayer in replayItem.BluePreviewPlayers)
            {
                if (bluePlayer.ChampionName.ToUpper().Contains(searchTerm)) { return true; }
                if (bluePlayer.PlayerName.ToUpper().Contains(searchTerm)) { return true; }
            }

            foreach (var redPlayer in replayItem.BluePreviewPlayers)
            {
                if (redPlayer.ChampionName.ToUpper().Contains(searchTerm)) { return true; }
                if (redPlayer.PlayerName.ToUpper().Contains(searchTerm)) { return true; }
            }

            return false;
        }
    }
}
