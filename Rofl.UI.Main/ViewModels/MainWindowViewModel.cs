using Etirps.RiZhi;
using Rofl.Files;
using Rofl.Files.Models;
using Rofl.Requests;
using Rofl.Requests.Models;
using Rofl.Settings;
using Rofl.Settings.Models;
using Rofl.UI.Main.Extensions;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Rofl.UI.Main.ViewModels
{
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

        // flag to indicate something is wrong with internet connection
        private bool InternetFailed { get; set; }

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
                SearchTerm = string.Empty,
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
            IReadOnlyCollection<FileResult> databaseResults = _fileManager.GetReplays(SortParameters, SettingsManager.Settings.ItemsPerPage, PreviewReplays.Count);

            _log.Information($"Retrieved {databaseResults.Count} replays");

            foreach (FileResult file in databaseResults)
            {
                _ = AddReplay(file);
            }

            return databaseResults.Count;
        }

        /// <summary>
        /// Save a Replay File result
        /// </summary>
        /// <param name="fileResult"></param>
        public ReplayPreview AddReplay(FileResult file)
        {
            ReplayPreview previewModel = CreateReplayPreview(file);

            Application.Current.Dispatcher.Invoke(delegate
            {
                PreviewReplays.Add(previewModel);
            });

            FileResults.Add(file.FileInfo.Path, file);

            return previewModel;
        }

        public ReplayPreview CreateReplayPreview(FileResult file)
        {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }

            ReplayPreview previewModel = new ReplayPreview(file.ReplayFile,
                file.FileInfo.CreationTime,
                SettingsManager.Settings.PlayerMarkerStyle,
                SettingsManager.Settings.RenameAction,
                file.IsNewFile);

            previewModel.IsSupported = SettingsManager.Executables.DoesVersionExist(previewModel.GameVersion);

            foreach (PlayerPreview bluePlayer in previewModel.BluePreviewPlayers)
            {
                bluePlayer.Marker = KnownPlayers.FirstOrDefault(x => x.Name.Equals(bluePlayer.PlayerName, StringComparison.OrdinalIgnoreCase));
            }

            foreach (PlayerPreview redPlayer in previewModel.RedPreviewPlayers)
            {
                redPlayer.Marker = KnownPlayers.FirstOrDefault(x => x.Name.Equals(redPlayer.PlayerName, StringComparison.OrdinalIgnoreCase));
            }

            return previewModel;
        }

        public void ClearReplays()
        {
            _log.Information("Clearing replay list...");
            Application.Current.Dispatcher.Invoke(delegate
            {
                PreviewReplays.Clear();
            });

            FileResults.Clear();
        }

        public async Task LoadItemThumbnails(ReplayDetail replay)
        {
            _log.Information("Loading/downloading thumbnails for items...");
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

            // we already know the internet failed
            if (InternetFailed) { return; }

            string dataVersion;
            try
            {
                dataVersion = await RequestManager.GetLatestDataDragonVersionAsync().ConfigureAwait(true);
            }
            catch (HttpRequestException)
            {
                _log.Error("Could not load item thumbnails due to http exception");
                InternetFailed = true;
                return;
            }

            List<Item> allItems = new List<Item>();
            List<Task> itemTasks = new List<Task>();

            allItems.AddRange(replay.BluePlayers.SelectMany(x => x.Items));
            allItems.AddRange(replay.RedPlayers.SelectMany(x => x.Items));

            _log.Information($"Processing {allItems.Count} item thumbnail requests");
            foreach (Item item in allItems)
            {
                // If an item does not exist, set it to nothing!
                if (item.ItemId == "0")
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        item.ShowBorder = true;

                        // the default image is an error, so clear it out
                        item.OverlayIcon = null;
                    });
                    continue;
                }

                // Set default item image, to be replaced
                Application.Current.Dispatcher.Invoke(delegate
                {
                    item.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("DownloadPathIcon");
                });

                itemTasks.Add(Task.Run(async () =>
                {
                    ResponseBase response = await RequestManager.MakeRequestAsync(new ItemRequest
                    {
                        DataDragonVersion = dataVersion,
                        ItemID = item.ItemId
                    }).ConfigureAwait(true);

                    if (response.IsFaulted)
                    {
                        _log.Warning($"Failed to load image for {(response.Request as ItemRequest).ItemID}");
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            item.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                        });
                    }
                    else
                    {
                        if (response.FromCache)
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                item.OverlayIcon = null; // hide overlay icons, if any
                                item.ImageSource = ResourceTools.GetImageSourceFromPath(response.ResponsePath);
                            });
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(delegate
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

            foreach (ReplayPreview replay in PreviewReplays)
            {
                await LoadSinglePreviewPlayerThumbnails(replay).ConfigureAwait(true);
            }
        }

        public async Task LoadSinglePreviewPlayerThumbnails(ReplayPreview replay)
        {
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

            // we already know the internet failed
            if (InternetFailed) { return; }

            string dataVersion;
            try
            {
                dataVersion = await RequestManager.GetLatestDataDragonVersionAsync().ConfigureAwait(true);
            }
            catch (HttpRequestException)
            {
                _log.Error("Could not load player thumbnails due to http exception");
                InternetFailed = true;
                return;
            }

            List<PlayerPreview> allPlayers = new List<PlayerPreview>();
            allPlayers.AddRange(replay.BluePreviewPlayers);
            allPlayers.AddRange(replay.RedPreviewPlayers);

            _log.Information($"Processing {allPlayers.Count} champion thumbnail requests");
            List<dynamic> allRequests = new List<dynamic>(allPlayers.Select(x =>
                new
                {
                    Player = x,
                    Request = new ChampionRequest()
                    {
                        ChampionName = x.ChampionName,
                        DataDragonVersion = dataVersion
                    }
                }));

            List<Task> allTasks = new List<Task>();

            foreach (dynamic request in allRequests)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    request.Player.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("DownloadPathIcon");
                });

                allTasks.Add(Task.Run(async () =>
                {
                    ResponseBase response = await RequestManager.MakeRequestAsync(request.Request as RequestBase)
                        .ConfigureAwait(true);

                    if (response.IsFaulted)
                    {
                        _log.Warning($"Failed to load image for {(response.Request as ChampionRequest).ChampionName}");
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            request.Player.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                        });
                    }

                    if (response.FromCache)
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            request.Player.OverlayIcon = null; // hide overlay icons, if any
                            request.Player.ImageSource = ResourceTools.GetImageSourceFromPath(response.ResponsePath);
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(delegate
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

            List<RuneStat> allRunes = new List<RuneStat>();
            List<Task> allTasks = new List<Task>();

            allRunes.AddRange(replay.AllPlayers.Select(x => x.KeystoneRune));
            allRunes.AddRange(replay.AllPlayers.SelectMany(x => x.Runes));
            allRunes.AddRange(replay.AllPlayers.SelectMany(x => x.StatsRunes));

            _log.Information($"Processing {allRunes.Count} rune thumbnail requests");
            foreach (RuneStat rune in allRunes)
            {
                Rune runeData = RuneHelper.GetRune(rune.RuneId);
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
                Application.Current.Dispatcher.Invoke(delegate
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
            foreach (ReplayPreview replay in PreviewReplays)
            {
                IEnumerable<PlayerPreview> allPlayers = replay.BluePreviewPlayers != null
                    ? replay.BluePreviewPlayers.Union(replay.RedPreviewPlayers)
                    : replay.RedPreviewPlayers;

                foreach (PlayerPreview player in allPlayers)
                {
                    PlayerMarker matchedMarker = KnownPlayers.FirstOrDefault(x => x.Name.Equals(player.PlayerName, StringComparison.OrdinalIgnoreCase));

                    if (matchedMarker != null)
                    {
                        player.Marker = matchedMarker;
                    }
                }
            }
        }

        public async Task ShowSettingsDialog()
        {
            SettingsWindow settingsDialog = new SettingsWindow
            {
                Top = Application.Current.MainWindow.Top + 50,
                Left = Application.Current.MainWindow.Left + 50,
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

            // Validate executables
            StatusBarModel.Visible = true;
            StatusBarModel.ShowProgressBar = true;
            StatusBarModel.ShowDismissButton = false;
            StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageExecutables") as string;
            await SettingsManager.Executables.VerifyRegisteredExecutables().ConfigureAwait(true);

            // Clear previously loaded replays
            FileResults.Clear();
            PreviewReplays.Clear();
            ValidateReplayStorage(closeOnComplete: false);

            // Discover and load replays into database
            IEnumerable<FileErrorResult> results = await _fileManager.InitialLoadAsync().ConfigureAwait(true);

            // Load from database into our viewmodel
            _ = LoadReplaysFromDatabase();

            // Load thumbnails
            StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageThumbnails") as string;
            await LoadPreviewPlayerThumbnails().ConfigureAwait(true);

            if (results.Any())
            {
                StatusBarModel.ShowProgressBar = false;
                StatusBarModel.ShowDismissButton = true;
                StatusBarModel.Errors = results;
                StatusBarModel.StatusMessage = $"{results.Count()} {Application.Current.TryFindResource("LoadingMessageErrors")}";
            }
            else
            {
                StatusBarModel.Visible = false;
            }
        }

        /// <summary>
        /// Function checks if replays in storage are valid. Removes any that are invalid.
        /// </summary>
        /// <returns></returns>
        public void ValidateReplayStorage(bool closeOnComplete)
        {
            StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageReplay") as string;
            StatusBarModel.Visible = true;
            StatusBarModel.ShowProgressBar = true;
            _fileManager.PruneDatabaseEntries();
            StatusBarModel.Visible = !closeOnComplete;
        }

        public async Task<Process> PlayReplay(ReplayPreview preview)
        {
            _log.Information($"Playing replay...");

            if (preview == null) { throw new ArgumentNullException(nameof(preview)); }

            Process process = await _player.PlayReplay(preview.Location).ConfigureAwait(true);
            // if process is null, replay failed to play for whatever reason
            if (process != null)
            {
                preview.IsPlaying = true;
                // add event handler for when the replay stops
                process.Exited += (object processSender, EventArgs processEventArgs) =>
                {
                    preview.IsPlaying = false;
                };
            }

            return process;
        }

        public void OpenReplayContainingFolder(string location)
        {
            _log.Information($"Opening replay file location {location}");

            if (string.IsNullOrEmpty(location)) { throw new ArgumentNullException(nameof(location)); }

            //FileResults.TryGetValue(location, out FileResult match);
            //if (match == null) { throw new ArgumentException($"{location} does not match any known replays"); }

            string selectArg = $"/select, \"{location}\"";
            _ = Process.Start("explorer.exe", selectArg);
        }

        public void ShowExportReplayDataWindow(ReplayPreview preview)
        {
            _log.Information($"Showing Export Dialog...");

            if (preview == null) { throw new ArgumentNullException(nameof(preview)); }

            // create transition collection
            ModernWpf.Media.Animation.TransitionCollection contentTransitions = new ModernWpf.Media.Animation.TransitionCollection
            {
                new ModernWpf.Media.Animation.NavigationThemeTransition()
                {
                    DefaultNavigationTransitionInfo = new ModernWpf.Media.Animation.SlideNavigationTransitionInfo()
                    {
                        Effect = ModernWpf.Media.Animation.SlideNavigationTransitionEffect.FromRight
                    }
                }
            };

            // create content frame
            ModernWpf.Controls.Frame contentFrame = new ModernWpf.Controls.Frame()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ContentTransitions = contentTransitions
            };

            // create default context values
            ExportDataContext exportDataContext = new ExportDataContext
            {
                PresetName = "unsavedPreset",
                ManualPlayerSelection = true,
                AlwaysIncludeMarked = false,
                IncludeAllPlayers = false,
                ReplayPreview = preview,
                Replay = FileResults[preview.Location].ReplayFile,
                ContentFrame = contentFrame,
                HideHeader = false,
                WindowTitleText = Application.Current.FindResource("ErdTitle") as string,
                Log = _log
            };

            ExportDataWindow exportDialog = new ExportDataWindow()
            {
                Top = Application.Current.MainWindow.Top + 50,
                Left = Application.Current.MainWindow.Left + 50,
                DataContext = exportDataContext,
            };

            _ = exportDialog.ShowDialog();
        }

        public void ShowWelcomeWindow()
        {
            string welcomeFileFlag = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/SKIPWELCOME");
            if (File.Exists(welcomeFileFlag))
            {
                _log.Information("Skipping welcome screen...");
                return;
            }

            _log.Information(welcomeFileFlag);
            WelcomeSetupWindow welcomeDialog = new WelcomeSetupWindow()
            {
                Top = Application.Current.MainWindow.Top + 50,
                Left = Application.Current.MainWindow.Left + 50,
                DataContext = this
            };

            _ = welcomeDialog.ShowDialog();
        }

        public void WriteSkipWelcome()
        {
            string welcomeFileFlag = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/SKIPWELCOME");

            if (File.Exists(welcomeFileFlag))
            {
                _log.Information("Welcome skip already exists, why was this called?");
                return;
            }

            _log.Information("Writing Welcome skip...");
            File.WriteAllText(welcomeFileFlag, contents: (string)Application.Current.TryFindResource("EggEggEgg"));
        }

        public static void DeleteSkipWelcome()
        {
            string welcomeFileFlag = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/SKIPWELCOME");

            if (File.Exists(welcomeFileFlag))
            {
                File.Delete(welcomeFileFlag);
            }
        }

        public void ApplyInitialSettings(WelcomeSetupSettings initialSettings)
        {
            if (initialSettings == null) { throw new ArgumentNullException(nameof(initialSettings)); }

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
                foreach (Executables.Models.LeagueExecutable executable in initialSettings.Executables)
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
            string[] missingPaths = SettingsManager.RemoveInvalidReplayPaths();
            if (missingPaths.Length > 0)
            {
                string msg = (Application.Current.TryFindResource("MissingPathText") as string) + "\n\n";
                msg += string.Join(",\n", missingPaths);

                _ = MessageBox.Show(
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
            if (preview == null) { throw new ArgumentNullException(nameof(preview)); }

            // Get the full replay object, it contains more information
            FileResult replay = FileResults[preview.Location];

            // Ask the file manager to rename the replay
            string error = _fileManager.RenameReplay(replay, newText);

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
            if (preview == null) { throw new ArgumentNullException(nameof(preview)); }

            _ = _fileManager.DeleteFile(FileResults[preview.Location]);

            await ReloadReplayList().ConfigureAwait(false);
        }

        public void ClearDeletedReplays()
        {
            _fileManager.ClearDeletedFiles();
        }

        public async Task<(long ItemsTotalSize, long ChampsTotalSize, long RunesTotalSize)> CalculateCacheSizes()
        {
            DirectoryInfo itemsInfo = new DirectoryInfo(RequestManager.GetItemCachePath());
            long itemsTotal = !itemsInfo.Exists ? 0L : await Task.Run(() => itemsInfo.EnumerateFiles("*.png").Sum(file => file.Length)).ConfigureAwait(true);

            DirectoryInfo champsInfo = new DirectoryInfo(RequestManager.GetChampionCachePath());
            long champsTotal = !champsInfo.Exists ? 0L : await Task.Run(() => champsInfo.EnumerateFiles("*.png").Sum(file => file.Length)).ConfigureAwait(true);

            DirectoryInfo runesInfo = new DirectoryInfo(RequestManager.GetRuneCachePath());
            long runesTotal = !runesInfo.Exists ? 0L : await Task.Run(() => runesInfo.EnumerateFiles("*.png").Sum(file => file.Length)).ConfigureAwait(true);

            return (itemsTotal, champsTotal, runesTotal);
        }

        public long CalculateReplayCacheSize()
        {
            FileInfo databaseInfo = new FileInfo(_fileManager.DatabasePath);
            return databaseInfo.Length;
        }

        public async Task ClearCache()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (ClearItemsCacheOnClose) { await RequestManager.ClearItemCache().ConfigureAwait(true); }
            if (ClearChampsCacheOnClose) { await RequestManager.ClearChampionCache().ConfigureAwait(true); }
            if (ClearRunesCacheOnClose) { await RequestManager.ClearRunesCache().ConfigureAwait(true); }

            if (ClearReplayCacheOnClose)
            {
                _fileManager.DeleteDatabase();
            }
        }
    }
}

