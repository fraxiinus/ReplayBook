namespace Fraxiinus.ReplayBook.UI.Main.ViewModels;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Models.Search;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using Fraxiinus.ReplayBook.UI.Main.Models.View;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.Views;
using Fraxiinus.Rofl.Extract.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

public class MainWindowViewModel
{
    #region Private Variables
    private readonly FileManager _fileManager;
    private readonly RiZhi _log;
    private readonly ReplayPlayer _player;
    #endregion

    #region Properties
    /// <summary>
    /// In charge of static data, game images and text
    /// </summary>
    public StaticDataManager StaticDataManager { get; private set; }

    /// <summary>
    /// Contains properties used for filtering and sorting replays
    /// </summary>
    public SearchParameters SortParameters { get; private set; }

    /// <summary>
    /// Smaller, preview objects of replays
    /// </summary>
    public ObservableCollection<ReplayPreview> PreviewReplays { get; private set; }

    /// <summary>
    /// Full replay objects with the filepath as the key
    /// </summary>
    public Dictionary<string, FileResult> FileResults { get; private set; }

    /// <summary>
    /// Application configuration
    /// </summary>
    public ObservableConfiguration Configuration { get; private set; }

    /// <summary>
    /// Deals with League of Legends executables
    /// </summary>
    public ExecutableManager ExecutableManager { get; private set; }

    /// <summary>
    /// Contains properties used for displaying replay list status bar
    /// </summary>
    public StatusBar StatusBarModel { get; private set; }

    /// <summary>
    /// Flag used to clear cache (database and search) when closing
    /// </summary>
    public bool ClearReplayCacheOnClose { get; set; }

    /// <summary>
    /// Flag to communicate with startup method to restart the application
    /// </summary>
    public bool RestartOnClose { get; set; }

    #endregion

    public MainWindowViewModel(FileManager files,
        StaticDataManager staticData,
        ObservableConfiguration config,
        ExecutableManager executables,
        ReplayPlayer player,
        RiZhi log)
    {
        _fileManager = files ?? throw new ArgumentNullException(nameof(files));
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _player = player ?? throw new ArgumentNullException(nameof(player));

        Configuration = config ?? throw new ArgumentNullException(nameof(config));
        ExecutableManager = executables ?? throw new ArgumentNullException(nameof(executables));
        StaticDataManager = staticData ?? throw new ArgumentNullException(nameof(staticData));

        // Create new, blank objects
        PreviewReplays = new ObservableCollection<ReplayPreview>();
        FileResults = new Dictionary<string, FileResult>();
        StatusBarModel = new StatusBar();
        SortParameters = new SearchParameters
        {
            QueryString = string.Empty,
            SortMethod = SortMethod.DateDesc
        };

        // By default we do not want to delete our cache
        ClearReplayCacheOnClose = false;
        RestartOnClose = false;
    }

    /// <summary>
    /// Get replays from database and stores in collections
    /// </summary>
    public (int received, int total, Exception fault) LoadReplaysFromDatabase(bool resetSearch = false)
    {
        _log.Information("Loading replays from database...");

        IReadOnlyCollection<FileResult> databaseResults;
        int searchResultCount;
        try
        {
            (databaseResults, searchResultCount) = _fileManager.GetReplays(SortParameters, Configuration.ItemsPerPage, PreviewReplays.Count, resetSearch);
        }
        catch (Exception ex)
        {
            _log.Error(ex.ToString());
            return (0, 0, ex);
        }

        _log.Information($"Retrieved {databaseResults.Count} replays");

        foreach (FileResult file in databaseResults)
        {
            AddReplayToCollection(file);
        }

        return (databaseResults.Count, searchResultCount, null);
    }

    /// <summary>
    /// Save a Replay File result
    /// </summary>
    /// <param name="fileResult"></param>
    public ReplayPreview AddReplayToCollection(FileResult file)
    {
        if (file == null) { throw new ArgumentNullException(nameof(file)); }

        // create preview model, used in replay list
        var previewModel = new ReplayPreview(file, Configuration);
        previewModel.IsSupported = ExecutableManager.DoesVersionExist(previewModel.GameVersion);

        // add to collection
        Application.Current.Dispatcher.Invoke(delegate
        {
            PreviewReplays.Add(previewModel);
        });

        // skip if file already exists
        if (!FileResults.ContainsKey(file.FileInfo.Path))
        {
            FileResults.Add(file.FileInfo.Path, file);
        }

        return previewModel;
    }

    public async Task LoadItemThumbnails(ReplayDetail replay)
    {
        _log.Information("Loading thumbnails for items...");
        if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

        var patchVersion = replay.PreviewModel.GameVersion.VersionSubstring();
        var allItems = new List<Item>();

        allItems.AddRange(replay.AllPlayers.SelectMany(x => x.Items));

        foreach (var item in allItems)
        {
            var staticItem = await StaticDataManager.GetProperties<ItemProperties>(item.ItemId, patchVersion, LanguageHelper.CurrentLanguage);
            // given id was invalid
            if (staticItem == default || staticItem.ImageProperties == null)
            {
                if (item.ItemId == "0")
                {
                    // no item, show empty space with border
                    item.OverlayIcon = null;
                    item.ShowBorder = true;
                }
                else if (item.ItemId == "-1")
                {
                    // item id was invalid, keep error visible, show N/A
                    item.ItemName = staticItem.DisplayName;
                }
                else
                {
                    // item id was valid, maybe? show id
                    item.ItemName = item.ItemId;
                }
            }
            else
            {
                var image = StaticDataManager.GetAtlasImage(staticItem.ImageProperties.Source, patchVersion);
                if (image == null)
                {
                    // show id, we have no image to present
                    item.ItemName = staticItem.Id;
                }
                else
                {
                    // hide overlay, show apply item image and add name
                    item.OverlayIcon = null;
                    item.ItemName = staticItem.DisplayName;
                    item.Image = staticItem.CreateBrush(image);
                }
            }
        }
    }

    public async Task LoadPreviewPlayerThumbnails()
    {
        foreach (ReplayPreview replay in PreviewReplays)
        {
            await LoadSinglePreviewPlayerThumbnails(replay);
        }
    }

    public async Task LoadSinglePreviewPlayerThumbnails(ReplayPreview replay)
    {
        _log.Information("Loading thumbnails for players...");
        if (replay == null) { throw new ArgumentNullException(nameof(replay)); }
        if (replay.IsErrorReplay) { return; } // TODO

        var patchVersion = replay.GameVersion.VersionSubstring();
        var allPlayers = new List<PlayerPreview>();

        allPlayers.AddRange(replay.RedPreviewPlayers);
        allPlayers.AddRange(replay.BluePreviewPlayers);

        foreach (var player in allPlayers)
        {
            var staticChamp = await StaticDataManager.GetProperties<ChampionProperties>(player.ChampionId, patchVersion, LanguageHelper.CurrentLanguage);

            if (staticChamp == default || staticChamp.DisplayName == "N/A")
            {
                // no champion data was found, load id as the name
                player.ChampionName = player.ChampionId;
            }
            else
            {
                var image = StaticDataManager.GetAtlasImage(staticChamp.ImageProperties.Source, patchVersion);
                if (image == null)
                {
                    // we have no image, use the name
                    player.ChampionName = staticChamp.DisplayName;
                }
                else
                {
                    // hide overlay, show apply item image and add name
                    player.OverlayIcon = null;
                    player.ChampionName = staticChamp.DisplayName;
                    player.Image = staticChamp.CreateBrush(image);
                }
            }
        }
    }

    public async Task LoadRuneThumbnails(ReplayDetail replay)
    {
        _log.Information("Loading/downloading thumbnails for runes...");
        if (replay == null) { throw new ArgumentNullException(nameof(replay)); }

        var patchVersion = replay.PreviewModel.GameVersion.VersionSubstring();

        var allRunes = new List<RuneStat>();
        var allTasks = new List<Task>();

        allRunes.AddRange(replay.AllPlayers.Select(x => x.KeystoneRune));
        allRunes.AddRange(replay.AllPlayers.SelectMany(x => x.Runes));
        allRunes.AddRange(replay.AllPlayers.SelectMany(x => x.StatsRunes));

        _log.Information($"Processing {allRunes.Count} rune thumbnail requests");
        foreach (RuneStat rune in allRunes)
        {
            var runeData = await StaticDataManager.GetProperties<RuneProperties>(rune.RuneId, patchVersion, LanguageHelper.CurrentLanguage);
            // If an item does not exist, set it to nothing!
            if (runeData == default || string.IsNullOrEmpty(runeData.IconUrl))
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    rune.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                });
            }
            else
            {
                var runeImagePath = StaticDataManager.GetRuneImagePath(runeData.Key, patchVersion);

                if (runeImagePath == null)
                {
                    rune.OverlayIcon = ResourceTools.GetObjectFromResource<Geometry>("ErrorPathIcon");
                }
                else
                {
                    rune.OverlayIcon = null; // hide overlay icons, if any
                    rune.ImageSource = ResourceTools.GetImageSourceFromPath(runeImagePath);
                }
            }
        }

        await Task.WhenAll(allTasks).ConfigureAwait(true);
    }

    public void ReloadPlayerMarkers()
    {
        _log.Information($"Refreshing player markers...");
        // Look through all replays to get all players
        foreach (ReplayPreview replay in PreviewReplays)
        {
            if (replay.IsErrorReplay) { continue; } // TODO

            IEnumerable<PlayerPreview> allPlayers = replay.BluePreviewPlayers != null
                ? replay.BluePreviewPlayers.Union(replay.RedPreviewPlayers)
                : replay.RedPreviewPlayers;

            foreach (PlayerPreview player in allPlayers)
            {
                PlayerMarkerConfiguration matchedMarker = Configuration.PlayerMarkers
                    .FirstOrDefault(x => x.Name.Equals(player.PlayerName, StringComparison.OrdinalIgnoreCase));

                if (matchedMarker != null)
                {
                    player.Marker = matchedMarker;
                }
            }
        }
    }

    public async Task ShowSettingsDialog()
    {
        var settingsDialog = new SettingsWindow()
        {
            Top = Application.Current.MainWindow.Top + 50,
            Left = Application.Current.MainWindow.Left + 50,
            DataContext = new SettingsWindowDataContext
            {
                Configuration = Configuration,
                Executables = ExecutableManager,
                StaticData = StaticDataManager
            }
        };

        settingsDialog.ShowDialog();

        if (RestartOnClose)
        {
            Application.Current.MainWindow.Close();
        }

        // Refresh markers
        ReloadPlayerMarkers();

        // Refresh all replays
        await ReloadReplayList(settingsDialog.UpdateExecutablesOnClose).ConfigureAwait(true);
    }

    /// <summary>
    /// The function to call to refresh the list
    /// </summary>
    /// <returns></returns>
    public async Task ReloadReplayList(bool scanExecutables)
    {
        _log.Information($"Refreshing replay list...");

        // Clear previously loaded replays
        FileResults.Clear();
        PreviewReplays.Clear();

        StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageReplay") as string;
        StatusBarModel.Visible = true;
        StatusBarModel.ShowProgressBar = true;

        await Task.Run(() => _fileManager.PruneDatabaseEntries());

        // Discover and load replays into database, results are files that failed to load
        _ = await _fileManager.InitialLoadAsync().ConfigureAwait(true);

        // Load from database into our viewmodel
        int searchResults = -1;
        await Task.Run(() => (_, searchResults, _) = LoadReplaysFromDatabase(true));

        // Load thumbnails
        StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageThumbnails") as string;
        await LoadPreviewPlayerThumbnails();

        // Validate executables
        if (scanExecutables)
        {
            StatusBarModel.StatusMessage = Application.Current.TryFindResource("LoadingMessageExecutables") as string;
            await ExecutableManager.VerifyRegisteredExecutables().ConfigureAwait(true);
        }

        if (searchResults == -1)
        {
            StatusBarModel.Visible = false;
        }
        else
        {
            StatusBarModel.ShowProgressBar = false;
            StatusBarModel.StatusMessage = $"{PreviewReplays.Count} / {searchResults}";
        }
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

    public void OpenNewWindow(string replayPath)
    {
        _log.Information("Opening new window...");

        var singleWindow = new SingleReplayWindow(_log, Configuration, StaticDataManager, ExecutableManager, _fileManager, _player, true)
        {
            ReplayFileLocation = replayPath,
            DataContext = this
        };
        singleWindow.Show();
    }

    public void OpenReplayContainingFolder(string location)
    {
        _log.Information($"Opening replay file location {location}");

        if (string.IsNullOrEmpty(location)) { throw new ArgumentNullException(nameof(location)); }

        string selectArg = $"/select, \"{location}\"";
        _ = Process.Start("explorer.exe", selectArg);
    }

    public void ShowExportReplayDataWindow(ReplayPreview preview)
    {
        _log.Information($"Showing Export Dialog...");

        if (preview == null) { throw new ArgumentNullException(nameof(preview)); }
        if (preview.IsErrorReplay) { return; } // TODO

        // create transition collection
        var contentTransitions = new ModernWpf.Media.Animation.TransitionCollection
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
        var contentFrame = new ModernWpf.Controls.Frame()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            ContentTransitions = contentTransitions
        };

        // create default context values
        var exportDataContext = new ExportDataContext
        {
            PresetName = "< Unsaved Preset >",
            // attempt to get last saved preset name
            LastPreset = Configuration.Stash.TryGetString("LastExportPreset", out string lastPreset) ? lastPreset : string.Empty,
            ManualPlayerSelection = true,
            AlwaysIncludeMarked = false,
            IncludeAllPlayers = false,
            ReplayPreview = preview,
            Replay = FileResults[preview.Location].ReplayFile,
            ContentFrame = contentFrame,
            HideHeader = false,
            WindowTitleText = Application.Current.FindResource("ErdTitle") as string,
            Log = _log,
            StaticDataManager = StaticDataManager
        };

        var exportDialog = new ExportDataWindow()
        {
            Top = Application.Current.MainWindow.Top + 50,
            Left = Application.Current.MainWindow.Left + 50,
            DataContext = exportDataContext,
        };

        _ = exportDialog.ShowDialog();

        // save last preset to stash
        if (!string.IsNullOrEmpty((exportDialog.DataContext as ExportDataContext).LastPreset))
        {
            Configuration.Stash["LastExportPreset"] = (exportDialog.DataContext as ExportDataContext).LastPreset;
        }
    }

    public void ShowWelcomeWindow()
    {
        string welcomeFileFlag = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache/SKIPWELCOME");
        if (File.Exists(welcomeFileFlag))
        {
            _log.Information("Skipping welcome screen...");
            return;
        }

        // create transition collection
        var contentTransitions = new ModernWpf.Media.Animation.TransitionCollection
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
        var contentFrame = new ModernWpf.Controls.Frame()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            ContentTransitions = contentTransitions
        };

        var setupContext = new WelcomeSetupDataContext
        {
            ContentFrame = contentFrame,
            PageIndex = 0,
            PageCount = 5,
            DisableBackButton = true,
            RiotGamesPath = (string)Application.Current.TryFindResource("WswExecutablesHint"),
            Language = ApplicationLanguage.En,
            Log= _log
        };

        _log.Information(welcomeFileFlag);
        var welcomeDialog = new WelcomeSetupWindow()
        {
            Top = Application.Current.MainWindow.Top + 50,
            Left = Application.Current.MainWindow.Left + 50,
            DataContext = setupContext
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

    public void ApplyInitialSettings(WelcomeSetupDataContext initialSettings)
    {
        if (initialSettings == null) { throw new ArgumentNullException(nameof(initialSettings)); }

        if (!string.IsNullOrEmpty(initialSettings.ReplayPath) && Directory.Exists(initialSettings.ReplayPath))
        {
            // Only add the replay folder if it doesn't exist already. It really should be empty...
            if (!Configuration.ReplayFolders.Contains(initialSettings.ReplayPath))
            {
                Configuration.ReplayFolders.Add(initialSettings.ReplayPath);
            }
        }

        if (!string.IsNullOrEmpty(initialSettings.RiotGamesPath) && Directory.Exists(initialSettings.ReplayPath))
        {
            // Only add the executable folder if it doesn't exist already. It really should be empty...
            if (!ExecutableManager.Settings.SourceFolders.Contains(initialSettings.RiotGamesPath))
            {
                ExecutableManager.Settings.SourceFolders.Add(initialSettings.RiotGamesPath);
            }
        }

        if (initialSettings.Executables != null)
        {
            foreach (Executables.Old.Models.LeagueExecutable executable in initialSettings.Executables)
            {
                ExecutableManager.AddExecutable(executable);
            }
        }

        Configuration.Language = initialSettings.Language;
    }

    /// <summary>
    /// Function is called at the start of the program, checking for invalid replay paths
    /// </summary>
    public void ShowMissingReplayFoldersMessageBox()
    {
        // Check if replay paths are missing, if so remove them
        string[] missingPaths = Configuration.RemoveInvalidReplayPaths(_log);
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
        string errorMessage = null;
        try
        {
            string newReplayLocation = _fileManager.RenameReplay(replay, newText);

            preview.DisplayName = Path.GetFileName(newReplayLocation);
            preview.Location = newReplayLocation;
        }
        catch (Exception ex)
        {
            if (ex.Message == "{EMPTY ERROR}")
            {
                errorMessage = Application.Current.TryFindResource("RenameFlyoutEmptyError") as string;
            }
            else if (ex.Message == "{NOT FOUND ERROR}")
            {
                errorMessage = Application.Current.TryFindResource("RenameFlyoutNotFoundError") as string;
            }
            else
            {
                throw;
            }
        }

        return errorMessage;
    }

    public async Task DeleteReplayFile(ReplayPreview preview)
    {
        if (preview == null) { throw new ArgumentNullException(nameof(preview)); }

        _ = _fileManager.DeleteFile(FileResults[preview.Location]);

        await ReloadReplayList(false).ConfigureAwait(false);
    }

    /// <summary>
    /// Forward to file manager to delete replays
    /// </summary>
    public void ClearDeletedReplays()
    {
        _fileManager.ClearDeletedFiles();
    }

    /// <summary>
    /// Calculate size on disk for replay cache (database + search index)
    /// </summary>
    /// <returns></returns>
    public long CalculateReplayCacheSize()
    {
        var databaseInfo = new FileInfo(_fileManager.DatabasePath);
        var totalSize = databaseInfo.Length;

        // Calculate disk size of search index files
        var searchIndexInfo = new DirectoryInfo(_fileManager.SearchIndexPath);
        foreach (var file in searchIndexInfo.EnumerateFiles())
        {
            totalSize += file.Length;
        }

        return totalSize;
    }

    public void ClearCachedData()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        if (ClearReplayCacheOnClose)
        {
            _fileManager.DeleteDatabase();
            _fileManager.DeleteSearchIndex();
        }
    }
}

