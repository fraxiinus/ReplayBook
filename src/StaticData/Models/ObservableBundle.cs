using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class ObservableBundle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _patch = string.Empty;
        public string Patch
        {
            get { return _patch; }
            set
            {
                _patch = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Patch)));
            }
        }

        private DateTimeOffset _lastDownloadDate;
        public DateTimeOffset LastDownloadDate
        {
            get { return _lastDownloadDate; }
            set
            {
                _lastDownloadDate = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(LastDownloadDate)));
            }
        }

        public ObservableCollection<string> ChampionImagePaths { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> ItemImagePaths { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<(string key, string relativePath)> RuneImageFiles { get; set; } = new ObservableCollection<(string key, string relativePath)>();

        public ObservableCollection<(string language, string relativePath)> ChampionDataFiles { get; set; } = new ObservableCollection<(string language, string relativePath)>();

        public ObservableCollection<(string language, string relativePath)> ItemDataFiles { get; set; } = new ObservableCollection<(string language, string relativePath)>();

        public ObservableCollection<(string language, string relativePath)> RuneDataFiles { get; set; } = new ObservableCollection<(string language, string relativePath)>();

        // Object dictionaries, language -> id -> data
        private Dictionary<string, Dictionary<string, ChampionData>> LoadedChampionData { get; set; } = new Dictionary<string, Dictionary<string, ChampionData>>();

        private Dictionary<string, Dictionary<string, ItemData>> LoadedItemData { get; set; } = new Dictionary<string, Dictionary<string, ItemData>>();

        private Dictionary<string, Dictionary<string, RuneData>> LoadedRuneData { get; set; } = new Dictionary<string, Dictionary<string, RuneData>>();

        // Key is file name
        private Dictionary<string, BitmapFrame> LoadedChampionImages { get; set; } = new Dictionary<string, BitmapFrame>();

        private Dictionary<string, BitmapFrame> LoadedItemImages { get; set; } = new Dictionary<string, BitmapFrame>();

        /// <summary>
        /// Returns <see cref="Image"/> object either from cache, or from disk
        /// </summary>
        /// <param name="imageSource"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public BitmapFrame GetAtlasImage(string imageSource, string dataPath)
        {
            if (imageSource.StartsWith("champion"))
            {
                // have we loaded this already?
                if (!LoadedChampionImages.ContainsKey(imageSource))
                {
                    // no, find the image number
                    var atlasNumber = FindAtlasNumber(imageSource, "champion");
                    // get the file path using the number
                    var atlasFilePath = ChampionImagePaths[atlasNumber];
                    var targetPath = Path.Combine(dataPath, atlasFilePath);

                    // load image from file
                    LoadedChampionImages[imageSource] = BitmapFrame.Create(new Uri(targetPath), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    return LoadedChampionImages[imageSource];
                }
                else
                {
                    return LoadedChampionImages[imageSource];
                }
            }
            else if (imageSource.StartsWith("item"))
            {
                // have we loaded this already?
                if (!LoadedItemImages.ContainsKey(imageSource))
                {
                    // no, find the image number
                    var atlasNumber = FindAtlasNumber(imageSource, "item");
                    // get the file path using the number
                    var atlasFilePath = ItemImagePaths[atlasNumber];
                    var targetPath = Path.Combine(dataPath, atlasFilePath);

                    // load image from file
                    LoadedItemImages[imageSource] = BitmapFrame.Create(new Uri(targetPath), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    return LoadedItemImages[imageSource];
                }
                else
                {
                    return LoadedItemImages[imageSource];
                }
            }
            else
            {
                throw new Exception($"Unknown image source {imageSource}");
            }
        }

        /// <summary>
        /// Returns <see cref="ChampionData"/> object either from cache, or from disk.
        /// All champions are loaded to cache at once.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="championId"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ChampionData> GetChampionData(string language, string championId, string dataPath)
        {
            // find the file we are interested in
            var championFilePath = ChampionDataFiles
                .Where(x => x.language == language)
                .Select(x => x.relativePath)
                .FirstOrDefault()
                ?? throw new Exception($"language is not available: {language}");

            // language exists, see if we loaded it already
            if (!LoadedChampionData.ContainsKey(language))
            {
                // not loaded, read file
                var targetFile = Path.Combine(dataPath, championFilePath);

                LoadedChampionData[language] = await GetFileData<ChampionData>(targetFile);
            }

            return LoadedChampionData[language][championId];
        }

        /// <summary>
        /// Returns <see cref="ItemData"/> object either from cache, or from disk.
        /// All items are loaded to cache at once.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="itemId"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ItemData> GetItemData(string language, string itemId, string dataPath)
        {
            // find the file we are interested in
            var itemFilePath = ItemDataFiles
                .Where(x => x.language == language)
                .Select(x => x.relativePath)
                .FirstOrDefault()
                ?? throw new Exception($"language is not available: {language}");

            // language exists, see if we loaded it already
            if (!LoadedItemData.ContainsKey(language))
            {
                // not loaded, read file
                var targetFile = Path.Combine(dataPath, itemFilePath);

                LoadedItemData[language] = await GetFileData<ItemData>(targetFile);
            }

            return LoadedItemData[language][itemId];
        }

        /// <summary>
        /// Returns <see cref="RuneData"/> object either from cache, or from disk.
        /// All items are loaded to cache at once.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="runeId"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<RuneData> GetRuneData(string language, string runeId, string dataPath)
        {
            // find the file we are interested in
            var runeFilePath = RuneDataFiles
                .Where(x => x.language == language)
                .Select(x => x.relativePath)
                .FirstOrDefault()
                ?? throw new Exception($"language is not available: {language}");

            // language exists, see if we loaded it already
            if (!LoadedRuneData.ContainsKey(language))
            {
                // not loaded, read file
                var targetFile = Path.Combine(dataPath, runeFilePath);
                
                LoadedRuneData[language] = await GetFileData<RuneData>(targetFile);
            }

            return LoadedRuneData[language][runeId];
        }

        private static async Task<Dictionary<string, T>> GetFileData<T>(string filePath)
        {
            if (!(typeof(T) == typeof(ChampionData) || typeof(T) == typeof(ItemData) || typeof(T) == typeof(RuneData)))
            {
                throw new Exception($"unsupported type: {typeof(T)} vs {typeof(ChampionData)}");
            }
            if (!File.Exists(filePath))
            {
                throw new Exception($"file not found {filePath}");
            }

            using FileStream stream = File.OpenRead(filePath);
            using JsonDocument jsonDocument = await JsonDocument.ParseAsync(stream);

            var dataList = jsonDocument.Deserialize<IEnumerable<T>>()
                    ?? throw new Exception($"failed {typeof(T)} deserialization: {filePath}");

            var resultDict = new Dictionary<string, T>();
            foreach (var data in dataList)
            {
                var baseData = data as BaseStaticData
                    ?? throw new Exception($"failed reading data as base object");

                resultDict.Add(baseData.Id, data);
            }

            return resultDict;
        }

        private static int FindAtlasNumber(string atlasSource, string type)
        {
            var atlasFileName = atlasSource.Replace(type, "");
            var atlasNumber = int.Parse(atlasFileName[..1]);

            return atlasNumber;
        }

        public async Task<string> SaveToJson(string dataPath)
        {
            var jsonModel = new Bundle()
            {
                Patch = Patch,
                LastDownloadDate = LastDownloadDate,
            };

            jsonModel.ChampionImageFiles.AddRange(ChampionImagePaths);
            jsonModel.ItemImageFiles.AddRange(ItemImagePaths);

            foreach (var (key, relativePath) in RuneImageFiles)
            {
                jsonModel.RuneImageFiles.Add(key, relativePath);
            }
            foreach (var (language, relativePath) in ChampionDataFiles)
            {
                jsonModel.ChampionDataFiles.Add(language, relativePath);
            }
            foreach (var (language, relativePath) in ItemDataFiles)
            {
                jsonModel.ItemDataFiles.Add(language, relativePath);
            }
            foreach (var (language, relativePath) in RuneDataFiles)
            {
                jsonModel.RuneDataFiles.Add(language, relativePath);
            }

            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            var savePath = Path.Combine(dataPath, Patch, "bundle.json");
            await using FileStream bundleFile = File.Create(savePath);
            await JsonSerializer.SerializeAsync(bundleFile, jsonModel, serializerOptions);

            return Path.Combine(Patch, "bundle.json");
        }

        public static async Task<ObservableBundle> CreateFromJson(string bundleFilePath)
        {
            var result = new ObservableBundle();

            // load bundle data
            await using FileStream bundleFile = File.OpenRead(bundleFilePath);
            var jsonModel = await JsonSerializer.DeserializeAsync<Bundle>(bundleFile)
                ?? throw new Exception("bundle load null");

            result.Patch = jsonModel.Patch;
            result.LastDownloadDate = jsonModel.LastDownloadDate;
            foreach (var championImageFile in jsonModel.ChampionImageFiles)
            {
                result.ChampionImagePaths.Add(championImageFile);
            }
            foreach (var itemImageFile in jsonModel.ItemImageFiles)
            {
                result.ItemImagePaths.Add(itemImageFile);
            }
            foreach (var (key, relativePath) in jsonModel.RuneImageFiles)
            {
                result.RuneImageFiles.Add((key, relativePath));
            }
            foreach (var (language, relativePath) in jsonModel.ChampionDataFiles)
            {
                result.ChampionDataFiles.Add((language, relativePath));
            }
            foreach (var (language, relativePath) in jsonModel.ItemDataFiles)
            {
                result.ItemDataFiles.Add((language, relativePath));
            }
            foreach (var (language, relativePath) in jsonModel.RuneDataFiles)
            {
                result.RuneDataFiles.Add((language, relativePath));
            }

            return result;
        }

    }
}
