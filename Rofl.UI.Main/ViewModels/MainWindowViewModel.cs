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
        private readonly ReplayPlayer _player;
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

        // Flags used to clear cache when closing
        public bool ClearItemsCacheOnClose { get; set; }
        public bool ClearChampsCacheOnClose { get; set; }
        public bool ClearRunesCacheOnClose { get; set; }

        public bool ClearReplayCacheOnClose { get; set; }

        // public string LatestDataDragonVersion { get; private set; }

        public MainWindowViewModel(FileManager files, RequestManager requests, SettingsManager settingsManager, ReplayPlayer player, RiZhi log)
        {
            SettingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            _fileManager = files ?? throw new ArgumentNullException(nameof(files));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _player = player ?? throw new ArgumentNullException(nameof(player));

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

            // By default we do not want to delete our cache
            ClearItemsCacheOnClose = false;
            ClearChampsCacheOnClose = false;
            ClearRunesCacheOnClose = false;
            ClearReplayCacheOnClose = false;
        }

        /// <summary>
        /// Get replays from database and load to display
        /// </summary>
        public int LoadReplaysFromDatabase()
        {
            _log.Information("Loading replays from database...");
            var databaseResults = _fileManager.GetReplays(SortParameters, SettingsManager.Settings.ItemsPerPage, PreviewReplays.Count);

            _log.Information($"Retrieved {databaseResults.Count} replays");
            
            foreach (var file in databaseResults)
            {
                AddReplay(file);
            }

            return databaseResults.Count;
        }

        /// <summary>
        /// Save a Replay File result
        /// </summary>
        /// <param name="fileResult"></param>
        public ReplayPreview AddReplay(FileResult file)
        {
            var previewModel = CreateReplayPreview(file);

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                PreviewReplays.Add(previewModel);
            });

            FileResults.Add(file.FileInfo.Path, file);

            return previewModel;
        }

        public ReplayPreview CreateReplayPreview(FileResult file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            ReplayPreview previewModel = new ReplayPreview(file.ReplayFile,
                file.FileInfo.CreationTime,
                SettingsManager.Settings.PlayerMarkerStyle,
                SettingsManager.Settings.RenameAction,
                file.IsNewFile);

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
                    item.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("DownloadPathIcon");
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
                            item.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                        });
                    }
                    else
                    {
                        if (response.FromCache)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                item.OverlayIcon = null; // hide overlay icons, if any
                                item.ImageSource = ResourceTools.GetImageSourceFromPath(response.ResponsePath);
                            });
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                item.OverlayIcon = null; // hide overlay icons, if any
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
                    request.Player.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("DownloadPathIcon");
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
                            request.Player.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                        });
                    }

                    if (response.FromCache)
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            request.Player.OverlayIcon = null; // hide overlay icons, if any
                            request.Player.ImageSource = ResourceTools.GetImageSourceFromPath(response.ResponsePath);
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            request.Player.OverlayIcon = null; // hide overlay icons, if any
                            request.Player.ImageSource = response.ResponseBytes.ToBitmapImage();
                        });
                    }
                }));
            }

            await Task.WhenAll(allTasks).ConfigureAwait(true);
        }

        public async Task LoadRuneThumbnails(ReplayDetail replay)
        {
            _log.Information("Loading/downloading thumbnails for runes...");
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

            string dataVersion = await RequestManager.GetLatestDataDragonVersionAsync().ConfigureAwait(true);

            List<Rune> allRunes = new List<Rune>();
            List<Task> allTasks = new List<Task>();

            allRunes.AddRange(replay.AllPlayers.Select(x => x.KeystoneRune));
            allRunes.AddRange(replay.AllPlayers.SelectMany(x => x.Runes));

            _log.Information($"Processing {allRunes.Count} rune thumbnail requests");
            foreach (Rune rune in allRunes)
            {
                RuneJson runeData = RuneHelper.GetRune(rune.RuneId);
                // If an item does not exist, set it to nothing!
                if (string.IsNullOrEmpty(runeData.Icon))
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        rune.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                    });
                    // move on to the next rune
                    continue;
                }

                // Set default item image, to be replaced
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    rune.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("DownloadPathIcon");
                });

                // make requests for images
                allTasks.Add(Task.Run(async () =>
                {
                    ResponseBase response = await RequestManager.MakeRequestAsync(new RuneRequest
                    {
                        DataDragonVersion = dataVersion,
                        RuneKey = runeData.Key,
                        TargetPath = runeData.Icon
                    }).ConfigureAwait(true);

                    if (response.IsFaulted)
                    {
                        _log.Warning($"Failed to load image for {(response.Request as RuneRequest).RuneKey}");
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            rune.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                        });
                    }
                    else
                    {
                        if (response.FromCache) // load image from file
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                rune.OverlayIcon = null; // hide overlay icons, if any
                                rune.ImageSource = ResourceTools.GetImageSourceFromPath(response.ResponsePath);
                            });
                        }
                        else // load image straight from response if its not cachsed
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                rune.OverlayIcon = null; // hide overlay icons, if any
                                rune.ImageSource = response.ResponseBytes.ToBitmapImage();
                            });
                        }
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
            StatusBarModel.Visible = true;
            StatusBarModel.ShowProgressBar = true;
            
            // Discover and load replays into database
            await _fileManager.InitialLoadAsync().ConfigureAwait(true);

            // Load from database into our viewmodel
            LoadReplaysFromDatabase();

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
            StatusBarModel.Visible = true;
            StatusBarModel.ShowProgressBar = true;
            _fileManager.PruneDatabaseEntries();
            StatusBarModel.Visible = false;
        }

        public async Task<Process> PlayReplay(ReplayPreview preview)
        {
            _log.Information($"Playing replay...");

            if (preview == null) { throw new ArgumentNullException(nameof(preview)); }
            
            var process = await _player.PlayReplay(preview.Location).ConfigureAwait(true);
            // if process is null, replay failed to play for whatever reason
            if(process != null)
            {
                preview.IsPlaying = true;
                // add event handler for when the replay stops
                process.Exited += (object processSender, System.EventArgs processEventArgs) =>
                {
                    preview.IsPlaying = false;
                };
            }

            return process;
        }

        public void OpenReplayContainingFolder(string location)
        {
            _log.Information($"Opening replay file location {location}");

            if (String.IsNullOrEmpty(location)) { throw new ArgumentNullException(nameof(location)); }

            //FileResults.TryGetValue(location, out FileResult match);
            //if (match == null) { throw new ArgumentException($"{location} does not match any known replays"); }

            string selectArg = $"/select, \"{location}\"";
            Process.Start("explorer.exe", selectArg);
        }

        public void ViewOnlineMatchHistory(string matchid)
        {
            _log.Information($"Opening online match history for {matchid}");

            if (String.IsNullOrEmpty(matchid)) { throw new ArgumentNullException(nameof(matchid)); }

            string url = SettingsManager.Settings.MatchHistoryBaseUrl + matchid;
            Process.Start(url);
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
            var welcomeFileFlag = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/SKIPWELCOME");
            if (File.Exists(welcomeFileFlag))
            {
                _log.Information("Skipping welcome screen...");
                return;
            }

            _log.Information(welcomeFileFlag);
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
            var welcomeFileFlag = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/SKIPWELCOME");

            if (File.Exists(welcomeFileFlag))
            {
                _log.Information("Welcome skip already exists, why was this called?");
                return;
            }

            _log.Information("Writing Welcome skip...");
            File.WriteAllText(welcomeFileFlag, (string) Application.Current.TryFindResource("EggEggEgg"));
        }

        public void DeleteSkipWelcome()
        {
            var welcomeFileFlag = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/SKIPWELCOME");

            if (File.Exists(welcomeFileFlag))
            {
                File.Delete(welcomeFileFlag);
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
            SettingsManager.Settings.ProgramLanguage = initialSettings.SetupLanguage;
        }

        /// <summary>
        /// Function is called at the start of the program, checking for invalid replay paths
        /// </summary>
        public void ShowMissingReplayFoldersMessageBox()
        {
            // Check if replay paths are missing, if so remove them
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

        /// <summary>
        /// Renames a given replay and refreshes the list
        /// </summary>
        /// <param name="preview"></param>
        /// <param name="newText"></param>
        /// <returns></returns>
        public string RenameFile(ReplayPreview preview, string newText)
        {
            if (preview == null) throw new ArgumentNullException(nameof(preview));

            // Get the full replay object, it contains more information
            var replay = FileResults[preview.Location];

            // Ask the file manager to rename the replay
            var error = _fileManager.RenameReplay(replay, newText);

            // User entered nothing, change message
            if (error == "{EMPTY ERROR}")
            {
                error = Application.Current.TryFindResource("RenameFlyoutEmptyError") as string;
            }
            else if (error == "{NOT FOUND ERROR}")
            {
                error = Application.Current.TryFindResource("RenameFlyoutNotFoundError") as string;
            }
            else // Success
            {
                // Change the displayed data to the new name
                preview.DisplayName = newText;
            }

            return error;
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

        public async Task<(long ItemsTotalSize, long ChampsTotalSize, long RunesTotalSize)> CalculateCacheSizes()
        {
            var itemsInfo = new DirectoryInfo(RequestManager.GetItemCachePath());
            long itemsTotal = !itemsInfo.Exists ? 0L : await Task.Run(() => itemsInfo.EnumerateFiles("*.png").Sum(file => file.Length)).ConfigureAwait(true);
            
            var champsInfo = new DirectoryInfo(RequestManager.GetChampionCachePath());
            long champsTotal = !champsInfo.Exists ? 0L : await Task.Run(() => champsInfo.EnumerateFiles("*.png").Sum(file => file.Length)).ConfigureAwait(true);

            var runesInfo = new DirectoryInfo(RequestManager.GetRuneCachePath());
            long runesTotal = !runesInfo.Exists ? 0L : await Task.Run(() => runesInfo.EnumerateFiles("*.png").Sum(file => file.Length)).ConfigureAwait(true);

            return (itemsTotal, champsTotal, runesTotal);
        }

        public long CalculateReplayCacheSize()
        {
            var databaseInfo = new FileInfo(_fileManager.DatabasePath);
            return databaseInfo.Length;
        }

        public async Task ClearCache()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            if (ClearItemsCacheOnClose) await RequestManager.ClearItemCache().ConfigureAwait(true);
            if (ClearChampsCacheOnClose) await RequestManager.ClearChampionCache().ConfigureAwait(true);
            if (ClearRunesCacheOnClose) await RequestManager.ClearRunesCache().ConfigureAwait(true);

            if (ClearReplayCacheOnClose)
            {
                _fileManager.DeleteDatabase();
            }
        }
    }
}

