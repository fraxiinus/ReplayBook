using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class ObservableStaticDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private DateTimeOffset _lastPatchFetch;
        /// <summary>
        /// Last time patch versions were fetched
        /// </summary>
        public DateTimeOffset LastPatchFetch
        {
            get { return _lastPatchFetch; }
            set
            {
                _lastPatchFetch = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(LastPatchFetch)));
            }
        }

        public ObservableCollection<ObservableBundle> Bundles { get; set; } = new ObservableCollection<ObservableBundle>();
        
        /// <summary>
        /// List of all known patch versions available to download
        /// </summary>
        public ObservableCollection<string> KnownPatchNumbers { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Creates or returns existing Bundle
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public ObservableBundle? GetBundle(string version, bool createNewIfNotFound = true)
        {
            // Check input version for correct length
            if ((version.Split('.').Length - 1) > 2)
            {
                version = version.VersionSubstring();
            }

            // try to get an existing bundle, create new otherwise
            var result = Bundles.FirstOrDefault(x => x.Patch.StartsWith(version));

            if (result == null && createNewIfNotFound)
            {
                result = new ObservableBundle()
                {
                    Patch = version,
                    PatchSortNumber = CalculateSortNumber(version)
                };
                Bundles.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Get first Bundle with a download date
        /// </summary>
        /// <returns></returns>
        public ObservableBundle? GetFirstDownloadedBundle()
        {
            return Bundles.FirstOrDefault(x => x.LastDownloadDate > DateTimeOffset.MinValue);
        }

        /// <summary>
        /// Gets Bundle that closest matches a given patch
        /// </summary>
        /// <returns></returns>
        public ObservableBundle? GetAdjacentDownloadedBundle(string version)
        {
            var requestedSortNumber = CalculateSortNumber(version);

            var foundBundle = Bundles.OrderBy(x => Math.Abs(x.PatchSortNumber - requestedSortNumber))
                .FirstOrDefault();

            return foundBundle;
        }

        /// <summary>
        /// Delete bundle from disk
        /// </summary>
        /// <param name="dataPath"></param>
        /// <param name="patchVersion"></param>
        /// <exception cref="Exception"></exception>
        public void DeleteBundle(string dataPath, string patchVersion)
        {
            var deletePath = Path.Combine(dataPath, patchVersion);
            if (Directory.Exists(deletePath))
            {
                Directory.Delete(deletePath, true);
            }

            var deletedBundle = Bundles.First(x => x.Patch == patchVersion)
                ?? throw new Exception($"cannot find bundle to delete: {patchVersion}");

            Bundles.Remove(deletedBundle);
        }

        /// <summary>
        /// Save this object to file
        /// </summary>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        public async Task SaveToJson(string dataPath)
        {
            var jsonModel = new BundleIndex
            {
                LastPatchFetch = LastPatchFetch
            };

            // Save all bundles first
            foreach (var bundle in Bundles)
            {
                var relativeBundlePath = await bundle.SaveToJson(dataPath);
                if (relativeBundlePath != null)
                {
                    jsonModel.DownloadedBundles.Add(bundle.Patch, relativeBundlePath);
                }
            }

            jsonModel.PatchVersions.AddRange(KnownPatchNumbers);

            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            var savePath = Path.Combine(dataPath, "index.json");
            await using FileStream bundleFile = File.Create(savePath);
            await JsonSerializer.SerializeAsync(bundleFile, jsonModel, serializerOptions);
        }

        /// <summary>
        /// Load data from JSON file
        /// </summary>
        /// <param name="dataPath"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<ObservableStaticDataContext> CreateFromJson(string dataPath, RiZhi log)
        {
            var result = new ObservableStaticDataContext();

            // load index data
            await using FileStream indexFile = File.OpenRead(Path.Combine(dataPath, "index.json"));
            var jsonModel = await JsonSerializer.DeserializeAsync<BundleIndex>(indexFile)
                ?? throw new Exception("index load null");

            result.LastPatchFetch = jsonModel.LastPatchFetch;

            var deleteFromIndex = new List<string>();
            foreach (var (patch, relativePath) in jsonModel.DownloadedBundles)
            {
                var bundleFilePath = Path.Combine(dataPath, relativePath);
                if (!File.Exists(bundleFilePath))
                {
                    log.Information($"bundle {patch} is missing: {relativePath}");
                    deleteFromIndex.Add(patch);
                    continue;
                }
                var loadedBundle = await ObservableBundle.CreateFromJson(Path.Combine(dataPath, relativePath));
                result.Bundles.Add(loadedBundle);
            }
            foreach (var deletedBundle in deleteFromIndex)
            {
                log.Information($"Removing {deletedBundle} from index");
                jsonModel.DownloadedBundles.Remove(deletedBundle);
            }

            foreach (var patchNumber in jsonModel.PatchVersions)
            {
                result.KnownPatchNumbers.Add(patchNumber);
            }

            return result;
        }

        private static int CalculateSortNumber(string input)
        {
            var numbers = input.Split('.');

            var major = int.TryParse(numbers[0], out int majorParse)
                ? majorParse
                : throw new Exception($"cannot parse number {numbers[0]}");

            var minor = int.TryParse(numbers[1], out int minorParse)
                ? minorParse
                : throw new Exception($"cannot parse number {numbers[1]}");

            return (major * 100) + minor;
        }
    }
}
