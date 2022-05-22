using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Data;
using Fraxiinus.ReplayBook.StaticData.Extensions;
using Fraxiinus.ReplayBook.StaticData.Models;
using System.Drawing;
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

        public async Task GetPatchesIfOutdated()
        {
            if (!Context.KnownPatchNumbers.Any())
            {
                await RefreshPatches();
            }
        }

        public async Task RefreshPatches()
        {
            Context.KnownPatchNumbers.Clear();

            var allVersions = await _dataDragonClient.GetPatchesAsync();

            foreach (var version in allVersions)
            {
                Context.KnownPatchNumbers.Add(version);

                // no data past before this patch can be loaded.
                if (version == "7.22.1") break;
            }

            Context.LastPatchFetch = DateTime.Now;
        }

        public async Task<ItemData?> GetItemData(string itemId, ProgramLanguage language, string patchVersion)
        {
            var bundle = Context.GetBundle(patchVersion);
            try
            {
                return await bundle.GetItemData(language.GetRiotRegionCode(), itemId, _dataPath);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex}\ncould not load data for item: {itemId} - {patchVersion} - {language.GetRiotRegionCode()}");
                return null;
            }
        }

        public async Task<ChampionData?> GetChampionData(string championId, ProgramLanguage language, string patchVersion)
        {
            var bundle = Context.GetBundle(patchVersion);
            try
            {
                return await bundle.GetChampionData(language.GetRiotRegionCode(), championId, _dataPath);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex}\ncould not load data for champion: {championId} - {patchVersion} - {language.GetRiotRegionCode()}");
                return null;
            }
        }

        public async Task<RuneData?> GetRuneData(string runeId, ProgramLanguage language, string patchVersion)
        {
            var bundle = Context.GetBundle(patchVersion);
            try
            {
                return await bundle.GetRuneData(language.GetRiotRegionCode(), runeId, _dataPath);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex}\ncould not load data for rune: {runeId} - {patchVersion} - {language.GetRiotRegionCode()}");
                return null;
            }
        }

        public BitmapFrame? GetAtlasImage(string source, string patchVersion)
        {
            var bundle = Context.GetBundle(patchVersion);
            try
            {
                return bundle.GetAtlasImage(source, _dataPath);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex}\ncould not load image for: {source} - {patchVersion}");
                return null;
            }
        }

        public void DeleteBundle(string patchVersion)
        {
            Context.DeleteBundle(_dataPath, patchVersion);
        }

        public async Task DownloadImages(string patchVersion, StaticDataType types)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var paths = await _dataDragonClient.DownloadSpriteImages(patchVersion, StaticDataDefinitions.Item);
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
                var paths = await _dataDragonClient.DownloadSpriteImages(patchVersion, StaticDataDefinitions.Champion);
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

        public async Task DownloadRuneImages(string patchVersion, IEnumerable<RuneData> runeData)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            var paths = await _dataDragonClient.DownloadRuneImages(patchVersion, runeData);
            targetBundle.RuneImageFiles.Clear();
            foreach (var path in paths)
            {
                targetBundle.RuneImageFiles.Add(path);
            }

            targetBundle.LastDownloadDate = DateTime.Now;
        }

        public async Task DownloadRuneImages(string patchVersion, string language)
        {
            // get bundle for patch version
            var targetBundle = Context.GetBundle(patchVersion);

            // see if bundle has rune data already
            var (_, filePath) = targetBundle.RuneDataFiles.First(x => x.language == language);

            // read the rune data
            await using FileStream fileStream = File.OpenRead(Path.Combine(_dataPath, filePath));
            var runeData = JsonSerializer.Deserialize<IEnumerable<RuneData>>(fileStream)
                ?? throw new Exception("rune data load null");

            await DownloadRuneImages(patchVersion, runeData);
        }

        public async Task DownloadProperties(string patchVersion, StaticDataType types, string language)
        {
            var targetBundle = Context.GetBundle(patchVersion);

            // download items
            if (types.HasFlag(StaticDataType.Item))
            {
                var properties = await _dataDragonClient.DownloadPropertySet(patchVersion, StaticDataDefinitions.Item, language);
                var savedFile = await SavePropertySet(properties, patchVersion, StaticDataDefinitions.Item, language);
                var result = targetBundle.ItemDataFiles.TryAdd(language, savedFile);
                
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
