using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Data;
using Fraxiinus.ReplayBook.StaticData.Extensions;
using Fraxiinus.ReplayBook.StaticData.Models;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace Fraxiinus.ReplayBook.StaticData
{
    public class StaticDataManager
    {
        private readonly RiZhi _log;
        private readonly ObservableConfiguration _config;
        private readonly string _dataPath;

        private readonly DataDragonClient _dataDragonClient;
        private readonly CommunityDragonClient _communityDragonClient;

        public ObservableStaticDataContext Context { get; set; }

        public StaticDataManager(ObservableConfiguration config, string userAgent, RiZhi log)
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            _config = config;
            _log = log;

            _dataDragonClient = new DataDragonClient(_dataPath, config, userAgent, log);
            _communityDragonClient = new CommunityDragonClient(_dataPath, config, userAgent, log);

            Context = new ObservableStaticDataContext();
        }

        /// <summary>
        /// Load all static data information, must be done first.
        /// </summary>
        /// <returns></returns>
        public async Task LoadIndexAsync()
        {
            // check if index file exists
            if (!File.Exists(Path.Combine(_dataPath, "index.json")))
            {
                _log.Information("index.json is missing");

                if (!Directory.Exists(_dataPath))
                {
                    Directory.CreateDirectory(_dataPath);
                }

                // re-index existing bundles
                foreach (var existingBundle in Directory.EnumerateFiles(_dataPath, "bundle.json", SearchOption.AllDirectories))
                {
                    // load bundle data
                    _log.Information($"Discovered bundle {existingBundle}");

                    var newBundle = await ObservableBundle.CreateFromJson(existingBundle);

                    // add bundle to new index
                    Context.Bundles.Add(newBundle);
                }

                _log.Information($"Found and loaded {Context.Bundles.Count} existing bundles");

                return;
            }

            // load index data to _bundleIndex
            Context = await ObservableStaticDataContext.CreateFromJson(_dataPath, _log);
            _log.Information($"Found index, loaded {Context.Bundles.Count} bundles");
        }

        /// <summary>
        /// Saves all static data information
        /// </summary>
        /// <returns></returns>
        public async Task SaveIndexAsync()
        {
            await Context.SaveToJson(_dataPath);
        }

        /// <summary>
        /// Downloads patches if there are none loaded.
        /// </summary>
        /// <returns></returns>
        public async Task GetPatchesIfOutdated(CancellationToken cancellationToken = default)
        {
            if (!Context.KnownPatchNumbers.Any())
            {
                await RefreshPatches(cancellationToken);
            }
        }

        /// <summary>
        /// Downloads and overwrites patches list
        /// </summary>
        /// <returns></returns>
        public async Task RefreshPatches(CancellationToken cancellationToken = default)
        {
            Context.KnownPatchNumbers.Clear();

            var allVersions = await _dataDragonClient.GetPatchesAsync(cancellationToken);

            foreach (var version in allVersions)
            {
                Context.KnownPatchNumbers.Add(version);

                // no data past before this patch can be loaded.
                if (version == "7.22.1") break;
            }

            Context.LastPatchFetch = DateTime.Now;
        }

        /// <summary>
        /// Does a bundle matching the given patch version exist?
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <returns></returns>
        public bool DoesBundleExist(string patchVersion)
        {
            return Context.GetBundle(patchVersion, false) != null;
        }

        public string GetDataPath() => _dataPath;

        /// <summary>
        /// Attempts to get property set that matches input arguments.
        /// If language is not found, returns first available language as backup.
        /// Returns null if no data at all for the patch is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="patchVersion"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public async Task<T?> GetProperties<T>(string id, string patchVersion, ApplicationLanguage language) where T : IStaticProperties
        {
            // The FiddleSticks Hack(tm)
            if (id == "FiddleSticks")
            {
                id = "Fiddlesticks";
            } 
            else if (string.IsNullOrEmpty(id) || id == "0")
            {
                _log.Warning($"Empty/Zero id given: {patchVersion} - {language.GetRiotRegionCode()}");
                return default;
            }

            var bundle = Context.GetBundle(patchVersion, false);
            // bundle with patch does not actually exist, there is no data
            if (bundle == null)
            {
                bundle = Context.GetAdjacentDownloadedBundle(patchVersion);
            }

            if (bundle == null)
            {
                _log.Warning($"could not find bundle for properties: {id} - {patchVersion} - {language.GetRiotRegionCode()}");
                return default;
            }

            try
            {
                // throws if language not available
                return (T) await bundle.GetProperties<T>(id, _dataPath, language.GetRiotRegionCode());
            }
            catch (Exception)
            {
                // _log.Error($"could not load static data, loading backup: {id} - {patchVersion} - {language.GetRiotRegionCode()} - {ex}");

                try
                {
                    return (T) await bundle.GetProperties<T>(id, _dataPath);
                }
                catch (Exception exBackup)
                {
                    _log.Error($"could not load any static data: {id} - {patchVersion} - {language.GetRiotRegionCode()} - {exBackup}");
                    return default;
                }
            }
        }

        /// <summary>
        /// Given atlas source name and patch, returns <see cref="BitmapFrame"/>, null if nothing found
        /// </summary>
        /// <param name="source"></param>
        /// <param name="patchVersion"></param>
        /// <returns></returns>
        public BitmapFrame? GetAtlasImage(string source, string patchVersion)
        {
            var bundle = Context.GetBundle(patchVersion, false);
            
            // bundle with patch does not actually exist, there is no data
            if (bundle == null)
            {
                bundle = Context.GetAdjacentDownloadedBundle(patchVersion);
            }

            if (bundle == null)
            {
                _log.Error($"could not find bundle for image: {source} - {patchVersion}");
                return null;
            }

            try
            {
                return bundle.GetAtlasImage(source, _dataPath);
            }
            catch (Exception ex)
            {
                _log.Error($"could not load image for: {source} - {patchVersion} - {ex}");
                return null;
            }
        }

        public string? GetRuneImagePath(string key, string patchVersion)
        {
            var bundle = Context.GetBundle(patchVersion, false);

            if (bundle == null)
            {
                bundle = Context.GetAdjacentDownloadedBundle(patchVersion);
            }

            if (bundle == null)
            {
                _log.Error($"could not find bundle for image: {key} - {patchVersion}");
                return null;
            }

            var runeRelPath = bundle.RuneImageFiles
                .Where(x => x.key == key)
                .Select(x => x.relativePath)
                .FirstOrDefault();

            // Return full path, or null
            return runeRelPath != null
                ? Path.Combine(_dataPath, runeRelPath)
                : null;
        }

        public void DeleteBundle(string patchVersion)
        {
            Context.DeleteBundle(_dataPath, patchVersion);
        }

        /// <summary>
        /// Downloads all images for a given static data type
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public async Task DownloadImages(string patchVersion, StaticDataType types, CancellationToken cancellationToken = default)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            if (targetBundle == null)
            {
                throw new Exception("Got a null bundle when request should not return null");
            }

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var paths = await _dataDragonClient.DownloadSpriteImages(patchVersion, StaticDataDefinitions.Item, cancellationToken);
                targetBundle.ItemImagePaths.Clear();
                foreach (var path in paths)
                {
                    targetBundle.ItemImagePaths.Add(path);
                }

                // update last download date
                targetBundle.LastDownloadDate = DateTime.Now;
            }
            if (types.HasFlag(StaticDataType.Champion))
            {
                var paths = await _dataDragonClient.DownloadSpriteImages(patchVersion, StaticDataDefinitions.Champion, cancellationToken);
                targetBundle.ChampionImagePaths.Clear();
                foreach (var path in paths)
                {
                    targetBundle.ChampionImagePaths.Add(path);
                }

                // update last download date
                targetBundle.LastDownloadDate = DateTime.Now;
            }
            if (types.HasFlag(StaticDataType.Rune))
            {
                // Cannot download runes here,
                // we need runes property set!
                _log.Warning("Runes cannot be downloaded with this function, use rune specific function");
            }
        }

        /// <summary>
        /// Downloads rune images if you know patch and have the RuneData
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <param name="runeData"></param>
        /// <returns></returns>
        public async Task DownloadRuneImages(string patchVersion, IEnumerable<RuneProperties> runeData, CancellationToken cancellationToken = default)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            if (targetBundle == null)
            {
                throw new Exception("Got a null bundle when request should not return null");
            }

            var paths = await _dataDragonClient.DownloadRuneImages(patchVersion, runeData, cancellationToken);
            targetBundle.RuneImageFiles.Clear();
            foreach (var path in paths)
            {
                targetBundle.RuneImageFiles.Add(path);
            }

            targetBundle.LastDownloadDate = DateTime.Now;
        }

        /// <summary>
        /// Downloads rune images if you know patch and language
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DownloadRuneImages(string patchVersion, string language, CancellationToken cancellationToken = default)
        {
            // get bundle for patch version
            var targetBundle = Context.GetBundle(patchVersion);

            if (targetBundle == null)
            {
                throw new Exception("Got a null bundle when request should not return null");
            }

            // see if bundle has rune data already
            var (_, filePath) = targetBundle.RuneDataFiles.First(x => x.language == language);

            // read the rune data
            await using FileStream fileStream = File.OpenRead(Path.Combine(_dataPath, filePath));
            var runeData = await JsonSerializer.DeserializeAsync<IEnumerable<RuneProperties>>(fileStream, cancellationToken: cancellationToken)
                ?? throw new Exception("rune data load null");

            await DownloadRuneImages(patchVersion, runeData, cancellationToken);
        }

        /// <summary>
        /// Download 
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <param name="types"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public async Task DownloadProperties(string patchVersion, StaticDataType types, string language, CancellationToken cancellationToken = default)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            if (targetBundle == null)
            {
                throw new Exception("Got a null bundle when request should not return null");
            }

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var properties = await _dataDragonClient.DownloadPropertySet(patchVersion, StaticDataDefinitions.Item, language, cancellationToken);
                var savedFile = await SavePropertySet(properties, patchVersion, StaticDataDefinitions.Item, language, cancellationToken);
                var result = targetBundle.ItemDataFiles.TryAdd(language, savedFile);
                
                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }
            if (types.HasFlag(StaticDataType.Champion))
            {
                var properties = await _dataDragonClient.DownloadPropertySet(patchVersion, StaticDataDefinitions.Champion, language, cancellationToken);
                var savedFile = await SavePropertySet(properties, patchVersion, StaticDataDefinitions.Champion, language, cancellationToken);
                var result = targetBundle.ChampionDataFiles.TryAdd(language, savedFile);

                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }
            if (types.HasFlag(StaticDataType.Rune))
            {
                var properties = (List<RuneProperties>) await _dataDragonClient.DownloadPropertySet(patchVersion, StaticDataDefinitions.Rune, language, cancellationToken);
                await _communityDragonClient.GetRuneStatDescriptions(properties, patchVersion, language, cancellationToken);
                var savedFile = await SavePropertySet(properties, patchVersion, StaticDataDefinitions.Rune, language, cancellationToken);
                var result = targetBundle.RuneDataFiles.TryAdd(language, savedFile);
                
                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }

            // update last download date
            targetBundle.LastDownloadDate = DateTime.Now;
        }

        /// <summary>
        /// Calculates disk usage for all patches, or specified patch.
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <returns></returns>
        public async Task<long> CalculateDiskUsage(string? patchVersion = null)
        {
            string targetPath = _dataPath;
            if (string.IsNullOrEmpty(patchVersion))
            {
                // do nothing
            }
            else if (Context.Bundles.FirstOrDefault(x => x.Patch == patchVersion) != null)
            {
                targetPath = Path.Combine(_dataPath, patchVersion);
            }
            else
            {
                _log.Error($"Failed to calculate disk usage for patch: {patchVersion}");
                return -1;
            }

            var dataInfo = new DirectoryInfo(targetPath);
            long dataTotal = !dataInfo.Exists ? 0L : await Task.Run(() => dataInfo.EnumerateFiles("", SearchOption.AllDirectories).Sum(file => file.Length)).ConfigureAwait(true);
            return dataTotal;
        }

        /// <summary>
        /// Save property set to file.
        /// </summary>
        /// <param name="propertySet"></param>
        /// <param name="patchVersion"></param>
        /// <param name="dataType"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private async Task<string> SavePropertySet(IEnumerable<BaseStaticProperties> propertySet, string patchVersion, string dataType, string language, CancellationToken cancellationToken = default)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                // data is not being presented in HTML, should be safe!
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            // Make sure destination exists
            var relativeDestination = Path.Combine(patchVersion, dataType.ToLowerInvariant());
            var destinationFolder = Path.Combine(_dataPath, relativeDestination);
            Directory.CreateDirectory(destinationFolder);

            // create or overwrite index
            var destinationFile = Path.Combine(destinationFolder, $"{language}.data.json");
            await using FileStream indexFile = File.Create(Path.Combine(destinationFolder, destinationFile));
            if (dataType == StaticDataDefinitions.Rune)
            {
                await JsonSerializer.SerializeAsync(indexFile, (IEnumerable<RuneProperties>) propertySet, serializerOptions, cancellationToken);
            }
            else
            {
                await JsonSerializer.SerializeAsync(indexFile, propertySet, serializerOptions, cancellationToken);
            }

            return Path.Combine(relativeDestination, $"{language}.data.json");
        }
    
    }
}
