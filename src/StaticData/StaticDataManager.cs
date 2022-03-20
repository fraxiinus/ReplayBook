using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Data;
using Fraxiinus.ReplayBook.StaticData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.StaticData
{
    public class StaticDataManager
    {
        private readonly RiZhi _log;
        private readonly ObservableConfiguration _config;
        private readonly string _userAgent;
        private readonly string _dataPath;

        private BundleIndex _bundleIndex;

        private readonly Dictionary<string, Bundle> _bundles;

        private readonly DataDragonClient _dataDragonClient;

        public StaticDataManager(ObservableConfiguration config, string userAgent, RiZhi log)
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            _bundleIndex = new BundleIndex
            {
                DownloadedBundles = new Dictionary<string, string>()
            };
            _bundles = new Dictionary<string, Bundle>();
            _config = config;
            _userAgent = userAgent;
            _log = log;

            _dataDragonClient = new DataDragonClient(_dataPath, config, userAgent, log);
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
                    await using FileStream bundleFile = File.OpenRead(existingBundle);
                    var bundleData = await JsonSerializer.DeserializeAsync<Bundle>(bundleFile) ?? throw new Exception("bundle load null");
                    _log.Information($"Discovered bundle {bundleData.Patch}");
                    // add discovered bundle to bundle index
                    _bundleIndex.DownloadedBundles.Add(existingBundle, bundleData.Patch);
                    // add loaded bundle to bundle data
                    _bundles.Add(existingBundle, bundleData);
                }

                _log.Information($"Found and loaded {_bundles.Count} bundles");

                return;
            }

            // load index data to _bundleIndex
            await using FileStream indexFile = File.OpenRead(Path.Combine(_dataPath, "index.json"));
            _bundleIndex = await JsonSerializer.DeserializeAsync<BundleIndex>(indexFile) ?? throw new Exception("index.json load null");
            _log.Information($"Index claims {_bundleIndex.DownloadedBundles.Count} bundles");

            // load bundles defined in index
            var deleteFromIndex = new List<string>();
            foreach (var bundleKeyValue in _bundleIndex.DownloadedBundles)
            {
                // check if index file exists
                if (!File.Exists(bundleKeyValue.Value))
                {
                    // mark key for deletion
                    _log.Information($"bundle {bundleKeyValue.Key} is missing: {bundleKeyValue.Value}");
                    deleteFromIndex.Add(bundleKeyValue.Key);
                    continue;
                }

                // load bundle data
                await using FileStream bundleFile = File.OpenRead(bundleKeyValue.Value);
                _bundles[bundleKeyValue.Key] = await JsonSerializer.DeserializeAsync<Bundle>(bundleFile) ?? throw new Exception("bundle load null");
                _log.Information($"Loaded bundle {bundleKeyValue.Key}");
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
                await using FileStream bundleFile = File.Create(bundleKeyValue.Value);
                await JsonSerializer.SerializeAsync(bundleFile, _bundles[bundleKeyValue.Key], serializerOptions);
            }
        }

        public async Task DownloadData(string version, StaticDataType types)
        {
            var paths = await _dataDragonClient.GetSpriteImages(version, types);

            foreach (var path in paths)
            {
                _log.Information(path);
            }
        }
    }
}
