using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class ObservableBundle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Patch { get; set; } = string.Empty;

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

        public ObservableCollection<string> ChampionImageFiles { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> ItemImageFiles { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<(string language, string relativePath)> ChampionDataFiles { get; set; } = new ObservableCollection<(string language, string relativePath)>();

        public ObservableCollection<(string language, string relativePath)> ItemDataFiles { get; set; } = new ObservableCollection<(string language, string relativePath)>();

        public ObservableCollection<(string language, string relativePath)> RuneDataFiles { get; set; } = new ObservableCollection<(string language, string relativePath)>();

        public async Task<string> SaveToJson(string dataPath)
        {
            var jsonModel = new Bundle()
            {
                Patch = Patch,
                LastDownloadDate = LastDownloadDate,
            };

            jsonModel.ChampionImageFiles.AddRange(ChampionImageFiles);
            jsonModel.ItemImageFiles.AddRange(ItemImageFiles);

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
                result.ChampionImageFiles.Add(championImageFile);
            }
            foreach (var itemImageFile in jsonModel.ItemImageFiles)
            {
                result.ItemImageFiles.Add(itemImageFile);
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
