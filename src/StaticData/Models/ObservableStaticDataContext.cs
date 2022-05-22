using Etirps.RiZhi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        public ObservableBundle GetBundle(string version)
        {
            // try to get an existing bundle, create new otherwise
            var result = Bundles.FirstOrDefault(x => x.Patch.StartsWith(version));

            if (result == null)
            {
                result = new ObservableBundle()
                {
                    Patch = version
                };
                Bundles.Add(result);
            }

            return result;
        }

        public void DeleteBundle(string dataPath, string patchVersion)
        {
            Directory.Delete(Path.Combine(dataPath, patchVersion), true);

            var deletedBundle = Bundles.First(x => x.Patch == patchVersion)
                ?? throw new Exception($"cannot find bundle to delete: {patchVersion}");

            Bundles.Remove(deletedBundle);
        }

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
                jsonModel.DownloadedBundles.Add(bundle.Patch, relativeBundlePath);
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
    }
}
