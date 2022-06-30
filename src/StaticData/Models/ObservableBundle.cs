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
        private Dictionary<string, Dictionary<string, IStaticProperties>> LoadedChampionData { get; set; } = new Dictionary<string, Dictionary<string, IStaticProperties>>();

        private Dictionary<string, Dictionary<string, IStaticProperties>> LoadedItemData { get; set; } = new Dictionary<string, Dictionary<string, IStaticProperties>>();

        private Dictionary<string, Dictionary<string, IStaticProperties>> LoadedRuneData { get; set; } = new Dictionary<string, Dictionary<string, IStaticProperties>>();

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
        /// Returns <see cref="IStaticProperties"/> object either from cache, or from disk.
        /// All data sets are loaded to cache at once. Null language to get first available.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="championId"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IStaticProperties> GetProperties<T>(string id, string dataPath, string? language = null) where T : IStaticProperties
        {
            string dataFilePath;
            if (language == null)
            {
                var dataFile = GetDataFileCollection<T>().FirstOrDefault();

                if (dataFile.Equals(default(ValueTuple<string, string>)))
                {
                    throw new Exception($"no static data available: {id} - {Patch}");
                }

                language = dataFile.language;
                dataFilePath = dataFile.relativePath;
            }
            else
            {
                // find the file we are interested in
                dataFilePath = GetDataFileCollection<T>()
                    .Where(x => x.language == language)
                    .Select(x => x.relativePath)
                    .FirstOrDefault()
                    ?? throw new Exception($"language is not available: {id} - {Patch} - {language}");
            }

            var loadedDataDictionary = GetLoadedDataDictionary<T>();

            // language exists, see if we loaded it already
            if (!loadedDataDictionary.ContainsKey(language))
            {
                // not loaded, read file
                var targetFile = Path.Combine(dataPath, dataFilePath);

                loadedDataDictionary[language] = await GetFileData<T>(targetFile);
            }

            return loadedDataDictionary[language][id];
        }

        /// <summary>
        /// Returns correct collection for IStaticData type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private ObservableCollection<(string language, string relativePath)> GetDataFileCollection<T>() where T : IStaticProperties
        {
            if (typeof(T) == typeof(ChampionProperties))
            {
                return ChampionDataFiles;
            }
            else if (typeof(T) == typeof(ItemProperties))
            {
                return ItemDataFiles;
            }
            else if (typeof(T) == typeof(RuneProperties))
            {
                return RuneDataFiles;
            }
            else
            {
                throw new Exception($"{typeof(T)} is not supported");
            }
        }

        /// <summary>
        /// Returns correct dictionary for IStaticData type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Dictionary<string, Dictionary<string, IStaticProperties>> GetLoadedDataDictionary<T>() where T : IStaticProperties
        {
            if (typeof(T) == typeof(ChampionProperties))
            {
                return LoadedChampionData;
            }
            else if (typeof(T) == typeof(ItemProperties))
            {
                return LoadedItemData;
            }
            else if (typeof(T) == typeof(RuneProperties))
            {
                return LoadedRuneData;
            }
            else
            {
                throw new Exception($"{typeof(T)} is not supported");
            }
        }

        /// <summary>
        /// Helper function.
        /// Given file path to property set, deserialize and load to memory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task<Dictionary<string, IStaticProperties>> GetFileData<T>(string filePath) where T : IStaticProperties
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"file not found {filePath}");
            }

            using FileStream stream = File.OpenRead(filePath);
            using JsonDocument jsonDocument = await JsonDocument.ParseAsync(stream);

            var dataList = jsonDocument.Deserialize<IEnumerable<T>>()
                    ?? throw new Exception($"failed {typeof(T)} deserialization: {filePath}");

            var resultDict = new Dictionary<string, IStaticProperties>();
            foreach (var data in dataList)
            {
                var baseData = data as BaseStaticProperties
                    ?? throw new Exception($"failed reading data as base object");

                resultDict.Add(baseData.Id, data);
            }

            return resultDict;
        }

        /// <summary>
        /// Helper function.
        /// Returns the atlas number within the atlasSource name.
        /// </summary>
        /// <param name="atlasSource"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int FindAtlasNumber(string atlasSource, string type)
        {
            var atlasFileName = atlasSource.Replace(type, "");
            var atlasNumber = int.Parse(atlasFileName[..1]);

            return atlasNumber;
        }

        /// <summary>
        /// Save this object's data to file.
        /// </summary>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        public async Task<string?> SaveToJson(string dataPath)
        {
            // Do not save json for blank patch
            if (ChampionImagePaths.Count == 0 && ItemImagePaths.Count == 0
                && RuneImageFiles.Count == 0 && ChampionDataFiles.Count == 0
                && ItemDataFiles.Count == 0 && RuneDataFiles.Count == 0)
            {
                return null;
            }

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

        /// <summary>
        /// Load data from JSON file.
        /// </summary>
        /// <param name="bundleFilePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
