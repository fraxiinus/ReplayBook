using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Data;
using Fraxiinus.ReplayBook.StaticData.Extensions;
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

        private readonly DataDragonClient _dataDragonClient;
        private readonly CommunityDragonClient _communityDragonClient;

        public ObservableStaticDataContext Context;

        public StaticDataManager(ObservableConfiguration config, string userAgent, RiZhi log)
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            _config = config;
            _log = log;

            _dataDragonClient = new DataDragonClient(_dataPath, config, userAgent, log);
            _communityDragonClient = new CommunityDragonClient(_dataPath, config, userAgent, log);

            Context = new ObservableStaticDataContext();
        }

        public async Task LoadIndexAsync()
        {
            // check if index file exists
            if (!File.Exists(Path.Combine(_dataPath, "index.json")))
            {
                _log.Information("index.json is missing");

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

        public async Task SaveIndexAsync()
        {
            await Context.SaveToJson(_dataPath);
        }

        public async Task DownloadImages(string patchVersion, StaticDataType types)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var paths = await _dataDragonClient.DownloadSpriteImages(patchVersion, StaticDataDefinitions.Item);
                targetBundle.ItemImageFiles.Clear();
                foreach (var path in paths)
                {
                    targetBundle.ItemImageFiles.Add(path);
                }

                // update last download date
                targetBundle.LastDownloadDate = DateTime.Now;
            }
            if (types.HasFlag(StaticDataType.Champion))
            {
                var paths = await _dataDragonClient.DownloadSpriteImages(patchVersion, StaticDataDefinitions.Champion);
                targetBundle.ChampionImageFiles.Clear();
                foreach (var path in paths)
                {
                    targetBundle.ChampionImageFiles.Add(path);
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

        public async Task DownloadRuneImages(string patchVersion, IEnumerable<RuneData> runeData)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            await _dataDragonClient.DownloadRuneImages(patchVersion, runeData);

            targetBundle.LastDownloadDate = DateTime.Now;
        }

        public async Task DownloadProperties(string patchVersion, StaticDataType types, string language)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var properties = await _dataDragonClient.DownloadPropertySet(patchVersion, StaticDataDefinitions.Item, language);
                var savedFile = await SavePropertySet(properties, patchVersion, StaticDataDefinitions.Item, language);
                var result = targetBundle.ItemDataFiles.TryAdd(patchVersion, savedFile);
                
                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }
            if (types.HasFlag(StaticDataType.Champion))
            {
                var properties = await _dataDragonClient.DownloadPropertySet(patchVersion, StaticDataDefinitions.Champion, language);
                var savedFile = await SavePropertySet(properties, patchVersion, StaticDataDefinitions.Champion, language);
                var result = targetBundle.ChampionDataFiles.TryAdd(language, savedFile);

                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }
            if (types.HasFlag(StaticDataType.Rune))
            {
                var properties = (List<RuneData>) await _dataDragonClient.DownloadPropertySet(patchVersion, StaticDataDefinitions.Rune, language);
                await _communityDragonClient.GetRuneStatDescriptions(properties, patchVersion, language);
                var savedFile = await SavePropertySet(properties, patchVersion, StaticDataDefinitions.Rune, language);
                var result = targetBundle.RuneDataFiles.TryAdd(language, savedFile);
                
                if (!result)
                {
                    _log.Warning($"Failed to add to dictionary: {language}, already exists?");
                }
            }

            // update last download date
            targetBundle.LastDownloadDate = DateTime.Now;
        }

        private async Task<string> SavePropertySet(IEnumerable<BaseStaticData> data, string patchVersion, string dataType, string language)
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
            var relativeDestination = Path.Combine(patchVersion, dataType.ToLower());
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
