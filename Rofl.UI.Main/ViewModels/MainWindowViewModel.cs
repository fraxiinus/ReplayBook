using Etirps.RiZhi;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Reader.Models;
using Rofl.Requests;
using Rofl.Requests.Models;
using Rofl.Settings;
using Rofl.Settings.Models;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Extensions;

namespace Rofl.UI.Main.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class MainWindowViewModel
    {
        private readonly FileManager _fileManager;
        private readonly RiZhi _log;
        // private readonly string _myName;
        
        public RequestManager RequestManager { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public QueryProperties SortParameters { get; private set; }

        /// <summary>
        /// Smaller, preview objects of replays
        /// </summary>
        public ObservableCollection<ReplayPreview> PreviewReplays { get; private set; }

        /// <summary>
        /// Full replay objects with the filepath as the key
        /// </summary>
        public Dictionary<string, FileResult> FileResults { get; private set; }

        public ObservableCollection<PlayerMarker> KnownPlayers { get; private set; }

        public SettingsManager SettingsManager { get; private set; }

        public StatusBar StatusBarModel { get; private set; }

        // public string LatestDataDragonVersion { get; private set; }

        public MainWindowViewModel(FileManager files, RequestManager requests, SettingsManager settingsManager, RiZhi log)
        {
            SettingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            _fileManager = files ?? throw new ArgumentNullException(nameof(files));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            RequestManager = requests ?? throw new ArgumentNullException(nameof(requests));

            KnownPlayers = SettingsManager.Settings.KnownPlayers;

            PreviewReplays = new ObservableCollection<ReplayPreview>();
            FileResults = new Dictionary<string, FileResult>();

            SortParameters = new QueryProperties
            {
                SearchTerm = String.Empty,
                SortMethod = SortMethod.DateDesc
            };

            StatusBarModel = new StatusBar();
        }

        /// <summary>
        /// Get replays from database and load to display
        /// </summary>
        public void LoadReplays()
        {
            _log.Information("Loading replays from database...");
            var databaseResults = _fileManager.GetReplays(SortParameters, SettingsManager.Settings.ItemsPerPage, PreviewReplays.Count);

            _log.Information($"Retrieved {databaseResults.Count} replays");
            foreach (var file in databaseResults)
            {
                var previewModel = CreateReplayPreview(file);

                App.Current.Dispatcher.Invoke((Action) delegate
                {
                    PreviewReplays.Add(previewModel);
                });

                FileResults.Add(file.FileInfo.Path, file);
            }
        }

        public ReplayPreview CreateReplayPreview(FileResult file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            ReplayPreview previewModel = new ReplayPreview(file.ReplayFile, file.FileInfo.CreationTime, file.IsNewFile);
            previewModel.IsSupported = SettingsManager.Executables.DoesVersionExist(previewModel.GameVersion);

            foreach (var bluePlayer in previewModel.BluePreviewPlayers)
            {
                bluePlayer.Marker = KnownPlayers.FirstOrDefault(x => x.Name.Equals(bluePlayer.PlayerName, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var redPlayer in previewModel.RedPreviewPlayers)
            {
                redPlayer.Marker = KnownPlayers.FirstOrDefault(x => x.Name.Equals(redPlayer.PlayerName, StringComparison.OrdinalIgnoreCase));
            }

            return previewModel;
        }

        public void ClearReplays()
        {
            _log.Information("Clearing replay list...");
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                PreviewReplays.Clear();
            });

            FileResults.Clear();
        }

        public async Task LoadItemThumbnails(ReplayDetail replay)
        {
            _log.Information("Loading/downloading thumbnails for items...");
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

            var dataVersion = await RequestManager.GetLatestDataDragonVersionAsync().ConfigureAwait(true);

            var allItems = new List<Item>();
            var itemTasks = new List<Task>();

            allItems.AddRange(replay.BluePlayers.SelectMany(x => x.Items));
            allItems.AddRange(replay.RedPlayers.SelectMany(x => x.Items));

            _log.Information($"Processing {allItems.Count} item thumbnail requests");
            foreach (var item in allItems)
            {
                // If an item does not exist, set it to nothing!
                if (item.ItemId == "0")
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        item.ShowBorder = true;
                    });
                    continue;
                }

                // Set default item image, to be replaced
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    item.ImageSource = ResourceTools.GetImageSourceFromResource("DownloadDrawingImage");
                });

                itemTasks.Add(Task.Run(async () =>
                {
                    var response = await RequestManager.MakeRequestAsync(new ItemRequest
                    {
                        DataDragonVersion = dataVersion,
                        ItemID = item.ItemId
                    }).ConfigureAwait(true);

                    if (response.IsFaulted)
                    {
                        _log.Warning($"Failed to load image for {(response.Request as ItemRequest).ItemID}");
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            item.ImageSource = ResourceTools.GetImageSourceFromResource("ErrorDrawingImage");
                        });
                    }
                    else
                    {
                        if (response.FromCache)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                item.ImageSource = ResourceTools.GetImageSourceFromPath(response.ResponsePath);
                            });
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                item.ImageSource = response.ResponseBytes.ToBitmapImage();
                            });
                        }
                    }

                }));
            }

            await Task.WhenAll(itemTasks).ConfigureAwait(true);
        }

        public async Task LoadPreviewPlayerThumbnails()
        {
            _log.Information("Loading/downloading thumbnails for champions...");

            foreach (var replay in PreviewReplays)
            {
                await LoadSinglePreviewPlayerThumbnails(replay).ConfigureAwait(true);
            }
        }

        public async Task LoadSinglePreviewPlayerThumbnails(ReplayPreview replay)
        {
            if (replay == null) throw new ArgumentNullException(nameof(replay));

            var dataVersion = await RequestManager.GetLatestDataDragonVersionAsync().ConfigureAwait(true);

            var allPlayers = new List<PlayerPreview>();
            allPlayers.AddRange(replay.BluePreviewPlayers);
            allPlayers.AddRange(replay.RedPreviewPlayers);

            _log.Information($"Processing {allPlayers.Count} champion thumbnail requests");
            var allRequests = new List<dynamic>(allPlayers.Select(x =>
                new
                {
                    Player = x,
                    Request = new ChampionRequest()
                    {
                        ChampionName = x.ChampionName,
                        DataDragonVersion = dataVersion
                    }
                }));

            var allTasks = new List<Task>();

            foreach (var request in allRequests)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    request.Player.ImageSource = ResourceTools.GetImageSourceFromResource("DownloadDrawingImage");
                });

                allTasks.Add(Task.Run(async () =>
                {
                    var response = await RequestManager.MakeRequestAsync(request.Request as RequestBase)
                        .ConfigureAwait(true);

                    if (response.IsFaulted)
                    {
                        _log.Warning($"Failed to load image for {(response.Request as ChampionRequest).ChampionName}");
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            request.Player.ImageSource = ResourceTools.GetImageSourceFromResource("ErrorDrawingImage");
                        });
                    }

                    if (response.FromCache)
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            request.Player.ImageSource = ResourceTools.GetImageSourceFromPath(response.ResponsePath);
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            request.Player.ImageSource = response.ResponseBytes.ToBitmapImage();
                        });
                    }
                }));
            }

            await Task.WhenAll(allTasks).ConfigureAwait(true);
        }

        public void ReloadPlayerMarkers()
        {
            _log.Information($"Refreshing player markers...");
            // Look through all replays to get all players
            foreach (var replay in PreviewReplays)
            {
                IEnumerable<PlayerPreview> allPlayers;
                if(replay.BluePreviewPlayers != null)
                {
                    allPlayers = replay.BluePreviewPlayers.Union(replay.RedPreviewPlayers);
                }
                else
                {
                    allPlayers = replay.RedPreviewPlayers;
                }

                foreach (var player in allPlayers)
                {
                    var matchedMarker = KnownPlayers.FirstOrDefault(x => x.Name.Equals(player.PlayerName, StringComparison.OrdinalIgnoreCase));

                    if(matchedMarker != null)
                    {
                        player.Marker = matchedMarker;
                    }
                }
            }
        }

        public async Task ShowSettingsDialog()
        {
            var settingsDialog = new SettingsWindow
            {
                Top = App.Current.MainWindow.Top + 50,
                Left = App.Current.MainWindow.Left + 50,
                DataContext = SettingsManager,
            };

            if (settingsDialog.ShowDialog().Equals(true))
            {
                // Refresh markers
                ReloadPlayerMarkers();

                // Refresh all replays
                await ReloadReplayList().ConfigureAwait(true);
            }
        }

        /// <summary>
        /// The function to call to refresh the list
        /// </summary>
        /// <returns></returns>
        public async Task ReloadReplayList()
        {
            _log.Information($"Refreshing replay list...");

            FileResults.Clear();
            PreviewReplays.Clear();
            ValidateReplayStorage();
            StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageReplay") as string;
            StatusBarModel.Color = Brushes.White;
            StatusBarModel.Visible = true;
            StatusBarModel.ShowProgressBar = true;
            await _fileManager.InitialLoadAsync().ConfigureAwait(true);
            LoadReplays();
            StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageThumbnails") as string;
            await LoadPreviewPlayerThumbnails().ConfigureAwait(true);
            StatusBarModel.Visible = false;
        }

        /// <summary>
        /// Function checks if replays in storage are valid. Removes any that are invalid.
        /// </summary>
        /// <returns></returns>
        public void ValidateReplayStorage()
        {
            StatusBarModel.StatusMessage = "Pruning storage...";
            StatusBarModel.Color = Brushes.White;
            StatusBarModel.Visible = true;
            StatusBarModel.ShowProgressBar = true;
            _fileManager.PruneDatabaseEntries();
            StatusBarModel.Visible = false;
        }

        public void PlayReplay(ReplayPreview preview)
        {
            _log.Information($"Playing replay...");

            if (preview == null) { throw new ArgumentNullException(nameof(preview)); }
            
            var replay = FileResults[preview.Location];

            var executables = SettingsManager.Executables.GetExecutablesByPatch(preview.GameVersion);

            if (!executables.Any())
            {
                _log.Information($"No executables found to play replay");

                // No executable found that can be used to play
                MessageBox.Show
                (
                    Application.Current.TryFindResource("ExecutableNotFoundErrorText") as String + " " + preview.GameVersion,
                    Application.Current.TryFindResource("ExecutableNotFoundErrorTitle") as String,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            LeagueExecutable target;
            if (executables.Count > 1)
            {
                _log.Information($"More than one possible executable, asking user...");
                // More than one?????
                target = ShowChooseReplayDialog(executables);
                if (target == null) return;
            }
            else
            {
                target = executables.First();
                
            }
            
            if (SettingsManager.Settings.PlayConfirmation)
            {
                _log.Information($"Asking user for confirmation");
                // Show confirmation dialog
                var msgResult = MessageBox.Show
                    (
                        Application.Current.TryFindResource("ReplayPlayConfirmationText") as String,
                        Application.Current.TryFindResource("ReplayPlayConfirmationText") as String,
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question
                    );

                if (msgResult != MessageBoxResult.OK) return;
            }

            _log.Information($"Using {target.Name} to play replay {replay.FileInfo.Path}");
            ReplayPlayer.Play(target, replay.FileInfo.Path);
        }

        public void OpenReplayContainingFolder(string location)
        {
            _log.Information($"Opening replay file location {location}");

            if (String.IsNullOrEmpty(location)) { throw new ArgumentNullException(nameof(location)); }

            FileResults.TryGetValue(location, out FileResult match);
            if (match == null) { throw new ArgumentException($"{location} does not match any known replays"); }

            string selectArg = $"/select, \"{match.FileInfo.Path}\"";
            Process.Start("explorer.exe", selectArg);
        }

        public void ViewOnlineMatchHistory(string matchid)
        {
            _log.Information($"Opening online match history for {matchid}");

            if (String.IsNullOrEmpty(matchid)) { throw new ArgumentNullException(nameof(matchid)); }

            string url = SettingsManager.Settings.MatchHistoryBaseUrl + matchid;
            Process.Start(url);
        }

        public static LeagueExecutable ShowChooseReplayDialog(IReadOnlyCollection<LeagueExecutable> executables)
        {
            var selectWindow = new ExecutableSelectWindow
            {
                Top = App.Current.MainWindow.Top + 50,
                Left = App.Current.MainWindow.Left + 50,
                DataContext = executables,
            };

            if (selectWindow.ShowDialog().Equals(true))
            {
                return selectWindow.Selection;
            }
            return null;
        }

        public void ShowExportReplayDataWindow(ReplayPreview preview)
        {
            _log.Information($"Showing Export Dialog...");

            if (preview == null) { throw new ArgumentNullException(nameof(preview)); }

            var exportContext = new ExportContext()
            {
                Replays = new ReplayFile[] { FileResults[preview.Location].ReplayFile },
                Markers = KnownPlayers.ToList()
            };

            var exportDialog = new ExportReplayDataWindow()
            {
                Top = App.Current.MainWindow.Top + 50,
                Left = App.Current.MainWindow.Left + 50,
                DataContext = exportContext,
            };

            exportDialog.ShowDialog();
        }

        public void ShowWelcomeWindow()
        {
            if (File.Exists("cache/SKIPWELCOME"))
            {
                _log.Information("Skipping welcome screen...");
                return;
            }

            _log.Information("Showing welcome screen...");
            var welcomeDialog = new WelcomeSetupWindow()
            {
                Top = App.Current.MainWindow.Top + 50,
                Left = App.Current.MainWindow.Left + 50,
                DataContext = this
            };

            welcomeDialog.ShowDialog();
        }

        public void WriteSkipWelcome()
        {
            if (File.Exists("cache/SKIPWELCOME"))
            {
                _log.Information("Welcome skip already exists, why was this called?");
                return;
            }

            _log.Information("Writing Welcome skip...");
            File.WriteAllText("cache/SKIPWELCOME", (string) Application.Current.TryFindResource("EggEggEgg"));
        }

        public void DeleteSkipWelcome()
        {
            if (File.Exists("cache/SKIPWELCOME"))
            {
                File.Delete("cache/SKIPWELCOME");
            }
        }

        public void ApplyInitialSettings(WelcomeSetupSettings initialSettings)
        {
            if (initialSettings == null) throw new ArgumentNullException(nameof(initialSettings));

            if (!string.IsNullOrEmpty(initialSettings.ReplayPath))
            {
                // Only add the replay folder if it doesn't exist already. It really should be empty...
                if (!SettingsManager.Settings.SourceFolders.Contains(initialSettings.ReplayPath))
                {
                    SettingsManager.Settings.SourceFolders.Add(initialSettings.ReplayPath);
                }
            }

            if (!string.IsNullOrEmpty(initialSettings.RiotGamesPath))
            {
                // Only add the executable folder if it doesn't exist already. It really should be empty...
                if (!SettingsManager.Executables.Settings.SourceFolders.Contains(initialSettings.RiotGamesPath))
                {
                    SettingsManager.Executables.Settings.SourceFolders.Add(initialSettings.RiotGamesPath);
                }
            }

            if (initialSettings.Executables != null)
            {
                foreach (var executable in initialSettings.Executables)
                {
                    SettingsManager.Executables.AddExecutable(executable);
                }
            }

            SettingsManager.Executables.Settings.DefaultLocale = initialSettings.DefaultRegionLocale;
        }

        public void ShowMissingReplayFoldersMessageBox()
        {
            var missingPaths = SettingsManager.RemoveInvalidReplayPaths();
            if (missingPaths.Length > 0)
            {
                var msg = Application.Current.TryFindResource("MissingPathText") as string + "\n\n";
                msg += string.Join(",\n", missingPaths);

                MessageBox.Show(
                    msg,
                    Application.Current.TryFindResource("MissingPathTitle") as string,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
      
        public async Task ShowRenameDialog(ReplayPreview preview)
        {
            if (preview == null) throw new ArgumentNullException(nameof(preview));

            var replay = FileResults[preview.Location];

            var renameDialog = new RenameFileDialog
            {
                FileName = Path.GetFileNameWithoutExtension(replay.Id),
                Top = App.Current.MainWindow.Top + 50,
                Left = App.Current.MainWindow.Left + 50
            };

            if (renameDialog.ShowDialog().Equals(true))
            {
                // Okay
                if(_fileManager.RenameFile(replay, renameDialog.FileName) != null)
                {
                    await ReloadReplayList().ConfigureAwait(false);
                }
            }
        }

        public async Task DeleteReplayFile(ReplayPreview preview)
        {
            if (preview == null) throw new ArgumentNullException(nameof(preview));

            _fileManager.DeleteFile(FileResults[preview.Location]);

            await ReloadReplayList().ConfigureAwait(false);
        }

        public void ClearDeletedReplays()
        {
            _fileManager.ClearDeletedFiles();
        }
    }
}
