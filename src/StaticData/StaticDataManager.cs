using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Data;
using Fraxiinus.ReplayBook.StaticData.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData
{
    public class StaticDataManager
    {
        private readonly RiZhi _log;
        private readonly ObservableConfiguration _config;
        private readonly string _dataPath;

        private BundleIndex _bundleIndex;

        private readonly Dictionary<string, Bundle> _bundles;

        private readonly DataDragonClient _dataDragonClient;
        private readonly CommunityDragonClient _communityDragonClient;

        public StaticDataManager(ObservableConfiguration config, string userAgent, RiZhi log)
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            _bundleIndex = new BundleIndex
            {
                DownloadedBundles = new Dictionary<string, string>()
            };
            _bundles = new Dictionary<string, Bundle>();
            _config = config;
            _log = log;

            _dataDragonClient = new DataDragonClient(_dataPath, config, userAgent, log);
            _communityDragonClient = new CommunityDragonClient(_dataPath, config, userAgent, log);
        }

        public async Task LoadIndexAsync()
        {
            // clear any loaded index data
            _bundleIndex.DownloadedBundles.Clear();
            _bundles.Clear();

            // check if index file exists
            if (!File.Exists(Path.Combine(_dataPath, "index.json")))
            {
                _log.Information("index.json is missing");

                // re-index existing bundles
                foreach (var existingBundle in Directory.EnumerateFiles(_dataPath, "bundle.json", SearchOption.AllDirectories))
                {
                    // load bundle data
                    await using FileStream bundleFile = File.OpenRead(Path.Combine(_dataPath, existingBundle));
                    var bundleData = await JsonSerializer.DeserializeAsync<Bundle>(bundleFile) ?? throw new Exception("bundle load null");
                    _log.Information($"Discovered bundle {bundleData.Patch}");
                    // add discovered bundle to bundle index
                    _bundleIndex.DownloadedBundles.Add(bundleData.Patch, existingBundle);
                    // add loaded bundle to bundle data
                    _bundles.Add(existingBundle, bundleData);
                }

                _log.Information($"Found and loaded {_bundles.Count} bundles");

                return;
            }

            // load index data to _bundleIndex
            await using FileStream indexFile = File.OpenRead(Path.Combine(_dataPath, "index.json"));
            _bundleIndex = await JsonSerializer.DeserializeAsync<BundleIndex>(indexFile)
                ?? throw new Exception("index.json load null");
            _log.Information($"Index claims {_bundleIndex.DownloadedBundles.Count} bundles");

            // load bundles defined in index
            var deleteFromIndex = new List<string>();
            foreach (var bundleKeyValue in _bundleIndex.DownloadedBundles)
            {
                // check if index file exists
                if (!File.Exists(Path.Combine(_dataPath, bundleKeyValue.Value)))
                {
                    // mark key for deletion
                    _log.Information($"bundle {bundleKeyValue.Key} is missing: {bundleKeyValue.Value}");
                    deleteFromIndex.Add(bundleKeyValue.Key);
                    continue;
                }

                // load bundle data
                await using FileStream bundleFile = File.OpenRead(Path.Combine(_dataPath, bundleKeyValue.Value));
                _bundles[bundleKeyValue.Key] = await JsonSerializer.DeserializeAsync<Bundle>(bundleFile)
                    ?? throw new Exception("bundle load null");
                _log.Information($"Loaded bundle {bundleKeyValue.Key}");
            }

            foreach (var deletedBundle in deleteFromIndex)
            {
                _bundleIndex.DownloadedBundles.Remove(deletedBundle);
            }
        }

        public async Task SaveIndexAsync()
        {
            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            // create or overwrite index
            await using FileStream indexFile = File.Create(Path.Combine(_dataPath, "index.json"));
            await JsonSerializer.SerializeAsync(indexFile, _bundleIndex, serializerOptions);

            // create or overwrite bundles
            foreach (var bundleKeyValue in _bundleIndex.DownloadedBundles)
            {
                // if a bundle exists, then the folder should already exist (data downloaded)
                await using FileStream bundleFile = File.Create(Path.Combine(_dataPath, bundleKeyValue.Value));
                await JsonSerializer.SerializeAsync(bundleFile, _bundles[bundleKeyValue.Key], serializerOptions);
            }
        }

        public async Task DownloadImageData(string version, StaticDataType types)
        {
            Bundle targetBundle = GetBundle(version);

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var paths = await _dataDragonClient.DownloadSpriteImages(version, StaticDataDefinitions.Item);
                targetBundle.ItemImageFiles.Clear();
                targetBundle.ItemImageFiles.AddRange(paths);
            }
            if (types.HasFlag(StaticDataType.Champion))
            {
                var paths = await _dataDragonClient.DownloadSpriteImages(version, StaticDataDefinitions.Champion);
                targetBundle.ChampionImageFiles.Clear();
                targetBundle.ChampionImageFiles.AddRange(paths);
            }
            if (types.HasFlag(StaticDataType.Rune))
            {
                // TODO
            }

            // update last download date
            targetBundle.LastDownloadDate = DateTime.Now;
        }

        public async Task DownloadProperties(string version, StaticDataType types, string language)
        {
            Bundle targetBundle = GetBundle(version);

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var properties = await _dataDragonClient.DownloadPropertySet(version, StaticDataDefinitions.Item, language);
                var savedFile = await SavePropertySet(properties, version, StaticDataDefinitions.Item, language);
                var result = targetBundle.ItemDataFiles.TryAdd(language, savedFile);
                
                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }
            if (types.HasFlag(StaticDataType.Champion))
            {
                var properties = await _dataDragonClient.DownloadPropertySet(version, StaticDataDefinitions.Champion, language);
                var savedFile = await SavePropertySet(properties, version, StaticDataDefinitions.Champion, language);
                var result = targetBundle.ChampionDataFiles.TryAdd(language, savedFile);

                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }
            if (types.HasFlag(StaticDataType.Rune))
            {
                var properties = (List<RuneData>) await _dataDragonClient.DownloadPropertySet(version, StaticDataDefinitions.Rune, language);
                await _communityDragonClient.GetRuneStatDescriptions(properties, version, language);
                var savedFile = await SavePropertySet(properties, version, StaticDataDefinitions.Rune, language);
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
        /// Creates or returns existing Bundle
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private Bundle GetBundle(string version)
        {
            // try to get an existing bundle, create new otherwise
            if (!_bundles.TryGetValue(version, out var targetBundle))
            {
                targetBundle = new Bundle
                {
                    Patch = version
                };
                // save bundle to bundle lookup
                _bundles[version] = targetBundle;
                // save bundle to index (for saving/loading to file)
                _bundleIndex.DownloadedBundles[version] = Path.Combine(version, "bundle.json");
            }

            return targetBundle;
        }

        private async Task<string> SavePropertySet(IEnumerable<BaseStaticData> data, string version, string dataType, string language)
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
            var relativeDestination = Path.Combine(version, dataType.ToLower());
            var destinationFolder = Path.Combine(_dataPath, relativeDestination);
            Directory.CreateDirectory(destinationFolder);

            // create or overwrite index
            var destinationFile = Path.Combine(destinationFolder, $"{language}.data.json");
            await using FileStream indexFile = File.Create(Path.Combine(destinationFolder, destinationFile));
            if (dataType == StaticDataDefinitions.Rune)
            {
                await JsonSerializer.SerializeAsync(indexFile, (IEnumerable<RuneData>) data, serializerOptions);
            }
            else
            {
                await JsonSerializer.SerializeAsync(indexFile, data, serializerOptions);
            }

            return Path.Combine(relativeDestination, $"{language}.data.json");
        }
    }
}
