using Microsoft.Extensions.Configuration;
using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Requests;
using Rofl.Requests.Models;
using Rofl.UI.Main.Extensions;
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

        /// <summary>
        /// 
        /// </summary>
        public SortToolModel SortParameters { get; private set; }

        /// <summary>
        /// Smaller, preview objects of replays
        /// </summary>
        public ObservableCollection<ReplayPreviewModel> PreviewReplays { get; private set; }

        /// <summary>
        /// Full replay objects
        /// </summary>
        public List<FileResult> FileResults { get; private set; }

        public List<string> KnownPlayers { get; private set; }

        public MainWindowViewModel(FileManager files, RequestManager requests, IConfiguration config)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

            _fileManager = files ?? throw new ArgumentNullException(nameof(files));
            _requestManager = requests ?? throw new ArgumentNullException(nameof(requests));

            KnownPlayers = config.GetSection("user_settings:known_players").Get<List<string>>();

            PreviewReplays = new ObservableCollection<ReplayPreviewModel>();
            FileResults = new List<FileResult>();

            SortParameters = new SortToolModel
            {
                SearchTerm = String.Empty,
                SortMethod = SortMethod.DateDesc
            };
        }

        public async Task LoadReplays()
        {
            FileResults.AddRange(await _fileManager.GetReplayFilesAsync().ConfigureAwait(false));

            foreach (var file in FileResults)
            {
                ReplayPreviewModel newItem = new ReplayPreviewModel(file.ReplayFile, file.FileInfo.CreationTime, file.IsNewFile);

                foreach (var bluePlayer in newItem.BluePreviewPlayers)
                {
                    bluePlayer.IsKnownPlayer = KnownPlayers.Contains(bluePlayer.PlayerName, StringComparer.OrdinalIgnoreCase);
                }

                foreach (var redPlayer in newItem.RedPreviewPlayers)
                {
                    redPlayer.IsKnownPlayer = KnownPlayers.Contains(redPlayer.PlayerName, StringComparer.OrdinalIgnoreCase);
                }

                PreviewReplays.Add(newItem);
            }
        }

        public async Task LoadPreviewPlayerThumbnails()
        {
            foreach (var item in PreviewReplays)
            {
                string dataVersion = await _requestManager.GetDataDragonVersionAsync(item.GameVersion).ConfigureAwait(false);

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
                        }).ConfigureAwait(false);
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
                        }).ConfigureAwait(false);
                        player.ImageSource = response.ResponsePath;
                    }));
                }

                // Wait for all images to finish before doing the next replay
                await Task.WhenAll(imageTasks).ConfigureAwait(false);
            }
        }

        public void SortPreviewReplays(CollectionViewSource replayView)
        {
            if(replayView == null) { throw new ArgumentNullException(nameof(replayView)); }

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

        public bool FilterPreviewReplay(ReplayPreviewModel replayItem)
        {
            if(replayItem == null) { throw new ArgumentNullException(nameof(replayItem)); }

            string searchTerm = SortParameters.SearchTerm;

            // Minimum requirement
            if(searchTerm.Length < 3) { return true; }

            if (replayItem.MapName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) { return true; }

            foreach (var bluePlayer in replayItem.BluePreviewPlayers)
            {
                if (bluePlayer.ChampionName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) { return true; }
                if (bluePlayer.PlayerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) { return true; }
            }

            foreach (var redPlayer in replayItem.BluePreviewPlayers)
            {
                if (redPlayer.ChampionName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) { return true; }
                if (redPlayer.PlayerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) { return true; }
            }

            return false;
        }
    }
}
